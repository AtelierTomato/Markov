using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Storage.Sqlite.Model;
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

		public async Task DeleteAuthorFromAuthorGroup(Guid ID, AuthorOID author)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			await connection.ExecuteAsync($@"DELETE FROM {nameof(AuthorGroupPermission)} WHERE {nameof(AuthorGroupPermission.ID)} IS @id AND {nameof(AuthorGroupPermission.Author)} IS @author",
				new
				{
					id = ID.ToString(),
					author = author.ToString()
				});

			connection.Close();
		}

		public async Task<AuthorGroupPermission?> ReadAuthorGroupPermission(Guid ID, AuthorOID author)
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
				id = ID.ToString(),
				author = author.ToString()
			});

			connection.Close();

			return result?.ToAuthorGroupPermission();
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
				author = author.ToString()
			});

			connection.Close();

			return result.Select(u => u.ToAuthorGroupPermission());
		}

		public async Task<IEnumerable<AuthorGroupPermission>> ReadAuthorGroupPermissionRangeByID(Guid ID)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<AuthorGroupPermissionRow>($@"
SELECT {nameof(AuthorGroupPermission.ID)}, {nameof(AuthorGroupPermission.Author)}, {nameof(AuthorGroupPermission.Permissions)} FROM {nameof(AuthorGroupPermission)}
WHERE {nameof(AuthorGroupPermission.ID)} IS @id
",
			new
			{
				id = ID.ToString()
			});

			connection.Close();

			return result.Select(u => u.ToAuthorGroupPermission());
		}

		public async Task WriteAuthorGroupPermission(AuthorGroupPermission authorGroupPermission) => await WriteAuthorGroupPermissionRange([authorGroupPermission]);

		public async Task WriteAuthorGroupPermissionRange(IEnumerable<AuthorGroupPermission> authorGroupPermissions)
		{
			var authorGroupPermissionRows = authorGroupPermissions.Select(a => new AuthorGroupPermissionRow(a));
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();
			await using var transaction = await connection.BeginTransactionAsync();
			foreach (AuthorGroupPermissionRow authorGroupPermissionRow in authorGroupPermissionRows)
			{
				await connection.ExecuteAsync($@"
INSERT INTO {nameof(AuthorGroupPermission)} ( {nameof(AuthorGroupPermission.ID)}, {nameof(AuthorGroupPermission.Author)}, {nameof(AuthorGroupPermission.Permissions)} )
Values ( @id, @author, @permissions )
ON CONFLICT ({nameof(AuthorGroupPermission.ID)}, {nameof(AuthorGroupPermission.Author)}) DO UPDATE SET
{nameof(AuthorGroupPermission.Permissions)} = excluded.{nameof(AuthorGroupPermission.Permissions)}
",
				new
				{
					id = authorGroupPermissionRow.ID,
					author = authorGroupPermissionRow.Author,
					permissions = authorGroupPermissionRow.Permissions
				});
			}

			await transaction.CommitAsync();
		}
	}
}
