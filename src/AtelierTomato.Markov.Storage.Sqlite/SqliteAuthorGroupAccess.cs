using AtelierTomato.Markov.Model;
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

		public async Task DeleteAuthorGroup(ulong ID)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			await connection.ExecuteAsync($@"DELETE FROM {nameof(AuthorGroup)} WHERE {nameof(AuthorGroup.ID)} IS @id",
				new
				{
					id = ID
				});

			connection.Close();
		}

		public async Task<AuthorGroup?> ReadAuthorGroup(ulong ID) => (await ReadAuthorGroups([ID])).FirstOrDefault();
		public async Task<IEnumerable<AuthorGroup>> ReadAuthorGroups(IEnumerable<ulong> IDs)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<AuthorGroup>($@"
SELECT {nameof(AuthorGroup.ID)}, {nameof(AuthorGroup.Name)} FROM {nameof(AuthorGroup)}
WHERE {nameof(AuthorGroup.ID)} IN @ids
",
			new
			{
				ids = IDs
			});

			connection.Close();

			return result;
		}

		public async Task WriteAuthorGroup(AuthorGroup authorGroup)
		{
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
				id = authorGroup.ID,
				name = authorGroup.Name
			});

			connection.Close();
		}

		public async Task<ulong> WriteNewAuthorGroup(string name)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var ID = await connection.ExecuteScalarAsync<ulong>($@"
INSERT INTO {nameof(AuthorGroup)} ( {nameof(AuthorGroup.Name)} )
VALUES ( @name );
SELECT last_insert_rowid();
",
			new
			{
				name
			});

			return ID;
		}
	}
}
