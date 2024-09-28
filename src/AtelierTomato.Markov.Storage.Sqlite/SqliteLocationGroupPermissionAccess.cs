using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Storage.Sqlite.Model;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Storage.Sqlite
{
	public class SqliteLocationGroupPermissionAccess : ILocationGroupPermissionAccess
	{
		private readonly SqliteAccessOptions options;
		private readonly MultiParser<IObjectOID> objectOIDParser;
		public SqliteLocationGroupPermissionAccess(IOptions<SqliteAccessOptions> options, MultiParser<IObjectOID> objectOIDParser)
		{
			this.options = options.Value;
			this.objectOIDParser = objectOIDParser;
		}

		public async Task DeleteLocationFromLocationGroup(Guid ID, IObjectOID location)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			await connection.ExecuteAsync($@"DELETE FROM {nameof(LocationGroupPermission)} WHERE {nameof(LocationGroupPermission.ID)} IS @id AND {nameof(LocationGroupPermission.Location)} IS @location",
				new
				{
					id = ID.ToString(),
					location = location.ToString()
				});

			connection.Close();
		}

		public async Task<LocationGroupPermission?> ReadLocationGroupPermission(Guid ID, IObjectOID location)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QuerySingleOrDefaultAsync<LocationGroupPermissionRow>($@"
SELECT {nameof(LocationGroupPermission.ID)}, {nameof(LocationGroupPermission.Location)}, {nameof(LocationGroupPermission.Permissions)} FROM {nameof(LocationGroupPermission)} WHERE
{nameof(LocationGroupPermission.ID)} IS @id AND
{nameof(LocationGroupPermission.Location)} IS @location
",
			new
			{
				id = ID.ToString(),
				location = location.ToString()
			});

			connection.Close();

			return result?.ToLocationGroupPermission(objectOIDParser);
		}

		public async Task<IEnumerable<LocationGroupPermission>> ReadLocationGroupPermissionRangeByLocation(IObjectOID location)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<LocationGroupPermissionRow>($@"
SELECT {nameof(LocationGroupPermission.ID)}, {nameof(LocationGroupPermission.Location)}, {nameof(LocationGroupPermission.Permissions)} FROM {nameof(LocationGroupPermission)}
WHERE {nameof(LocationGroupPermission.Location)} IS @location
",
			new
			{
				location = location.ToString()
			});

			connection.Close();

			return result.Select(u => u.ToLocationGroupPermission(objectOIDParser));
		}

		public async Task<IEnumerable<LocationGroupPermission>> ReadLocationGroupPermissionRangeByID(Guid ID)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<LocationGroupPermissionRow>($@"
SELECT {nameof(LocationGroupPermission.ID)}, {nameof(LocationGroupPermission.Location)}, {nameof(LocationGroupPermission.Permissions)} FROM {nameof(LocationGroupPermission)}
WHERE {nameof(LocationGroupPermission.ID)} IS @id
",
			new
			{
				id = ID.ToString()
			});

			connection.Close();

			return result.Select(u => u.ToLocationGroupPermission(objectOIDParser));
		}

		public async Task WriteLocationGroupPermission(LocationGroupPermission locationGroupPermission) => await WriteLocationGroupPermissionRange([locationGroupPermission]);

		public async Task WriteLocationGroupPermissionRange(IEnumerable<LocationGroupPermission> locationGroupPermissions)
		{
			var locationGroupPermissionRows = locationGroupPermissions.Select(a => new LocationGroupPermissionRow(a));
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();
			await using var transaction = await connection.BeginTransactionAsync();
			foreach (LocationGroupPermissionRow locationGroupPermissionRow in locationGroupPermissionRows)
			{
				await connection.ExecuteAsync($@"
INSERT INTO {nameof(LocationGroupPermission)} ( {nameof(LocationGroupPermission.ID)}, {nameof(LocationGroupPermission.Location)}, {nameof(LocationGroupPermission.Permissions)} )
Values ( @id, @location, @permissions )
ON CONFLICT ({nameof(LocationGroupPermission.ID)}, {nameof(LocationGroupPermission.Location)}) DO UPDATE SET
{nameof(LocationGroupPermission.Permissions)} = excluded.{nameof(LocationGroupPermission.Permissions)}
",
				new
				{
					id = locationGroupPermissionRow.ID,
					location = locationGroupPermissionRow.Location,
					permissions = locationGroupPermissionRow.Permissions
				});
			}

			await transaction.CommitAsync();
		}
	}
}
