using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage
{
	public interface ILocationGroupRequestAccess
	{
		Task<LocationGroupPermission?> ReadLocationGroupRequest(ulong ID, IObjectOID location);
		Task<IEnumerable<LocationGroupPermission>> ReadLocationGroupRequestRangeByID(ulong ID);
		Task<IEnumerable<LocationGroupPermission>> ReadLocationGroupRequestRangeByLocation(IObjectOID location);
		Task<IEnumerable<LocationGroupPermission>> ReadLocationGroupRequestRangeByBaseLocation(IObjectOID location);
		Task<IEnumerable<LocationGroupPermission>> ReadLocationGroupRequestRangeByOwner(AuthorOID author);
		Task WriteLocationGroupRequest(LocationGroupPermission locationGroupPermission);
		Task WriteLocationGroupRequestRange(IEnumerable<LocationGroupPermission> locationGroupPermissions);
		Task DeleteLocationGroupRequest(ulong ID, IObjectOID location);
	}
}
