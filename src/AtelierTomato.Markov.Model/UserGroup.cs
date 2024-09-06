namespace AtelierTomato.Markov.Model
{
	public record UserGroup
	(
		string ID, AuthorOID Author, List<UserGroupPermissionType> Permissions
	);
}
