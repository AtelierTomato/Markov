using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage
{
	public interface IAuthorGroupAccess
	{
		Task<AuthorGroup?> ReadAuthorGroup(Guid ID);
		Task<IEnumerable<AuthorGroup>> ReadAuthorGroups(IEnumerable<Guid> IDs);
		Task WriteAuthorGroup(AuthorGroup authorGroup);
		Task DeleteAuthorGroup(Guid ID);
	}
}
