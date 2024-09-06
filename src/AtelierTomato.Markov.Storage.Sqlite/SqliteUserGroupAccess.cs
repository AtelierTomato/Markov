using AtelierTomato.Markov.Model;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Storage.Sqlite
{
	public class SqliteUserGroupAccess : IUserGroupAccess
	{
		private readonly SqliteAccessOptions options;
		public SqliteUserGroupAccess(IOptions<SqliteAccessOptions> options)
		{
			this.options = options.Value;
		}

		public async Task DeleteUserGroup(string ID)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			await connection.ExecuteAsync($@"DELETE FROM {nameof(UserGroup)} WHERE {nameof(UserGroup.ID)} IS @id",
				new
				{
					id = ID
				});

			connection.Close();
		}

		public async Task DeleteUserFromUserGroup(string ID, AuthorOID author)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			await connection.ExecuteAsync($@"DELETE FROM {nameof(UserGroup)} WHERE {nameof(UserGroup.ID)} IS @id AND {nameof(UserGroup.Author)} IS @author",
				new
				{
					id = ID,
					author
				});

			connection.Close();
		}

		public async Task<UserGroup?> ReadUserGroup(string ID, AuthorOID author)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QuerySingleOrDefaultAsync<UserGroupRow>($@"
SELECT {nameof(UserGroup.ID)}, {nameof(UserGroup.Author)}, {nameof(UserGroup.Permissions)} FROM {nameof(UserGroup)} WHERE
{nameof(UserGroup.ID)} IS @id AND
{nameof(UserGroup.Author)} IS @author
",
			new
			{
				id = ID,
				author
			});

			connection.Close();

			return result?.ToUserGroup();
		}

		public async Task<IEnumerable<UserGroup>> ReadUserGroupRangeByAuthor(AuthorOID author)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<UserGroupRow>($@"
SELECT {nameof(UserGroup.ID)}, {nameof(UserGroup.Author)}, {nameof(UserGroup.Permissions)} FROM {nameof(UserGroup)}
WHERE {nameof(UserGroup.Author)} IS @author
",
			new
			{
				author
			});

			connection.Close();

			return result.Select(u => u.ToUserGroup());
		}

		public async Task<IEnumerable<UserGroup>> ReadUserGroupRangeByID(string ID)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<UserGroupRow>($@"
SELECT {nameof(UserGroup.ID)}, {nameof(UserGroup.Author)}, {nameof(UserGroup.Permissions)} FROM {nameof(UserGroup)}
WHERE {nameof(UserGroup.ID)} IS @id
",
			new
			{
				id = ID
			});

			connection.Close();

			return result.Select(u => u.ToUserGroup());
		}

		public async Task WriteUserGroup(UserGroup userGroup) => await WriteUserGroupRange([userGroup]);

		public async Task WriteUserGroupRange(IEnumerable<UserGroup> userGroups)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();
			await using var transaction = await connection.BeginTransactionAsync();
			foreach (UserGroup userGroup in userGroups)
			{
				await connection.ExecuteAsync($@"
INSERT INTO {nameof(UserGroup)} ( {nameof(UserGroup)}
Values ( @id, @author, @permissions )
ON CONFLICT ({nameof(UserGroup.ID)}, {nameof(UserGroup.Author)}) DO UPDATE SET
{nameof(UserGroup.Permissions)} = excluded.{nameof(UserGroup.Permissions)}
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
