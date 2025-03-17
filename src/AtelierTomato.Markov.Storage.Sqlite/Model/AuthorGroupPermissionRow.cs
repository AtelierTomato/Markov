using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage.Sqlite.Model
{
	public class AuthorGroupPermissionRow
	{
		public ulong ID { get; set; }
		public string Author { get; set; } = string.Empty;
		public string Permissions { get; set; } = string.Empty;
		public AuthorGroupPermissionRow() { }
		public AuthorGroupPermissionRow(ulong ID, string author, string permissions)
		{
			this.ID = ID;
			Author = author;
			Permissions = permissions;
		}
		public AuthorGroupPermissionRow(AuthorGroupPermission authorGroupPermission)
		{
			ID = authorGroupPermission.ID;
			Author = authorGroupPermission.Author.ToString();
			Permissions = authorGroupPermission.Permissions.ToString();
		}
		public AuthorGroupPermission ToAuthorGroupPermission()
		{
			if (!Enum.TryParse(Permissions, out AuthorGroupPermissionType permissions))
				throw new InvalidOperationException($"One or more of listed permissions is invalid: {Permissions}");

			return new(
				ID,
				AuthorOID.Parse(Author),
				permissions
			);
		}
	}
}
