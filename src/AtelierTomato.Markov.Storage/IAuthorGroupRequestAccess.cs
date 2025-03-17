using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage
{
	public interface IAuthorGroupRequestAccess
	{
		Task<AuthorGroupPermission?> ReadAuthorGroupRequest(ulong ID, AuthorOID author);
		Task<IEnumerable<AuthorGroupPermission>> ReadAuthorGroupRequestRangeByID(ulong ID);
		Task<IEnumerable<AuthorGroupPermission>> ReadAuthorGroupRequestRangeByAuthor(AuthorOID author);
		Task WriteAuthorGroupRequest(AuthorGroupPermission authorGroupPermission);
		Task WriteAuthorGroupRequestRange(IEnumerable<AuthorGroupPermission> authorGroupPermissions);
		Task DeleteAuthorGroupRequest(ulong ID, AuthorOID author);
	}
}
