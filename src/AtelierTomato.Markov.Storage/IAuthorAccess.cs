using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage
{
	public interface IAuthorAccess
	{
		Task WriteAuthor(Author author);
		Task WriteAuthorRange(IEnumerable<Author> authors);
		Task<Author?> ReadAuthor(AuthorOID ID);
		Task<IEnumerable<Author>> ReadAuthorRange(IEnumerable<AuthorOID> IDs);
	}
}
