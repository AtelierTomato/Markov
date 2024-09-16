using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage
{
	public interface IAuthorGroupRequestAccess
	{
		Task<AuthorGroupPermission?> ReadAuthorGroupRequest(Guid ID, AuthorOID author);
		Task<IEnumerable<AuthorGroupPermission>> ReadAuthorGroupRequestRangeByID(Guid ID);
		Task<IEnumerable<AuthorGroupPermission>> ReadAuthorGroupRequestRangeByAuthor(AuthorOID author);
		Task WriteAuthorGroupRequest(AuthorGroupPermission authorGroupPermission);
		Task WriteAuthorGroupRequestRange(IEnumerable<AuthorGroupPermission> authorGroupPermissions);
		Task DeleteAuthorGroupRequest(Guid ID, AuthorOID author);
	}
}
