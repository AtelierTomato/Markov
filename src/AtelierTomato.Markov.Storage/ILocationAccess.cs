using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage
{
	public interface ILocationAccess
	{
		Task WriteLocation(Location location);
		Task WriteLocationRange(IEnumerable<Location> locations);
		Task<Location?> ReadLocation(IObjectOID ID);
		Task<IEnumerable<Location>> ReadLocationRange(IEnumerable<IObjectOID> IDs);
		Task<AuthorOID?> ReadLocationOwner(IObjectOID ID);
	}
}
