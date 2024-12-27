using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Storage.Sqlite.Model;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Storage.Sqlite
{
	public class SqliteLocationSettingAccess : ILocationSettingAccess
	{
		private readonly SqliteAccessOptions options;
		private readonly MultiParser<IObjectOID> objectOIDParser;
		public SqliteLocationSettingAccess(IOptions<SqliteAccessOptions> options, MultiParser<IObjectOID> objectOIDParser)
		{
			this.options = options.Value;
			this.objectOIDParser = objectOIDParser;
		}

		public async Task DeleteLocationSetting(IObjectOID ID)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			await connection.ExecuteAsync($@"DELETE FROM {nameof(LocationSetting)} WHERE {nameof(LocationSetting.ID)} IS @id",
				new
				{
					id = ID.ToString()
				});

			connection.Close();
		}

		public async Task<IEnumerable<LocationSetting>> ReadAllLocationSettings()
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<LocationSettingRow>($@"
SELECT {nameof(LocationSetting.ID)}, {nameof(LocationSetting.WriteReactions)}, {nameof(LocationSetting.DeleteReactions)}, {nameof(LocationSetting.FailReactions)}, {nameof(LocationSetting.GlobalAllowed)}, {nameof(LocationSetting.LocationGroup)}
FROM {nameof(LocationSetting)}
			");

			connection.Close();

			return result.Select(l => l.ToLocationSetting(objectOIDParser));
		}

		public async Task<LocationSetting?> ReadLocationSetting(IObjectOID ID) => (await ReadLocationSettingHierarchy(ID)).FirstOrDefault();
		public async Task<IEnumerable<LocationSetting>> ReadLocationSettingHierarchy(IObjectOID ID)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<LocationSettingRow>($@"
SELECT {nameof(LocationSetting.ID)}, {nameof(LocationSetting.WriteReactions)}, {nameof(LocationSetting.DeleteReactions)}, {nameof(LocationSetting.FailReactions)}, {nameof(LocationSetting.GlobalAllowed)}, {nameof(LocationSetting.LocationGroup)}
FROM {nameof(LocationSetting)}
WHERE @id || ':' LIKE {nameof(LocationSetting.ID)} || ':%'
ORDER BY LENGTH ({nameof(LocationSetting.ID)}) DESC
",
			new
			{
				id = ID.ToString()
			});

			connection.Close();

			return result.Select(l => l.ToLocationSetting(objectOIDParser));
		}

		public async Task<IEnumerable<LocationSetting>> ReadLocationSettingRangeByBaseLocation(IObjectOID ID)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<LocationSettingRow>($@"
SELECT {nameof(LocationSetting.ID)}, {nameof(LocationSetting.WriteReactions)}, {nameof(LocationSetting.DeleteReactions)}, {nameof(LocationSetting.FailReactions)}, {nameof(LocationSetting.GlobalAllowed)}, {nameof(LocationSetting.LocationGroup)}
FROM {nameof(LocationSetting)}
WHERE {nameof(LocationSetting.ID)} || ':' LIKE @id || ':%'
ORDER BY LENGTH({nameof(LocationSetting.ID)}) ASC
",
			new
			{
				id = ID.ToString()
			});

			connection.Close();

			return result.Select(l => l.ToLocationSetting(objectOIDParser));

		}

		public async Task WriteLocationSetting(LocationSetting locationSetting) => await WriteLocationSettingRange([locationSetting]);
		public async Task WriteLocationSettingRange(IEnumerable<LocationSetting> locationSettings)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();
			await using var transaction = await connection.BeginTransactionAsync();

			foreach (var locationSetting in locationSettings)
			{
				LocationSettingRow locationSettingRow = new LocationSettingRow(locationSetting);
				await connection.ExecuteAsync($@"
INSERT INTO {nameof(LocationSetting)} ( {nameof(LocationSetting.ID)}, {nameof(LocationSetting.WriteReactions)}, {nameof(LocationSetting.DeleteReactions)}, {nameof(LocationSetting.FailReactions)}, {nameof(LocationSetting.GlobalAllowed)}, {nameof(LocationSetting.LocationGroup)} )
VALUES ( @id, @writeReactions, @deleteReactions, @failReactions, @globalAllowed, @locationGroup )
ON CONFLICT ( {nameof(LocationSetting.ID)} ) DO UPDATE SET
{nameof(LocationSetting.WriteReactions)} = excluded.{nameof(LocationSetting.WriteReactions)},
{nameof(LocationSetting.DeleteReactions)} = excluded.{nameof(LocationSetting.DeleteReactions)},
{nameof(LocationSetting.FailReactions)} = excluded.{nameof(LocationSetting.FailReactions)},
{nameof(LocationSetting.GlobalAllowed)} = excluded.{nameof(LocationSetting.GlobalAllowed)},
{nameof(LocationSetting.LocationGroup)} = excluded.{nameof(LocationSetting.LocationGroup)}
",
				new
				{
					id = locationSettingRow.ID,
					writeReactions = locationSettingRow.WriteReactions,
					deleteReactions = locationSettingRow.DeleteReactions,
					failReactions = locationSettingRow.FailReactions,
					globalAllowed = locationSettingRow.GlobalAllowed,
					locationGroup = locationSettingRow.LocationGroup,
				});
			}

			await transaction.CommitAsync();
		}
	}
}
