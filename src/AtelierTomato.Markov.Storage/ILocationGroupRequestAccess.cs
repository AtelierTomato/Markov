using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage
{
	public interface ILocationGroupRequestAccess
	{
		Task<LocationGroupPermission?> ReadLocationGroupRequest(Guid ID, IObjectOID location);
		Task<IEnumerable<LocationGroupPermission>> ReadLocationGroupRequestRangeByID(Guid ID);
		Task<IEnumerable<LocationGroupPermission>> ReadLocationGroupRequestRangeByLocation(IObjectOID location);
		Task WriteLocationGroupRequest(LocationGroupPermission locationGroupPermission);
		Task WriteLocationGroupRequestRange(IEnumerable<LocationGroupPermission> locationGroupPermissions);
		Task DeleteLocationGroupRequest(Guid ID, IObjectOID location);
	}
}
