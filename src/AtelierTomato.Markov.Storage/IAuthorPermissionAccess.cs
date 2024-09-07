using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage
{
	public interface IAuthorPermissionAccess
	{
		Task<IEnumerable<AuthorPermission>> ReadAllAuthorPermissions();
		Task<AuthorPermission?> ReadAuthorPermission(AuthorOID author, IObjectOID queryScope);
		Task<IEnumerable<AuthorPermission>> ReadAuthorPermissionRange(IEnumerable<AuthorOID> authors, IEnumerable<IObjectOID> queryScopes);
		Task WriteAuthorPermission(AuthorPermission authorPermission);
		Task WriteAuthorPermissionRange(IEnumerable<AuthorPermission> authorPermissions);
	}
}
