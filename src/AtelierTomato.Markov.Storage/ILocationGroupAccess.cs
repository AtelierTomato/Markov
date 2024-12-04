using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage
{
	public interface ILocationGroupAccess
	{
		Task<LocationGroup?> ReadLocationGroup(Guid ID);
		Task WriteLocationGroup(LocationGroup locationGroup);
		Task DeleteLocationGroup(Guid ID);
	}
}
