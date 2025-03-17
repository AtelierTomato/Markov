namespace AtelierTomato.Markov.Model
{
	public class AuthorGroupPermission(ulong ID, AuthorOID author, AuthorGroupPermissionType permissions)
	{
		public ulong ID { get; init; } = ID;
		public AuthorOID Author { get; init; } = author;
		public AuthorGroupPermissionType Permissions { get; set; } = permissions;
	}
}
