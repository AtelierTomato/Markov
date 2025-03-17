using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage
{
	public interface IAuthorGroupPermissionAccess
	{
		Task<AuthorGroupPermission?> ReadAuthorGroupPermission(ulong ID, AuthorOID author);
		Task<IEnumerable<AuthorGroupPermission>> ReadAuthorGroupPermissionRangeByID(ulong ID);
		Task<IEnumerable<AuthorGroupPermission>> ReadAuthorGroupPermissionRangeByAuthor(AuthorOID author);
		Task WriteAuthorGroupPermission(AuthorGroupPermission authorGroupPermission);
		Task WriteAuthorGroupPermissionRange(IEnumerable<AuthorGroupPermission> authorGroupPermissions);
		Task DeleteAuthorFromAuthorGroup(ulong ID, AuthorOID author);
	}
}
