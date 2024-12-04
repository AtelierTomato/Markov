using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Storage.Sqlite.Model;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Storage.Sqlite
{
	public class SqliteAuthorRetortConfigAccess : IAuthorRetortConfigAccess
	{
		private readonly SqliteAccessOptions options;
		private readonly MultiParser<IObjectOID> objectOIDParser;
		public SqliteAuthorRetortConfigAccess(IOptions<SqliteAccessOptions> options, MultiParser<IObjectOID> objectOIDParser)
		{
			this.options = options.Value;
			this.objectOIDParser = objectOIDParser;
		}
		public async Task DeleteAuthorRetortConfig(AuthorOID author, IObjectOID location)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			await connection.ExecuteAsync($@"
DELETE FROM {nameof(AuthorRetortConfig)} WHERE
{nameof(AuthorRetortConfig.Author)} IS @author AND
{nameof(AuthorRetortConfig.Location)} IS @location
",
			new
			{
				author = author.ToString(),
				location = location.ToString()
			});

			connection.Close();
		}

		public async Task DeleteAuthorRetortConfigRangeByAuthor(AuthorOID author)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			await connection.ExecuteAsync($@"DELETE FROM {nameof(AuthorRetortConfig)} WHERE {nameof(AuthorRetortConfig.Author)} IS @author",
				new
				{
					author = author.ToString()
				});

			connection.Close();
		}

		public async Task DeleteAuthorRetortConfigRangeByLocation(IObjectOID location)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			await connection.ExecuteAsync($@"DELETE FROM {nameof(AuthorRetortConfig)} WHERE {nameof(AuthorRetortConfig.Location)} IS @location",
				new
				{
					location = location.ToString()
				});

			connection.Close();
		}

		public async Task<IEnumerable<AuthorRetortConfig>> ReadAllAuthorRetortConfigs()
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<AuthorRetortConfigRow>($@"
SELECT {nameof(AuthorRetortConfig.Author)}, {nameof(AuthorRetortConfig.Location)}, {nameof(AuthorRetortConfig.DisplayOption)}, {nameof(AuthorRetortConfig.Filter)}{nameof(SentenceFilter.OIDs)}, {nameof(AuthorRetortConfig.Filter)}{nameof(SentenceFilter.Authors)}, {nameof(AuthorRetortConfig.AuthorGroup)}, {nameof(AuthorRetortConfig.LocationGroup)}, {nameof(AuthorRetortConfig.Keyword)}, {nameof(AuthorRetortConfig.FirstWord)}
FROM {nameof(AuthorRetortConfig)}
			");

			connection.Close();
			return result.Select(r => r.ToAuthorRetortConfig(objectOIDParser));
		}

		public async Task<AuthorRetortConfig?> ReadAuthorRetortConfig(AuthorOID author, IObjectOID location)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QuerySingleOrDefaultAsync<AuthorRetortConfigRow?>($@"
SELECT {nameof(AuthorRetortConfig.Author)}, {nameof(AuthorRetortConfig.Location)}, {nameof(AuthorRetortConfig.DisplayOption)}, {nameof(AuthorRetortConfig.Filter)}{nameof(SentenceFilter.OIDs)}, {nameof(AuthorRetortConfig.Filter)}{nameof(SentenceFilter.Authors)}, {nameof(AuthorRetortConfig.AuthorGroup)}, {nameof(AuthorRetortConfig.LocationGroup)}, {nameof(AuthorRetortConfig.Keyword)}, {nameof(AuthorRetortConfig.FirstWord)}
FROM {nameof(AuthorRetortConfig)} WHERE
{nameof(AuthorRetortConfig.Author)} IS @author AND
@location || ':' LIKE {nameof(AuthorRetortConfig.Location)} || ':%'
ORDER BY LENGTH ({nameof(AuthorRetortConfig.Location)}) DESC LIMIT 1
",
			new
			{
				author = author.ToString(),
				location = location.ToString()
			});

			connection.Close();
			return result?.ToAuthorRetortConfig(objectOIDParser);
		}

		public async Task<IEnumerable<AuthorRetortConfig>> ReadAuthorRetortConfigRangeByAuthor(AuthorOID author)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<AuthorRetortConfigRow>($@"
SELECT {nameof(AuthorRetortConfig.Author)}, {nameof(AuthorRetortConfig.Location)}, {nameof(AuthorRetortConfig.DisplayOption)}, {nameof(AuthorRetortConfig.Filter)}{nameof(SentenceFilter.OIDs)}, {nameof(AuthorRetortConfig.Filter)}{nameof(SentenceFilter.Authors)}, {nameof(AuthorRetortConfig.AuthorGroup)}, {nameof(AuthorRetortConfig.LocationGroup)}, {nameof(AuthorRetortConfig.Keyword)}, {nameof(AuthorRetortConfig.FirstWord)}
FROM {nameof(AuthorRetortConfig)} WHERE
{nameof(AuthorRetortConfig.Author)} IS @author
",
			new
			{
				author = author.ToString()
			});

			connection.Close();
			return result.Select(r => r.ToAuthorRetortConfig(objectOIDParser));
		}

		public async Task<IEnumerable<AuthorRetortConfig>> ReadAuthorRetortConfigRangeByLocation(IObjectOID location)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<AuthorRetortConfigRow>($@"
SELECT {nameof(AuthorRetortConfig.Author)}, {nameof(AuthorRetortConfig.Location)}, {nameof(AuthorRetortConfig.DisplayOption)}, {nameof(AuthorRetortConfig.Filter)}{nameof(SentenceFilter.OIDs)}, {nameof(AuthorRetortConfig.Filter)}{nameof(SentenceFilter.Authors)}, {nameof(AuthorRetortConfig.AuthorGroup)}, {nameof(AuthorRetortConfig.LocationGroup)}, {nameof(AuthorRetortConfig.Keyword)}, {nameof(AuthorRetortConfig.FirstWord)}
FROM {nameof(AuthorRetortConfig)} WHERE
{nameof(AuthorRetortConfig.Location)} IS @location
",
			new
			{
				location = location.ToString()
			});

			connection.Close();
			return result.Select(r => r.ToAuthorRetortConfig(objectOIDParser));
		}

		public async Task WriteAuthorRetortConfig(AuthorRetortConfig authorRetortConfig)
		{
			AuthorRetortConfigRow authorRetortConfigRow = new AuthorRetortConfigRow(authorRetortConfig);

			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			await connection.ExecuteAsync($@"
INSERT INTO {nameof(AuthorRetortConfig)} ( {nameof(AuthorRetortConfig.Author)}, {nameof(AuthorRetortConfig.Location)}, {nameof(AuthorRetortConfig.DisplayOption)}, {nameof(AuthorRetortConfig.Filter)}{nameof(SentenceFilter.OIDs)}, {nameof(AuthorRetortConfig.Filter)}{nameof(SentenceFilter.Authors)}, {nameof(AuthorRetortConfig.AuthorGroup)}, {nameof(AuthorRetortConfig.LocationGroup)}, {nameof(AuthorRetortConfig.Keyword)}, {nameof(AuthorRetortConfig.FirstWord)} )
VALUES ( @author, @location, @displayOption, @filterOIDs, @filterAuthors, @authorGroup, @locationGroup, @keyword, @firstWord )
ON CONFLICT ( {nameof(AuthorRetortConfig.Author)}, {nameof(AuthorRetortConfig.Location)} ) DO UPDATE SET
{nameof(AuthorRetortConfig.DisplayOption)} = excluded.{nameof(AuthorRetortConfig.DisplayOption)},
{nameof(AuthorRetortConfig.Filter)}{nameof(SentenceFilter.OIDs)} = excluded.{nameof(AuthorRetortConfig.Filter)}{nameof(SentenceFilter.OIDs)},
{nameof(AuthorRetortConfig.Filter)}{nameof(SentenceFilter.Authors)} = excluded.{nameof(AuthorRetortConfig.Filter)}{nameof(SentenceFilter.Authors)},
{nameof(AuthorRetortConfig.AuthorGroup)} = excluded.{nameof(AuthorRetortConfig.AuthorGroup)},
{nameof(AuthorRetortConfig.LocationGroup)} = excluded.{nameof(AuthorRetortConfig.LocationGroup)},
{nameof(AuthorRetortConfig.Keyword)} = excluded.{nameof(AuthorRetortConfig.Keyword)},
{nameof(AuthorRetortConfig.FirstWord)} = excluded.{nameof(AuthorRetortConfig.FirstWord)}
",
			new
			{
				author = authorRetortConfigRow.Author,
				location = authorRetortConfigRow.Location,
				displayOption = authorRetortConfigRow.DisplayOption,
				filterOIDs = authorRetortConfigRow.FilterOIDs,
				filterAuthors = authorRetortConfigRow.FilterAuthors,
				authorGroup = authorRetortConfigRow.AuthorGroup,
				locationGroup = authorRetortConfigRow.LocationGroup,
				keyword = authorRetortConfigRow.Keyword,
				firstWord = authorRetortConfigRow.FirstWord
			});

			connection.Close();
		}
	}
}
