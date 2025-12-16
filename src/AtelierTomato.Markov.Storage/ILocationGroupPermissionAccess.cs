using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage
{
	public interface ILocationGroupPermissionAccess
	{
		Task<LocationGroupPermission?> ReadLocationGroupPermission(ulong ID, IObjectOID location);
		Task<IEnumerable<LocationGroupPermission>> ReadLocationGroupPermissionHierarchyForGroup(ulong ID, IObjectOID location);
		Task<IEnumerable<LocationGroupPermission>> ReadLocationGroupPermissionRangeByID(ulong ID);
		Task<IEnumerable<LocationGroupPermission>> ReadLocationGroupPermissionHierarchy(IObjectOID location);
		Task<IEnumerable<LocationGroupPermission>> ReadLocationGroupPermissionRangeByBaseLocation(IObjectOID location);
		Task<IEnumerable<LocationGroupPermission>> ReadLocationGroupPermissionRangeByOwner(AuthorOID author);
		Task<LocationGroupPermissionType> ReadLocationGroupPermissionsForOwner(ulong ID, AuthorOID author);
		Task WriteLocationGroupPermission(LocationGroupPermission locationGroupPermission);
		Task WriteLocationGroupPermissionRange(IEnumerable<LocationGroupPermission> locationGroupPermissions);
		Task DeleteLocationFromLocationGroup(ulong ID, IObjectOID location);
	}
}
