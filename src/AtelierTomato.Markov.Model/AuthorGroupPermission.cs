namespace AtelierTomato.Markov.Model
{
	public class AuthorGroupPermission(Guid ID, AuthorOID author, AuthorGroupPermissionType permissions)
	{
		public Guid ID { get; init; } = ID;
		public AuthorOID Author { get; init; } = author;
		public AuthorGroupPermissionType Permissions { get; set; } = permissions;
	}
}
