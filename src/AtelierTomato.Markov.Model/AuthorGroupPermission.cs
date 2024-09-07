namespace AtelierTomato.Markov.Model
{
	public record AuthorGroupPermission
	(
		Guid ID, AuthorOID Author, AuthorGroupPermissionType Permissions
	);
}
