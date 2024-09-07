using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage
{
	public interface IAuthorGroupPermissionAccess
	{
		Task<AuthorGroupPermission?> ReadAuthorGroupPermission(Guid ID, AuthorOID author);
		Task<IEnumerable<AuthorGroupPermission>> ReadAuthorGroupPermissionRangeByID(Guid ID);
		Task<IEnumerable<AuthorGroupPermission>> ReadAuthorGroupPermissionRangeByAuthor(AuthorOID author);
		Task WriteAuthorGroupPermission(AuthorGroupPermission authorGroupPermission);
		Task WriteAuthorGroupPermissionRange(IEnumerable<AuthorGroupPermission> authorGroupPermissions);
		Task DeleteAuthorFromAuthorGroup(Guid ID, AuthorOID author);
	}
}
