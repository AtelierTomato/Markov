using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Storage.Sqlite.Model;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Storage.Sqlite
{
	public class SqliteAuthorSettingAccess : IAuthorSettingAccess
	{
		private readonly SqliteAccessOptions options;
		private readonly MultiParser<IObjectOID> objectOIDParser;
		public SqliteAuthorSettingAccess(IOptions<SqliteAccessOptions> options, MultiParser<IObjectOID> objectOIDParser)
		{
			this.options = options.Value;
			this.objectOIDParser = objectOIDParser;
		}
		public async Task DeleteAuthorSetting(AuthorOID author, IObjectOID location)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			await connection.ExecuteAsync($@"
DELETE FROM {nameof(AuthorSetting)} WHERE
{nameof(AuthorSetting.Author)} IS @author AND
{nameof(AuthorSetting.Location)} IS @location
",
			new
			{
				author = author.ToString(),
				location = location.ToString()
			});

			connection.Close();
		}

		public async Task DeleteAuthorSettingRangeByAuthor(AuthorOID author)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			await connection.ExecuteAsync($@"DELETE FROM {nameof(AuthorSetting)} WHERE {nameof(AuthorSetting.Author)} IS @author",
				new
				{
					author = author.ToString()
				});

			connection.Close();
		}

		public async Task DeleteAuthorSettingRangeByLocation(IObjectOID location)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			await connection.ExecuteAsync($@"DELETE FROM {nameof(AuthorSetting)} WHERE {nameof(AuthorSetting.Location)} IS @location",
				new
				{
					location = location.ToString()
				});

			connection.Close();
		}

		public async Task<IEnumerable<AuthorSetting>> ReadAllAuthorSettings()
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<AuthorSettingRow>($@"
SELECT {nameof(AuthorSetting.Author)}, {nameof(AuthorSetting.Location)}, {nameof(AuthorSetting.DisplayOption)}, {nameof(AuthorSetting.Filter)}{nameof(SentenceFilter.OIDs)}, {nameof(AuthorSetting.Filter)}{nameof(SentenceFilter.Authors)}, {nameof(AuthorSetting.AuthorGroup)}, {nameof(AuthorSetting.LocationGroup)}, {nameof(AuthorSetting.Keyword)}, {nameof(AuthorSetting.FirstWord)}
FROM {nameof(AuthorSetting)}
			");

			connection.Close();
			return result.Select(r => r.ToAuthorSetting(objectOIDParser));
		}

		public async Task<AuthorSetting?> ReadAuthorSetting(AuthorOID author, IObjectOID location)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QuerySingleOrDefaultAsync<AuthorSettingRow?>($@"
SELECT {nameof(AuthorSetting.Author)}, {nameof(AuthorSetting.Location)}, {nameof(AuthorSetting.DisplayOption)}, {nameof(AuthorSetting.Filter)}{nameof(SentenceFilter.OIDs)}, {nameof(AuthorSetting.Filter)}{nameof(SentenceFilter.Authors)}, {nameof(AuthorSetting.AuthorGroup)}, {nameof(AuthorSetting.LocationGroup)}, {nameof(AuthorSetting.Keyword)}, {nameof(AuthorSetting.FirstWord)}
FROM {nameof(AuthorSetting)} WHERE
{nameof(AuthorSetting.Author)} IS @author AND
{nameof(AuthorSetting.Location)} IS @location
",
			new
			{
				author = author.ToString(),
				location = location.ToString()
			});

			connection.Close();
			return result?.ToAuthorSetting(objectOIDParser);
		}

		public async Task<IEnumerable<AuthorSetting>> ReadAuthorSettingRangeByAuthor(AuthorOID author)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<AuthorSettingRow>($@"
SELECT {nameof(AuthorSetting.Author)}, {nameof(AuthorSetting.Location)}, {nameof(AuthorSetting.DisplayOption)}, {nameof(AuthorSetting.Filter)}{nameof(SentenceFilter.OIDs)}, {nameof(AuthorSetting.Filter)}{nameof(SentenceFilter.Authors)}, {nameof(AuthorSetting.AuthorGroup)}, {nameof(AuthorSetting.LocationGroup)}, {nameof(AuthorSetting.Keyword)}, {nameof(AuthorSetting.FirstWord)}
FROM {nameof(AuthorSetting)} WHERE
{nameof(AuthorSetting.Author)} IS @author
",
			new
			{
				author = author.ToString()
			});

			connection.Close();
			return result.Select(r => r.ToAuthorSetting(objectOIDParser));
		}

		public async Task<IEnumerable<AuthorSetting>> ReadAuthorSettingRangeByLocation(IObjectOID location)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<AuthorSettingRow>($@"
SELECT {nameof(AuthorSetting.Author)}, {nameof(AuthorSetting.Location)}, {nameof(AuthorSetting.DisplayOption)}, {nameof(AuthorSetting.Filter)}{nameof(SentenceFilter.OIDs)}, {nameof(AuthorSetting.Filter)}{nameof(SentenceFilter.Authors)}, {nameof(AuthorSetting.AuthorGroup)}, {nameof(AuthorSetting.LocationGroup)}, {nameof(AuthorSetting.Keyword)}, {nameof(AuthorSetting.FirstWord)}
FROM {nameof(AuthorSetting)} WHERE
{nameof(AuthorSetting.Location)} IS @location
",
			new
			{
				location = location.ToString()
			});

			connection.Close();
			return result.Select(r => r.ToAuthorSetting(objectOIDParser));
		}

		public async Task WriteAuthorSetting(AuthorSetting authorSetting)
		{
			AuthorSettingRow authorSettingRow = new AuthorSettingRow(authorSetting);

			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			await connection.ExecuteAsync($@"
INSERT INTO {nameof(AuthorSetting)} ( {nameof(AuthorSetting.Author)}, {nameof(AuthorSetting.Location)}, {nameof(AuthorSetting.DisplayOption)}, {nameof(AuthorSetting.Filter)}{nameof(SentenceFilter.OIDs)}, {nameof(AuthorSetting.Filter)}{nameof(SentenceFilter.Authors)}, {nameof(AuthorSetting.AuthorGroup)}, {nameof(AuthorSetting.LocationGroup)}, {nameof(AuthorSetting.Keyword)}, {nameof(AuthorSetting.FirstWord)} )
VALUES ( @author, @location, @displayOption, @filterOIDs, @filterAuthors, @authorGroup, @locationGroup, @keyword, @firstWord )
ON CONFLICT ( {nameof(AuthorSetting.Author)}, {nameof(AuthorSetting.Location)} ) DO UPDATE SET
{nameof(AuthorSetting.DisplayOption)} = excluded.{nameof(AuthorSetting.DisplayOption)},
{nameof(AuthorSetting.Filter)}{nameof(SentenceFilter.OIDs)} = excluded.{nameof(AuthorSetting.Filter)}{nameof(SentenceFilter.OIDs)},
{nameof(AuthorSetting.Filter)}{nameof(SentenceFilter.Authors)} = excluded.{nameof(AuthorSetting.Filter)}{nameof(SentenceFilter.Authors)},
{nameof(AuthorSetting.AuthorGroup)} = excluded.{nameof(AuthorSetting.AuthorGroup)},
{nameof(AuthorSetting.LocationGroup)} = excluded.{nameof(AuthorSetting.LocationGroup)},
{nameof(AuthorSetting.Keyword)} = excluded.{nameof(AuthorSetting.Keyword)},
{nameof(AuthorSetting.FirstWord)} = excluded.{nameof(AuthorSetting.FirstWord)}
",
			new
			{
				author = authorSettingRow.Author,
				location = authorSettingRow.Location,
				displayOption = authorSettingRow.DisplayOption,
				filterOIDs = authorSettingRow.FilterOIDs,
				filterAuthors = authorSettingRow.FilterAuthors,
				authorGroup = authorSettingRow.AuthorGroup,
				locationGroup = authorSettingRow.LocationGroup,
				keyword = authorSettingRow.Keyword,
				firstWord = authorSettingRow.FirstWord
			});

			connection.Close();
		}
	}
}
