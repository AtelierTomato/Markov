using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Storage.Sqlite.Model;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Storage.Sqlite
{
	public class SqliteAuthorGroupRequestAccess : IAuthorGroupRequestAccess
	{
		private readonly SqliteAccessOptions options;
		public SqliteAuthorGroupRequestAccess(IOptions<SqliteAccessOptions> options)
		{
			this.options = options.Value;
		}

		public async Task DeleteAuthorGroupRequest(Guid ID, AuthorOID author)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			await connection.ExecuteAsync($@"DELETE FROM {nameof(AuthorGroup)}Request WHERE {nameof(AuthorGroupPermission.ID)} IS @id AND {nameof(AuthorGroupPermission.Author)} IS @author",
				new
				{
					id = ID.ToString(),
					author = author.ToString()
				});

			connection.Close();
		}

		public async Task<AuthorGroupPermission?> ReadAuthorGroupRequest(Guid ID, AuthorOID author)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QuerySingleOrDefaultAsync<AuthorGroupPermissionRow>($@"
SELECT {nameof(AuthorGroupPermission.ID)}, {nameof(AuthorGroupPermission.Author)}, {nameof(AuthorGroupPermission.Permissions)} FROM {nameof(AuthorGroup)}Request WHERE
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

		public async Task<IEnumerable<AuthorGroupPermission>> ReadAuthorGroupRequestRangeByAuthor(AuthorOID author)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<AuthorGroupPermissionRow>($@"
SELECT {nameof(AuthorGroupPermission.ID)}, {nameof(AuthorGroupPermission.Author)}, {nameof(AuthorGroupPermission.Permissions)} FROM {nameof(AuthorGroup)}Request
WHERE {nameof(AuthorGroupPermission.Author)} IS @author
",
			new
			{
				author = author.ToString()
			});

			connection.Close();

			return result.Select(u => u.ToAuthorGroupPermission());
		}

		public async Task<IEnumerable<AuthorGroupPermission>> ReadAuthorGroupRequestRangeByID(Guid ID)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<AuthorGroupPermissionRow>($@"
SELECT {nameof(AuthorGroupPermission.ID)}, {nameof(AuthorGroupPermission.Author)}, {nameof(AuthorGroupPermission.Permissions)} FROM {nameof(AuthorGroup)}Request
WHERE {nameof(AuthorGroupPermission.ID)} IS @id
",
			new
			{
				id = ID.ToString()
			});

			connection.Close();

			return result.Select(u => u.ToAuthorGroupPermission());
		}

		public async Task WriteAuthorGroupRequest(AuthorGroupPermission authorGroupPermission) => await WriteAuthorGroupRequestRange([authorGroupPermission]);

		public async Task WriteAuthorGroupRequestRange(IEnumerable<AuthorGroupPermission> authorGroupPermissions)
		{
			var authorGroupPermissionRows = authorGroupPermissions.Select(a => new AuthorGroupPermissionRow(a));
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();
			await using var transaction = await connection.BeginTransactionAsync();
			foreach (AuthorGroupPermissionRow authorGroupPermissionRow in authorGroupPermissionRows)
			{
				await connection.ExecuteAsync($@"
INSERT INTO {nameof(AuthorGroup)}Request ( {nameof(AuthorGroupPermission.ID)}, {nameof(AuthorGroupPermission.Author)}, {nameof(AuthorGroupPermission.Permissions)} )
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
