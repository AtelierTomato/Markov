using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage
{
	public interface ILocationGroupAccess
	{
		Task<LocationGroup?> ReadLocationGroup(Guid ID);
		Task<IEnumerable<LocationGroup>> ReadLocationGroups(IEnumerable<Guid> IDs);
		Task WriteLocationGroup(LocationGroup locationGroup);
		Task DeleteLocationGroup(Guid ID);
	}
}
