using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage
{
	public interface IAuthorGroupAccess
	{
		Task<AuthorGroup?> ReadAuthorGroup(ulong ID);
		Task<IEnumerable<AuthorGroup>> ReadAuthorGroups(IEnumerable<ulong> IDs);
		Task<ulong> WriteNewAuthorGroup(string name);
		Task WriteAuthorGroup(AuthorGroup authorGroup);
		Task DeleteAuthorGroup(ulong ID);
	}
}
