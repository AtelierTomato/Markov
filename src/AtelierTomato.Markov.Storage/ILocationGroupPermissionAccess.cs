using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage
{
	public interface ILocationGroupPermissionAccess
	{
		Task<LocationGroupPermission?> ReadLocationGroupPermission(Guid ID, IObjectOID location);
		Task<IEnumerable<LocationGroupPermission>> ReadLocationGroupPermissionRangeByID(Guid ID);
		Task<IEnumerable<LocationGroupPermission>> ReadLocationGroupPermissionRangeByLocation(IObjectOID location);
		Task<LocationGroupPermissionType> ReadLocationGroupPermissionRangeByOwner(Guid ID, AuthorOID author);
		Task WriteLocationGroupPermission(LocationGroupPermission locationGroupPermission);
		Task WriteLocationGroupPermissionRange(IEnumerable<LocationGroupPermission> locationGroupPermissions);
		Task DeleteLocationFromLocationGroup(Guid ID, IObjectOID location);
	}
}
