namespace AtelierTomato.Markov.Model
{
	public class LocationGroupPermission(ulong ID, IObjectOID location, LocationGroupPermissionType permissions)
	{
		public ulong ID { get; init; } = ID;
		public IObjectOID Location { get; init; } = location;
		public LocationGroupPermissionType Permissions { get; set; } = permissions;
	}
}
