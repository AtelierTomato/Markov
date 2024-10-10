using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Storage.Sqlite.Model;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Storage.Sqlite
{
	public class SqliteLocationGroupRequestAccess : ILocationGroupRequestAccess
	{
		private readonly SqliteAccessOptions options;
		private readonly MultiParser<IObjectOID> objectOIDParser;
		public SqliteLocationGroupRequestAccess(IOptions<SqliteAccessOptions> options, MultiParser<IObjectOID> objectOIDParser)
		{
			this.options = options.Value;
			this.objectOIDParser = objectOIDParser;
		}

		public async Task DeleteLocationGroupRequest(Guid ID, IObjectOID location)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			await connection.ExecuteAsync($@"DELETE FROM {nameof(LocationGroup)}Request WHERE {nameof(LocationGroupPermission.ID)} IS @id AND {nameof(LocationGroupPermission.Location)} IS @location",
				new
				{
					id = ID.ToString(),
					location = location.ToString()
				});

			connection.Close();
		}

		public async Task<LocationGroupPermission?> ReadLocationGroupRequest(Guid ID, IObjectOID location)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QuerySingleOrDefaultAsync<LocationGroupPermissionRow>($@"
SELECT {nameof(LocationGroupPermission.ID)}, {nameof(LocationGroupPermission.Location)}, {nameof(LocationGroupPermission.Permissions)} FROM {nameof(LocationGroup)}Request WHERE
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

		public async Task<IEnumerable<LocationGroupPermission>> ReadLocationGroupRequestRangeByLocation(IObjectOID location)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<LocationGroupPermissionRow>($@"
SELECT {nameof(LocationGroupPermission.ID)}, {nameof(LocationGroupPermission.Location)}, {nameof(LocationGroupPermission.Permissions)} FROM {nameof(LocationGroup)}Request
WHERE {nameof(LocationGroupPermission.Location)} IS @location
",
			new
			{
				location = location.ToString()
			});

			connection.Close();

			return result.Select(r => r.ToLocationGroupPermission(objectOIDParser));
		}

		public async Task<IEnumerable<LocationGroupPermission>> ReadLocationGroupRequestRangeByID(Guid ID)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<LocationGroupPermissionRow>($@"
SELECT {nameof(LocationGroupPermission.ID)}, {nameof(LocationGroupPermission.Location)}, {nameof(LocationGroupPermission.Permissions)} FROM {nameof(LocationGroup)}Request
WHERE {nameof(LocationGroupPermission.ID)} IS @id
",
			new
			{
				id = ID.ToString()
			});

			connection.Close();

			return result.Select(r => r.ToLocationGroupPermission(objectOIDParser));
		}

		public async Task<IEnumerable<LocationGroupPermission>> ReadLocationGroupRequestRangeByOwner(AuthorOID author)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<LocationGroupPermissionRow>($@"
SELECT {nameof(LocationGroup)}Request.{nameof(LocationGroupPermission.ID)}, {nameof(LocationGroup)}Request.{nameof(LocationGroupPermission.Location)}, {nameof(LocationGroup)}Request.{nameof(LocationGroupPermission.Permissions)} FROM {nameof(LocationGroup)}Request INNER JOIN {nameof(Location)}
ON {nameof(LocationGroup)}Request.{nameof(LocationGroupPermission.Location)} = {nameof(Location)}.{nameof(Location.ID)}
WHERE {nameof(Location)}.{nameof(Location.Owner)} IS @author
",
			new
			{
				author = author.ToString()
			});

			connection.Close();

			return result.Select(r => r.ToLocationGroupPermission(objectOIDParser));
		}

		public async Task WriteLocationGroupRequest(LocationGroupPermission locationGroupPermission) => await WriteLocationGroupRequestRange([locationGroupPermission]);

		public async Task WriteLocationGroupRequestRange(IEnumerable<LocationGroupPermission> locationGroupPermissions)
		{
			var locationGroupPermissionRows = locationGroupPermissions.Select(a => new LocationGroupPermissionRow(a));
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();
			await using var transaction = await connection.BeginTransactionAsync();
			foreach (LocationGroupPermissionRow locationGroupPermissionRow in locationGroupPermissionRows)
			{
				await connection.ExecuteAsync($@"
INSERT INTO {nameof(LocationGroup)}Request ( {nameof(LocationGroupPermission.ID)}, {nameof(LocationGroupPermission.Location)}, {nameof(LocationGroupPermission.Permissions)} )
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
