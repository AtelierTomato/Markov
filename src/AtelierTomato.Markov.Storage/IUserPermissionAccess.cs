using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage
{
	public interface IUserPermissionAccess
	{
		Task<IEnumerable<UserPermission>> ReadAllUserPermissions();
		Task<UserPermission?> ReadUserPermission(AuthorOID author, IObjectOID originScope);
		Task<IEnumerable<UserPermission>> ReadUserPermissionRange(IEnumerable<AuthorOID> authors, IEnumerable<IObjectOID> originScopes);
		Task WriteUserPermission(UserPermission userPermission);
		Task WriteUserPermissionRange(IEnumerable<UserPermission> userPermissions);
	}
}
