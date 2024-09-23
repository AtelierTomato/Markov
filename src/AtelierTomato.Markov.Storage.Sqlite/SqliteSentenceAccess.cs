using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Storage.Sqlite.Model;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Storage.Sqlite
{
	public class SqliteSentenceAccess : ISentenceAccess
	{
		private readonly SqliteAccessOptions options;
		private readonly MultiParser<IObjectOID> objectOIDParser;
		public SqliteSentenceAccess(IOptions<SqliteAccessOptions> options, MultiParser<IObjectOID> objectOIDParser)
		{
			this.options = options.Value;
			this.objectOIDParser = objectOIDParser;
		}

		public async Task DeleteSentenceRange(SentenceFilter filter, string? searchString = null)
		{
			if (filter is { Authors: [], OIDs: [] } && string.IsNullOrWhiteSpace(searchString))
				throw new ArgumentException("You cannot delete all sentences from the database through this command, at least one part of the filter must have a value.", nameof(filter));

			IEnumerable<string>? authors = null;
			if (filter.Authors is not [])
			{
				authors = filter.Authors!.Select(a => a.ToString());
			}

			await using var connection = new SqliteConnection(options.ConnectionString);

			connection.Open();

			if (filter.OIDs is not [])
			{
				await CreateSentenceFilterOIDsTempTable(filter.OIDs!, connection);

				await connection.ExecuteAsync($@"
DELETE FROM {nameof(Sentence)} INNER JOIN {nameof(SentenceFilter)}{nameof(SentenceFilter.OIDs)}
ON ({nameof(Sentence)}.{nameof(Sentence.OID)} || ':') LIKE ({nameof(SentenceFilter)}{nameof(SentenceFilter.OIDs)}.{nameof(Sentence.OID)} || ':%') WHERE
( @authors IS NULL OR {nameof(Sentence.Author)} IN @authors ) AND
( @searchString IS NULL OR (' ' || {nameof(Sentence.Text)} || ' ') LIKE '% ' || @searchString || ' %' )
",
				new
				{
					authors,
					searchString
				});
			}
			else
			{
				await connection.ExecuteAsync($@"
DELETE FROM {nameof(Sentence)} WHERE
( @authors IS NULL OR {nameof(Sentence.Author)} IN @authors ) AND
( @searchString IS NULL OR (' ' || {nameof(Sentence.Text)} || ' ') LIKE '% ' || @searchString || ' %' )
",
				new
				{
					authors,
					searchString
				});
			}

			connection.Close();
		}

		public async Task<IEnumerable<Sentence>> ReadNextRandomSentences(int amount, List<string> prevList, List<IObjectOID> previousIDs, SentenceFilter filter, string? keyword = null, IObjectOID? queryScope = null)
		{
			IEnumerable<string>? authors = null;
			if (filter.Authors is not [])
			{
				authors = filter.Authors.Select(a => a.ToString());
			}

			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();
			IEnumerable<SentenceRow> result;

			if (filter.OIDs is not [])
			{
				await CreateSentenceFilterOIDsTempTable(filter.OIDs, connection);

				result = await connection.QueryAsync<SentenceRow>($@"
SELECT SentenceAfterLinkWithPermission.{nameof(Sentence.OID)}, {nameof(Sentence.Author)}, {nameof(Sentence.Date)}, {nameof(Sentence.Text)} FROM SentenceAfterLinkWithPermission INNER JOIN {nameof(SentenceFilter)}{nameof(SentenceFilter.OIDs)}
ON (SentenceAfterLinkWithPermission.{nameof(Sentence.OID)} || ':') LIKE ({nameof(SentenceFilter)}{nameof(SentenceFilter.OIDs)}.{nameof(Sentence.OID)} || ':%') WHERE
( {nameof(AuthorPermission.AllowedScope)} IS NULL OR {nameof(AuthorPermission.AllowedScope)} IS '' OR @queryScope || ':' LIKE {nameof(AuthorPermission.AllowedScope)} || ':%' ) AND
( @authors IS NULL OR {nameof(Sentence.Author)} IN @authors ) AND
( SentenceAfterLinkWithPermission.{nameof(Sentence.OID)} NOT IN @previousIDs ) AND
( ( ' ' || {nameof(Sentence.Text)} || ' ') LIKE '% ' || @prevList || ' %' )
ORDER BY
CASE WHEN @keyword IS NOT NULL AND (' ' || {nameof(Sentence.Text)} || ' ') LIKE '% ' || @keyword || ' %' THEN 1 ELSE 2 END,
RANDOM()
LIMIT @amount
",
				new
				{
          queryScope = queryScope?.ToString(),
					authors,
					previousIDs = previousIDs.Select(x => x.ToString()),
					prevList = string.Join(' ', prevList),
          keyword,
					amount
				});
			}
			else
			{
				result = await connection.QueryAsync<SentenceRow>($@"
SELECT SentenceAfterLinkWithPermission.{nameof(Sentence.OID)}, {nameof(Sentence.Author)}, {nameof(Sentence.Date)}, {nameof(Sentence.Text)} FROM SentenceAfterLinkWithPermission WHERE
( {nameof(AuthorPermission.AllowedScope)} IS NULL OR {nameof(AuthorPermission.AllowedScope)} IS '' OR @queryScope || ':' LIKE {nameof(AuthorPermission.AllowedScope)} || ':%' ) AND
( @authors IS NULL OR {nameof(Sentence.Author)} IN @authors ) AND
( SentenceAfterLinkWithPermission.{nameof(Sentence.OID)} NOT IN @previousIDs ) AND
( ( ' ' || {nameof(Sentence.Text)} || ' ') LIKE '% ' || @prevList || ' %' )
ORDER BY
CASE WHEN @keyword IS NOT NULL AND (' ' || {nameof(Sentence.Text)} || ' ') LIKE '% ' || @keyword || ' %' THEN 1 ELSE 2 END,
RANDOM()
LIMIT @amount
",
				new
				{
          queryScope = queryScope?.ToString(),
					authors,
					previousIDs = previousIDs.Select(x => x.ToString()),
					prevList = string.Join(' ', prevList),
          keyword,
					amount
				});
			}

			connection.Close();

			return result.Select(s => s.ToSentence(objectOIDParser));
		}

		public async Task<Sentence?> ReadRandomSentence(SentenceFilter filter, string? keyword = null, IObjectOID? queryScope = null)
		{
			IEnumerable<string>? authors = null;
			if (filter.Authors is not [])
			{
				authors = filter.Authors.Select(a => a.ToString());
			}

			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();
			SentenceRow? result;
			if (filter.OIDs is not [])
			{
				await CreateSentenceFilterOIDsTempTable(filter.OIDs, connection);
        
        result = await connection.QuerySingleOrDefaultAsync<SentenceRow?>($@"
SELECT SentenceAfterLinkWithPermission.{nameof(Sentence.OID)}, {nameof(Sentence.Author)}, {nameof(Sentence.Date)}, {nameof(Sentence.Text)} FROM SentenceAfterLinkWithPermission INNER JOIN {nameof(SentenceFilter)}{nameof(SentenceFilter.OIDs)}
ON (SentenceAfterLinkWithPermission.{nameof(Sentence.OID)} || ':') LIKE ({nameof(SentenceFilter)}{nameof(SentenceFilter.OIDs)}.{nameof(Sentence.OID)} || ':%') WHERE
( {nameof(AuthorPermission.AllowedScope)} IS NULL OR {nameof(AuthorPermission.AllowedScope)} IS '' OR @queryScope || ':' LIKE {nameof(AuthorPermission.AllowedScope)} || ':%' ) AND
( @authors IS NULL OR {nameof(Sentence.Author)} IN @authors )
ORDER BY
CASE WHEN @keyword IS NOT NULL AND (' ' || {nameof(Sentence.Text)} || ' ') LIKE '% ' || @keyword || ' %' THEN 1 ELSE 2 END,
RANDOM()
LIMIT 1
",
				new
				{
          queryScope = queryScope?.ToString(),
					authors,
					keyword
				});
			}
			else
			{
        result = await connection.QuerySingleOrDefaultAsync<SentenceRow?>($@"
SELECT SentenceAfterLinkWithPermission.{nameof(Sentence.OID)}, {nameof(Sentence.Author)}, {nameof(Sentence.Date)}, {nameof(Sentence.Text)} FROM SentenceAfterLinkWithPermission WHERE
( {nameof(AuthorPermission.AllowedScope)} IS NULL OR {nameof(AuthorPermission.AllowedScope)} IS '' OR @queryScope || ':' LIKE {nameof(AuthorPermission.AllowedScope)} || ':%' ) AND
( @authors IS NULL OR {nameof(Sentence.Author)} IN @authors )
ORDER BY
CASE WHEN @keyword IS NOT NULL AND (' ' || {nameof(Sentence.Text)} || ' ') LIKE '% ' || @keyword || ' %' THEN 1 ELSE 2 END,
RANDOM()
LIMIT 1
",
				new
				{
          queryScope = queryScope?.ToString(),
					authors,
					keyword
				});
      }
      
			connection.Close();

			return result?.ToSentence(objectOIDParser);
		}

		public async Task<IEnumerable<Sentence>> ReadSentenceRange(SentenceFilter filter, string? searchString = null)
		{
			IEnumerable<string>? authors = null;
			if (filter.Authors is not [])
			{
				authors = filter.Authors.Select(a => a.ToString());
			}

			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();
			IEnumerable<SentenceRow> result;

			if (filter.OIDs is not [])
			{
				await CreateSentenceFilterOIDsTempTable(filter.OIDs, connection);

				result = await connection.QueryAsync<SentenceRow>($@"
SELECT {nameof(Sentence)}.{nameof(Sentence.OID)}, {nameof(Sentence.Author)}, {nameof(Sentence.Date)}, {nameof(Sentence.Text)} FROM {nameof(Sentence)} INNER JOIN {nameof(SentenceFilter)}{nameof(SentenceFilter.OIDs)}
ON ({nameof(Sentence)}.{nameof(Sentence.OID)} || ':') LIKE ({nameof(SentenceFilter)}{nameof(SentenceFilter.OIDs)}.{nameof(Sentence.OID)} || ':%') WHERE
( @authors IS NULL OR {nameof(Sentence.Author)} IN @authors ) AND
( @searchString IS NULL OR (' ' || {nameof(Sentence.Text)} || ' ') LIKE '% ' || @searchString || ' %' )
",
				new
				{
					authors,
					searchString
				});
			}
			else
			{
				result = await connection.QueryAsync<SentenceRow>($@"
SELECT {nameof(Sentence)}.{nameof(Sentence.OID)}, {nameof(Sentence.Author)}, {nameof(Sentence.Date)}, {nameof(Sentence.Text)} FROM {nameof(Sentence)} WHERE
( @authors IS NULL OR {nameof(Sentence.Author)} IN @authors ) AND
( @searchString IS NULL OR (' ' || {nameof(Sentence.Text)} || ' ') LIKE '% ' || @searchString || ' %' )
",
				new
				{
					authors,
					searchString
				});
			}

			connection.Close();
			return result.Select(s => s.ToSentence(objectOIDParser));
		}

		public async Task WriteSentence(Sentence sentence) => await WriteSentenceRange([sentence]);

		public async Task WriteSentenceRange(IEnumerable<Sentence> sentenceRange)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();
			await using var transaction = await connection.BeginTransactionAsync();

			foreach (Sentence sentence in sentenceRange)
			{
				await WriteCore(sentence, connection);
			}

			await transaction.CommitAsync();
		}

		private async Task WriteCore(Sentence sentence, SqliteConnection connection)
		{
			SentenceRow sentenceRow = new(sentence);
			await connection.ExecuteAsync($@"
insert into {nameof(Sentence)} ( {nameof(Sentence.OID)}, {nameof(Sentence.Author)}, {nameof(Sentence.Date)}, {nameof(Sentence.Text)} )
Values ( @oid, @author, @date, @text )
on conflict ({nameof(Sentence.OID)}) do update set
{nameof(Sentence.Date)} = excluded.{nameof(Sentence.Date)},
{nameof(Sentence.Text)} = excluded.{nameof(Sentence.Text)}
",
			new
			{
				oid = sentenceRow.OID,
				author = sentenceRow.Author,
				date = sentenceRow.Date,
				text = sentenceRow.Text
			});
		}

		private static async Task CreateSentenceFilterOIDsTempTable(IReadOnlyCollection<IObjectOID> objectOIDs, SqliteConnection connection)
		{
			await connection.ExecuteAsync(@$"
CREATE TEMPORARY TABLE IF NOT EXISTS {nameof(SentenceFilter)}{nameof(SentenceFilter.OIDs)} (
	{nameof(Sentence.OID)}	TEXT NOT NULL UNIQUE,
	PRIMARY KEY ({nameof(Sentence.OID)})
);
			");
			await using var transaction = await connection.BeginTransactionAsync();

			foreach (var objectOID in objectOIDs)
			{
				await connection.ExecuteAsync($@"
INSERT INTO {nameof(SentenceFilter)}{nameof(SentenceFilter.OIDs)} ( {nameof(Sentence.OID)} )
VALUES ( @oid )
ON CONFLICT DO NOTHING
				",
				new
				{
					oid = objectOID.ToString()
				});
			}

			await transaction.CommitAsync();
		}
	}
}
