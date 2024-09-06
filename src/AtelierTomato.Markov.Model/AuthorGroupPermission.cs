namespace AtelierTomato.Markov.Model
{
	public record AuthorGroupPermission
	(
		string ID, AuthorOID Author, IEnumerable<AuthorGroupPermissionType> Permissions
	);
}
