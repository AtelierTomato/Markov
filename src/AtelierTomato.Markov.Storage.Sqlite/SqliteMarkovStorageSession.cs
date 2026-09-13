using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Storage.Sqlite.Model;
using Dapper;
using Microsoft.Data.Sqlite;

namespace AtelierTomato.Markov.Storage.Sqlite
{
	public class SqliteMarkovStorageSession : IMarkovStorageSession
	{
		private readonly SqliteAccessOptions options;
		private readonly MultiParser<IObjectOID> objectOIDParser;
		private SqliteConnection connection;
		private readonly string tempTableName = "MarkovTempTable";
		public SqliteMarkovStorageSession(SqliteAccessOptions options, MultiParser<IObjectOID> objectOIDParser)
		{
			this.options = options;
			this.objectOIDParser = objectOIDParser;
			connection = new SqliteConnection(options.ConnectionString);
			connection.Open();
		}

		public async Task CreateTempTable(SentenceFilter filter, IObjectOID? queryScope = null)
		{
			// This has different behavior based on whether or not we have an OID filter or not, so if.
			if (filter.OIDs.Any())
			{
				// First, we need to make an even temp-er table to fuel the query to make the temp table, we first delete it if it exists (it shouldn't).
				await connection.ExecuteAsync($@"
DROP TABLE IF EXISTS {nameof(SentenceFilter)}{nameof(SentenceFilter.OIDs)};
				");

				// Create the temper table.
				await connection.ExecuteAsync(@$"
CREATE TEMPORARY TABLE IF NOT EXISTS {nameof(SentenceFilter)}{nameof(SentenceFilter.OIDs)} (
	{nameof(Sentence.OID)}	TEXT NOT NULL UNIQUE,
	PRIMARY KEY ({nameof(Sentence.OID)})
);
				");
				await using var transaction = await connection.BeginTransactionAsync();

				foreach (var objectOID in filter.OIDs)
				{
					await connection.ExecuteAsync($@"
INSERT INTO {nameof(SentenceFilter)}{nameof(SentenceFilter.OIDs)} ( {nameof(Sentence.OID)} )
VALUES ( @oid )
ON CONFLICT DO NOTHING
",
						new
						{
							oid = objectOID.ToString()
						}
					);
				}

				await transaction.CommitAsync();

				// Now, we can make the real temp table.

				// Just in case, drop the table if it already exists.
				await connection.ExecuteAsync($@"
DROP TABLE IF EXISTS {tempTableName};
				");

				// Now create the new table.
				await connection.ExecuteAsync($@"
CREATE TEMP TABLE IF NOT EXISTS {tempTableName} AS
SELECT {nameof(Sentence.OID)}, {nameof(Sentence.Author)}, {nameof(Sentence.Date)}, {nameof(Sentence.Text)}
FROM SentenceAfterLinkWithPermission INNER JOIN {nameof(SentenceFilter)}{nameof(SentenceFilter.OIDs)}
ON (SentenceAfterLinkWithPermission.{nameof(Sentence.OID)} || ':') LIKE ({nameof(SentenceFilter)}{nameof(SentenceFilter.OIDs)}.{nameof(Sentence.OID)} || ':%')
WHERE
( {nameof(AuthorPermission.AllowedScope)} IS NULL OR {nameof(AuthorPermission.AllowedScope)} IS '' OR @queryScope || ':' LIKE {nameof(AuthorPermission.AllowedScope)} || ':%' ) AND
( @hasAuthors IS 0 OR {nameof(Sentence.Author)} IN @authors )
",
					new
					{
						queryScope = queryScope?.ToString(),
						hasAuthors = filter.Authors.Any(),
						authors = filter.Authors,
					}
				);

				// We've got our table, drop the temp-er one.
				await connection.ExecuteAsync($@"
DROP TABLE {nameof(SentenceFilter)}{nameof(SentenceFilter.OIDs)}
				");
			}
			else
			{
				// Just in case, drop the table if it already exists.
				await connection.ExecuteAsync($@"
DROP TABLE IF EXISTS {tempTableName};
				");

				// Now create the new table.
				await connection.ExecuteAsync($@"
CREATE TEMP TABLE IF NOT EXISTS {tempTableName} AS
SELECT {nameof(Sentence.OID)}, {nameof(Sentence.Author)}, {nameof(Sentence.Date)}, {nameof(Sentence.Text)}
FROM SentenceAfterLinkWithPermission
WHERE
( {nameof(AuthorPermission.AllowedScope)} IS NULL OR {nameof(AuthorPermission.AllowedScope)} IS '' OR @queryScope || ':' LIKE {nameof(AuthorPermission.AllowedScope)} || ':%' ) AND
( @hasAuthors IS 0 OR {nameof(Sentence.Author)} IN @authors )
",
					new
					{
						queryScope = queryScope?.ToString(),
						hasAuthors = filter.Authors.Any(),
						authors = filter.Authors,
					}
				);

				// We've got our table, drop the temp-er one.
				await connection.ExecuteAsync($@"
DROP TABLE {nameof(SentenceFilter)}{nameof(SentenceFilter.OIDs)}
				");
			}
		}

		public void Dispose()
		{
			connection.Close();
		}

		public async Task<Sentence?> ReadRandomSentence(string? keyword = null)
		{
			SentenceRow? result;
			result = await connection.QuerySingleOrDefaultAsync<SentenceRow?>($@"
SELECT {nameof(Sentence.OID)}, {nameof(Sentence.Author)}, {nameof(Sentence.Date)}, {nameof(Sentence.Text)}
FROM {tempTableName}
ORDER BY
	CASE
		WHEN @keyword IS NOT NULL
		 AND {nameof(Sentence.Text)} LIKE '% ' || @keyword || ' %'
		THEN 1
		ELSE 2
	END,
	RANDOM()
LIMIT 1
",
				new
				{
					keyword
				}
			);

			return result?.ToSentence(objectOIDParser);
		}

		public async Task<IEnumerable<Sentence>> ReadNextRandomSentences(int amount, List<string> prevList, List<IObjectOID> previousIDs, string? keyword = null)
		{
			IEnumerable<SentenceRow> result;
			result = await connection.QueryAsync<SentenceRow>($@"
SELECT {nameof(Sentence.OID)}, {nameof(Sentence.Author)}, {nameof(Sentence.Date)}, {nameof(Sentence.Text)}
FROM {tempTableName}
WHERE
	( {nameof(Sentence.OID)} NOT IN @previousIDs ) AND
	( {nameof(Sentence.Text)} LIKE '% ' || @prevList || ' %' )
ORDER BY
	CASE
		WHEN @keyword IS NOT NULL
		 AND {nameof(Sentence.Text)} LIKE '% ' || @keyword || ' %'
		THEN 1
		ELSE 2
	END,
	RANDOM()
LIMIT @amount
",
				new
				{
					previousIDs = previousIDs.Select(x => x.ToString()),
					prevList = string.Join(' ', prevList),
					keyword,
					amount
				}
			);

			return result.Select(s => s.ToSentence(objectOIDParser));
		}

	}
}
