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
		private readonly MultiParser<IObjectOID> objectOIDParser;
		public SqliteAuthorPermissionAccess(IOptions<SqliteAccessOptions> options, MultiParser<IObjectOID> objectOIDParser)
		{
			this.options = options.Value;
			this.objectOIDParser = objectOIDParser;
		}

		public async Task<IEnumerable<AuthorPermission>> ReadAllAuthorPermissions()
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<AuthorPermissionRow>($@"
SELECT {nameof(AuthorPermission.Author)}, {nameof(AuthorPermission.QueryScope)}, {nameof(AuthorPermission.AllowedScope)}
FROM {nameof(AuthorPermission)}
			");

			connection.Close();

			return result.Select(u => u.ToAuthorPermission(objectOIDParser));
		}

		public async Task<AuthorPermission?> ReadAuthorPermission(AuthorOID author, IObjectOID queryScope)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QuerySingleOrDefaultAsync<AuthorPermissionRow>($@"
SELECT {nameof(AuthorPermission.Author)}, {nameof(AuthorPermission.QueryScope)}, {nameof(AuthorPermission.AllowedScope)}
FROM {nameof(AuthorPermission)} WHERE
{nameof(AuthorPermission.Author)} = @author AND
@queryScope || ':' LIKE {nameof(AuthorPermission.QueryScope)} || ':%'
ORDER BY LENGTH ({nameof(AuthorPermission.QueryScope)}) DESC LIMIT 1
",
			new
			{
				author,
				queryScope
			});

			connection.Close();

			return result?.ToAuthorPermission(objectOIDParser);
		}

		public async Task<IEnumerable<AuthorPermission>> ReadAuthorPermissionRange(IEnumerable<AuthorOID> authors, IEnumerable<IObjectOID> queryScopes)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<AuthorPermissionRow>($@"
SELECT {nameof(AuthorPermission.Author)}, {nameof(AuthorPermission.QueryScope)}, {nameof(AuthorPermission.AllowedScope)}
FROM {nameof(AuthorPermission)}
WHERE {nameof(AuthorPermission.Author)} in @authors AND {nameof(AuthorPermission.QueryScope)} in @queryScopes
",
			new
			{
				authors,
				queryScopes
			});

			connection.Close();

			return result.Select(u => u.ToAuthorPermission(objectOIDParser));
		}

		private static async Task WriteCore(SqliteConnection connection, AuthorPermission authorPermission)
		{
			AuthorPermissionRow authorPermissionRow = new AuthorPermissionRow(authorPermission);
			await connection.ExecuteAsync($@"
INSERT INTO {nameof(AuthorPermission)} ( {nameof(AuthorPermission.Author)}, {nameof(AuthorPermission.QueryScope)}, {nameof(AuthorPermission.AllowedScope)} )
VALUES ( @author, @queryScope, @allowedScope )
ON CONFLICT ( {nameof(AuthorPermission.Author)}, {nameof(AuthorPermission.QueryScope)} ) DO UPDATE SET
{nameof(AuthorPermission.AllowedScope)} = excluded.{nameof(AuthorPermission.AllowedScope)}
",
			new
			{
				author = authorPermissionRow.Author,
				queryScope = authorPermissionRow.QueryScope,
				allowedScope = authorPermissionRow.AllowedScope
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
