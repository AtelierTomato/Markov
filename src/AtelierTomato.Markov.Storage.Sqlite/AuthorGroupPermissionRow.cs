using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage.Sqlite
{
	public class AuthorGroupPermissionRow
	{
		public string ID { get; set; }
		public string Author { get; set; }
		public string Permissions { get; set; }
		public AuthorGroupPermissionRow(string ID, string author, string permissions)
		{
			this.ID = ID;
			Author = author;
			Permissions = permissions;
		}
		public AuthorGroupPermissionRow(AuthorGroupPermission authorGroupPermission)
		{
			ID = authorGroupPermission.ID.ToString();
			Author = authorGroupPermission.Author.ToString();
			Permissions = string.Join(' ', authorGroupPermission.Permissions.Distinct().Select(p => p.ToString()));
		}
		public AuthorGroupPermission ToAuthorGroupPermission() => new(
			Guid.Parse(ID),
			AuthorOID.Parse(Author),
			Permissions.Split(' ')
					   .Distinct()
					   .Select(p =>
					   {
						   if (Enum.TryParse(p, out AuthorGroupPermissionType permission))
							   return permission;
						   throw new InvalidOperationException($"Invalid permission type: {p}");
					   })
		);
	}
}
