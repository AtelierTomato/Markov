using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage
{
	public interface ILocationGroupAccess
	{
		Task<LocationGroup?> ReadLocationGroup(ulong ID);
		Task<IEnumerable<LocationGroup>> ReadLocationGroups(IEnumerable<ulong> IDs);
		Task<ulong> WriteNewLocationGroup(string name);
		Task WriteLocationGroup(LocationGroup locationGroup);
		Task DeleteLocationGroup(ulong ID);
	}
}
