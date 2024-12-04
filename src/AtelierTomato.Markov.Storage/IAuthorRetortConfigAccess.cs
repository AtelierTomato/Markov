using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage
{
	public interface IAuthorRetortConfigAccess
	{
		Task<IEnumerable<AuthorRetortConfig>> ReadAllAuthorRetortConfigs();
		Task<AuthorRetortConfig?> ReadAuthorRetortConfig(AuthorOID author, IObjectOID location);
		Task<IEnumerable<AuthorRetortConfig>> ReadAuthorRetortConfigRangeByAuthor(AuthorOID author);
		Task<IEnumerable<AuthorRetortConfig>> ReadAuthorRetortConfigRangeByLocation(IObjectOID location);
		Task WriteAuthorRetortConfig(AuthorRetortConfig authorRetortConfig);
		Task DeleteAuthorRetortConfig(AuthorOID author, IObjectOID location);
		Task DeleteAuthorRetortConfigRangeByAuthor(AuthorOID author);
		Task DeleteAuthorRetortConfigRangeByLocation(IObjectOID location);
	}
}
