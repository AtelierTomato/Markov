namespace AtelierTomato.Markov.Model
{
	public class LocationGroupPermission(Guid ID, IObjectOID location, LocationGroupPermissionType permissions)
	{
		public Guid ID { get; init; } = ID;
		public IObjectOID Location { get; init; } = location;
		public LocationGroupPermissionType Permissions { get; set; } = permissions;
	}
}
