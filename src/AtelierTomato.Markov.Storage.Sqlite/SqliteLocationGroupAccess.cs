using AtelierTomato.Markov.Model;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Storage.Sqlite
{
	public class SqliteLocationGroupAccess : ILocationGroupAccess
	{
		private readonly SqliteAccessOptions options;
		public SqliteLocationGroupAccess(IOptions<SqliteAccessOptions> options)
		{
			this.options = options.Value;
		}

		public async Task DeleteLocationGroup(ulong ID)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			await connection.ExecuteAsync($@"DELETE FROM {nameof(LocationGroup)} WHERE {nameof(LocationGroup.ID)} IS @id",
				new
				{
					id = ID
				});

			connection.Close();
		}

		public async Task<LocationGroup?> ReadLocationGroup(ulong ID) => (await ReadLocationGroups([ID])).FirstOrDefault();
		public async Task<IEnumerable<LocationGroup>> ReadLocationGroups(IEnumerable<ulong> IDs)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<LocationGroup>($@"
SELECT {nameof(LocationGroup.ID)}, {nameof(LocationGroup.Name)} FROM {nameof(LocationGroup)}
WHERE {nameof(LocationGroup.ID)} IN @ids
",
			new
			{
				ids = IDs
			});

			connection.Close();

			return result;
		}

		public async Task WriteLocationGroup(LocationGroup locationGroup)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			await connection.ExecuteAsync($@"
INSERT INTO {nameof(LocationGroup)} ( {nameof(LocationGroup.ID)}, {nameof(LocationGroup.Name)} )
Values ( @id, @name )
ON CONFLICT ({nameof(LocationGroup.ID)}) DO UPDATE SET
{nameof(LocationGroup.Name)} = excluded.{nameof(LocationGroup.Name)}
",
			new
			{
				id = locationGroup.ID,
				name = locationGroup.Name
			});

			connection.Close();
		}

		public async Task<ulong> WriteNewLocationGroup(string name)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var ID = await connection.ExecuteScalarAsync<ulong>($@"
INSERT INTO {nameof(LocationGroup)} ( {nameof(AuthorGroup.Name)} )
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
