using AtelierTomato.Markov.Model;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Storage.Sqlite
{
	public class SqliteAuthorGroupPermissionAccess : IAuthorGroupPermissionAccess
	{
		private readonly SqliteAccessOptions options;
		public SqliteAuthorGroupPermissionAccess(IOptions<SqliteAccessOptions> options)
		{
			this.options = options.Value;
		}

		public async Task DeleteAuthorGroup(string ID)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			await connection.ExecuteAsync($@"DELETE FROM {nameof(AuthorGroupPermission)} WHERE {nameof(AuthorGroupPermission.ID)} IS @id",
				new
				{
					id = ID
				});

			connection.Close();
		}

		public async Task DeleteAuthorFromAuthorGroup(string ID, AuthorOID author)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			await connection.ExecuteAsync($@"DELETE FROM {nameof(AuthorGroupPermission)} WHERE {nameof(AuthorGroupPermission.ID)} IS @id AND {nameof(AuthorGroupPermission.Author)} IS @author",
				new
				{
					id = ID,
					author
				});

			connection.Close();
		}

		public async Task<AuthorGroupPermission?> ReadAuthorGroupPermission(string ID, AuthorOID author)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QuerySingleOrDefaultAsync<AuthorGroupPermissionRow>($@"
SELECT {nameof(AuthorGroupPermission.ID)}, {nameof(AuthorGroupPermission.Author)}, {nameof(AuthorGroupPermission.Permissions)} FROM {nameof(AuthorGroupPermission)} WHERE
{nameof(AuthorGroupPermission.ID)} IS @id AND
{nameof(AuthorGroupPermission.Author)} IS @author
",
			new
			{
				id = ID,
				author
			});

			connection.Close();

			return result?.ToUserGroup();
		}

		public async Task<IEnumerable<AuthorGroupPermission>> ReadAuthorGroupPermissionRangeByAuthor(AuthorOID author)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<AuthorGroupPermissionRow>($@"
SELECT {nameof(AuthorGroupPermission.ID)}, {nameof(AuthorGroupPermission.Author)}, {nameof(AuthorGroupPermission.Permissions)} FROM {nameof(AuthorGroupPermission)}
WHERE {nameof(AuthorGroupPermission.Author)} IS @author
",
			new
			{
				author
			});

			connection.Close();

			return result.Select(u => u.ToUserGroup());
		}

		public async Task<IEnumerable<AuthorGroupPermission>> ReadAuthorGroupPermissionRangeByID(string ID)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<AuthorGroupPermissionRow>($@"
SELECT {nameof(AuthorGroupPermission.ID)}, {nameof(AuthorGroupPermission.Author)}, {nameof(AuthorGroupPermission.Permissions)} FROM {nameof(AuthorGroupPermission)}
WHERE {nameof(AuthorGroupPermission.ID)} IS @id
",
			new
			{
				id = ID
			});

			connection.Close();

			return result.Select(u => u.ToUserGroup());
		}

		public async Task WriteAuthorGroupPermission(AuthorGroupPermission authorGroupPermission) => await WriteAuthorGroupPermissionRange([authorGroupPermission]);

		public async Task WriteAuthorGroupPermissionRange(IEnumerable<AuthorGroupPermission> authorGroupPermissions)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();
			await using var transaction = await connection.BeginTransactionAsync();
			foreach (AuthorGroupPermission userGroup in authorGroupPermissions)
			{
				await connection.ExecuteAsync($@"
INSERT INTO {nameof(AuthorGroupPermission)} ( {nameof(AuthorGroupPermission)}
Values ( @id, @author, @permissions )
ON CONFLICT ({nameof(AuthorGroupPermission.ID)}, {nameof(AuthorGroupPermission.Author)}) DO UPDATE SET
{nameof(AuthorGroupPermission.Permissions)} = excluded.{nameof(AuthorGroupPermission.Permissions)}
",
				new
				{
					id = userGroup.ID,
					author = userGroup.Author,
					permissions = userGroup.Permissions
				});
			}

			await transaction.CommitAsync();
		}
	}
}
