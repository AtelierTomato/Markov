using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage.Sqlite.Model
{
	public class LocationGroupPermissionRow
	{
		public string ID { get; set; } = string.Empty;
		public string Location { get; set; } = string.Empty;
		public string Permissions { get; set; } = string.Empty;
		public LocationGroupPermissionRow() { }
		public LocationGroupPermissionRow(string ID, string location, string permissions)
		{
			this.ID = ID;
			Location = location;
			Permissions = permissions;
		}
		public LocationGroupPermissionRow(LocationGroupPermission locationGroupPermission)
		{
			ID = locationGroupPermission.ID.ToString();
			Location = locationGroupPermission.Location.ToString();
			Permissions = locationGroupPermission.Permissions.ToString();
		}
		public LocationGroupPermission ToLocationGroupPermission(MultiParser<IObjectOID> objectOIDParser)
		{
			if (!Enum.TryParse(Permissions, out LocationGroupPermissionType permissions))
				throw new InvalidOperationException($"One or more of listed permissions is invalid: {Permissions}");

			return new(
				Guid.Parse(ID),
				objectOIDParser.Parse(Location),
				permissions
			);
		}
	}
}
