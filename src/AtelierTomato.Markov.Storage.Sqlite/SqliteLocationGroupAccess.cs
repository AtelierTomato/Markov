using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Storage.Sqlite.Model;
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

		public async Task DeleteLocationGroup(Guid ID)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			await connection.ExecuteAsync($@"DELETE FROM {nameof(LocationGroup)} WHERE {nameof(LocationGroup.ID)} IS @id",
				new
				{
					id = ID.ToString()
				});

			connection.Close();
		}

		public async Task<LocationGroup?> ReadLocationGroup(Guid ID)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QuerySingleOrDefaultAsync<LocationGroupRow>($@"
SELECT {nameof(LocationGroup.ID)}, {nameof(LocationGroup.Name)} FROM {nameof(LocationGroup)}
WHERE {nameof(LocationGroup.ID)} IS @id
",
			new
			{
				id = ID.ToString()
			});

			connection.Close();

			return result?.ToLocationGroup();
		}

		public async Task WriteLocationGroup(LocationGroup locationGroup)
		{
			var locationGroupRow = new LocationGroupRow(locationGroup);
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
				id = locationGroupRow.ID,
				name = locationGroupRow.Name
			});

			connection.Close();
		}
	}
}
