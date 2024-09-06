using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage
{
	public interface IAuthorGroupAccess
	{
		Task<AuthorGroup?> ReadAuthorGroup(Guid ID);
		Task WriteAuthorGroup(AuthorGroup authorGroup);
		Task DeleteAuthorGroup(Guid ID);
	}
}
