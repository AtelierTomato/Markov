using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Storage.Sqlite.Model;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Storage.Sqlite
{
	public class SqliteLocationAccess : ILocationAccess
	{
		private readonly SqliteAccessOptions options;
		private readonly MultiParser<IObjectOID> objectOIDParser;
		public SqliteLocationAccess(IOptions<SqliteAccessOptions> options, MultiParser<IObjectOID> objectOIDParser)
		{
			this.options = options.Value;
			this.objectOIDParser = objectOIDParser;
		}

		public async Task<Location?> ReadLocation(IObjectOID ID) => (await ReadLocationRange([ID])).FirstOrDefault();
		public async Task<IEnumerable<Location>> ReadLocationRange(IEnumerable<IObjectOID> IDs)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<LocationRow>($@"SELECT {nameof(Location.ID)}, {nameof(Location.Name)}, {nameof(Location.Owner)} FROM {nameof(Location)} WHERE {nameof(Location.ID)} IN @ids",
				new
				{
					ids = IDs.Select(i => i.ToString())
				});

			connection.Close();

			return result.Select(a => a.ToLocation(objectOIDParser));
		}

		public async Task WriteLocation(Location location) => await WriteLocationRange([location]);
		public async Task WriteLocationRange(IEnumerable<Location> locations)
		{
			var locationRows = locations.Select(a => new LocationRow(a));
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();
			await using var transaction = await connection.BeginTransactionAsync();

			foreach (LocationRow locationRow in locationRows)
			{
				await connection.ExecuteAsync($@"
INSERT INTO {nameof(Location)} ( {nameof(Location.ID)}, {nameof(Location.Name)}, {nameof(Location.Owner)} )
Values ( @id, @name, @owner )
ON CONFLICT ({nameof(Location.ID)}) DO UPDATE SET
{nameof(Location.Name)} = excluded.{nameof(Location.Name)}
{nameof(Location.Owner)} = excluded.{nameof(Location.Owner)}
",
				new
				{
					id = locationRow.ID,
					name = locationRow.Name,
					owner = locationRow.Owner
				});
			}

			await transaction.CommitAsync();
		}
	}
}
