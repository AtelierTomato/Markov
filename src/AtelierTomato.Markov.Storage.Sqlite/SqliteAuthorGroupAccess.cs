using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Storage.Sqlite.Model;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Storage.Sqlite
{
	public class SqliteAuthorGroupAccess : IAuthorGroupAccess
	{
		private readonly SqliteAccessOptions options;
		public SqliteAuthorGroupAccess(IOptions<SqliteAccessOptions> options)
		{
			this.options = options.Value;
		}

		public async Task DeleteAuthorGroup(Guid ID)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			await connection.ExecuteAsync($@"DELETE FROM {nameof(AuthorGroup)} WHERE {nameof(AuthorGroup.ID)} IS @id",
				new
				{
					id = ID.ToString()
				});

			connection.Close();
		}

		public async Task<AuthorGroup?> ReadAuthorGroup(Guid ID)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QuerySingleOrDefaultAsync<AuthorGroupRow>($@"
SELECT {nameof(AuthorGroup.ID)}, {nameof(AuthorGroup.Name)} FROM {nameof(AuthorGroup)}
WHERE {nameof(AuthorGroup.ID)} IS @id
",
			new
			{
				id = ID.ToString()
			});

			connection.Close();

			return result?.ToAuthorGroup();
		}

		public async Task WriteAuthorGroup(AuthorGroup authorGroup)
		{
			var authorGroupRow = new AuthorGroupRow(authorGroup);
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			await connection.ExecuteAsync($@"
INSERT INTO {nameof(AuthorGroup)} ( {nameof(AuthorGroup.ID)}, {nameof(AuthorGroup.Name)} )
Values ( @id, @name )
ON CONFLICT ({nameof(AuthorGroup.ID)}) DO UPDATE SET
{nameof(AuthorGroup.Name)} = excluded.{nameof(AuthorGroup.Name)}
",
			new
			{
				id = authorGroupRow.ID,
				name = authorGroupRow.Name
			});

			connection.Close();
		}
	}
}
