using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Storage.Sqlite.Model;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Storage.Sqlite
{
	public class SqliteAuthorPermissionAccess : IAuthorPermissionAccess
	{
		private readonly SqliteAccessOptions options;
		public SqliteAuthorPermissionAccess(IOptions<SqliteAccessOptions> options)
		{
			this.options = options.Value;
		}

		public async Task<IEnumerable<AuthorPermission>> ReadAllAuthorPermissions()
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<AuthorPermissionRow>($@"
SELECT {nameof(AuthorPermission.Author)}, {nameof(AuthorPermission.OriginScope)}, {nameof(AuthorPermission.AllowedScope)}
FROM {nameof(AuthorPermission)}
			");

			connection.Close();

			return result.Select(u => u.ToAuthorPermission());
		}

		public async Task<AuthorPermission?> ReadAuthorPermission(AuthorOID author, IObjectOID originScope)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QuerySingleOrDefaultAsync<AuthorPermissionRow>($@"
SELECT {nameof(AuthorPermission.Author)}, {nameof(AuthorPermission.OriginScope)}, {nameof(AuthorPermission.AllowedScope)}
FROM {nameof(AuthorPermission)}
WHERE {nameof(AuthorPermission.Author)} = @author AND {nameof(AuthorPermission.OriginScope)} = @originScope
",
			new
			{
				author,
				originScope
			});

			connection.Close();

			return result?.ToAuthorPermission();
		}

		public async Task<IEnumerable<AuthorPermission>> ReadAuthorPermissionRange(IEnumerable<AuthorOID> authors, IEnumerable<IObjectOID> originScopes)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<AuthorPermissionRow>($@"
SELECT {nameof(AuthorPermission.Author)}, {nameof(AuthorPermission.OriginScope)}, {nameof(AuthorPermission.AllowedScope)}
FROM {nameof(AuthorPermission)}
WHERE {nameof(AuthorPermission.Author)} in @authors AND {nameof(AuthorPermission.OriginScope)} in @originScopes
",
			new
			{
				authors,
				originScopes
			});

			connection.Close();

			return result.Select(u => u.ToAuthorPermission());
		}

		private static async Task WriteCore(SqliteConnection connection, AuthorPermission authorPermission)
		{
			AuthorPermissionRow authorPermissionRaw = new AuthorPermissionRow(authorPermission);
			await connection.ExecuteAsync($@"
INSERT INTO {nameof(AuthorPermission)} ( {nameof(AuthorPermission.Author)}, {nameof(AuthorPermission.OriginScope)}, {nameof(AuthorPermission.AllowedScope)} )
VALUES ( @author, @originScope, allowedScope )
ON CONFLICT ( {nameof(AuthorPermission.Author)}, {nameof(AuthorPermission.OriginScope)} ) DO UPDATE SET
{nameof(AuthorPermission.AllowedScope)} = excluded.{nameof(AuthorPermission.AllowedScope)}
",
			new
			{
				author = authorPermissionRaw.Author,
				originScope = authorPermissionRaw.OriginScope,
				allowedScope = authorPermissionRaw.AllowedScope
			});
		}

		public async Task WriteAuthorPermission(AuthorPermission authorPermission) => await WriteAuthorPermissionRange([authorPermission]);

		public async Task WriteAuthorPermissionRange(IEnumerable<AuthorPermission> authorPermissions)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();
			await using var transaction = await connection.BeginTransactionAsync();

			foreach (var authorPermission in authorPermissions)
			{
				await WriteCore(connection, authorPermission);
			}

			await transaction.CommitAsync();
		}
	}
}
