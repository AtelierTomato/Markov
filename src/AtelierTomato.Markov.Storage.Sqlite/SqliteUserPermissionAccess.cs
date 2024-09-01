using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Storage.Sqlite.Model;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Storage.Sqlite
{
	public class SqliteUserPermissionAccess : IUserPermissionAccess
	{
		private readonly SqliteAccessOptions options;
		public SqliteUserPermissionAccess(IOptions<SqliteAccessOptions> options)
		{
			this.options = options.Value;
		}

		public async Task<IEnumerable<UserPermission>> ReadAllUserPermissions()
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<UserPermissionRaw>($@"
SELECT {nameof(UserPermission.Author)}, {nameof(UserPermission.OriginScope)}, {nameof(UserPermission.AllowedScope)}
FROM {nameof(UserPermission)}
			");

			connection.Close();

			return result.Select(u => u.ToUserPermission());
		}

		public async Task<UserPermission?> ReadUserPermission(AuthorOID author, IObjectOID originScope)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QuerySingleOrDefaultAsync<UserPermissionRaw>($@"
SELECT {nameof(UserPermission.Author)}, {nameof(UserPermission.OriginScope)}, {nameof(UserPermission.AllowedScope)}
FROM {nameof(UserPermission)}
WHERE {nameof(UserPermission.Author)} = @author AND {nameof(UserPermission.OriginScope)} = @originScope
",
			new
			{
				author,
				originScope
			});

			connection.Close();

			return result?.ToUserPermission();
		}

		public async Task<IEnumerable<UserPermission>> ReadUserPermissionRange(IEnumerable<AuthorOID> authors, IEnumerable<IObjectOID> originScopes)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<UserPermissionRaw>($@"
SELECT {nameof(UserPermission.Author)}, {nameof(UserPermission.OriginScope)}, {nameof(UserPermission.AllowedScope)}
FROM {nameof(UserPermission)}
WHERE {nameof(UserPermission.Author)} in @authors AND {nameof(UserPermission.OriginScope)} in @originScopes
",
			new
			{
				authors,
				originScopes
			});

			connection.Close();

			return result.Select(u => u.ToUserPermission());
		}

		private static async Task WriteCore(SqliteConnection connection, UserPermission userPermission)
		{
			UserPermissionRaw userPermissionRaw = new UserPermissionRaw(userPermission);
			await connection.ExecuteAsync($@"
INSERT INTO {nameof(UserPermission)} ( {nameof(UserPermission.Author)}, {nameof(UserPermission.OriginScope)}, {nameof(UserPermission.AllowedScope)} )
VALUES ( @author, @originScope, allowedScope )
ON CONFLICT ( {nameof(UserPermission.Author)}, {nameof(UserPermission.OriginScope)} ) DO UPDATE SET
{nameof(UserPermission.AllowedScope)} = excluded.{nameof(UserPermission.AllowedScope)}
",
			new
			{
				author = userPermissionRaw.Author,
				originScope = userPermissionRaw.OriginScope,
				allowedScope = userPermissionRaw.AllowedScope
			});
		}

		public async Task WriteUserPermission(UserPermission userPermission) => await WriteUserPermissionRange([userPermission]);

		public async Task WriteUserPermissionRange(IEnumerable<UserPermission> userPermissions)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();
			await using var transaction = await connection.BeginTransactionAsync();

			foreach (var userPermission in userPermissions)
			{
				await WriteCore(connection, userPermission);
			}

			await transaction.CommitAsync();
		}
	}
}
