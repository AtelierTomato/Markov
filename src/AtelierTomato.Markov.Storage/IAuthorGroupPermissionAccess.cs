using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage
{
	public interface IAuthorGroupPermissionAccess
	{
		Task<AuthorGroupPermission?> ReadAuthorGroupPermission(string ID, AuthorOID author);
		Task<IEnumerable<AuthorGroupPermission>> ReadAuthorGroupPermissionRangeByID(string ID);
		Task<IEnumerable<AuthorGroupPermission>> ReadAuthorGroupPermissionRangeByAuthor(AuthorOID author);
		Task WriteAuthorGroupPermission(AuthorGroupPermission authorGroupPermission);
		Task WriteAuthorGroupPermissionRange(IEnumerable<AuthorGroupPermission> authorGroupPermissions);
		Task DeleteAuthorFromAuthorGroup(string ID, AuthorOID author);
		Task DeleteAuthorGroup(string ID);
	}
}
