using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage.Sqlite.Model
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
			Permissions = authorGroupPermission.Permissions.ToString();
		}
		public AuthorGroupPermission ToAuthorGroupPermission()
		{
			if (!Enum.TryParse(Permissions, out AuthorGroupPermissionType permissions))
				throw new InvalidOperationException($"One or more of listed permissions is invalid: {Permissions}");

			return new(
				Guid.Parse(ID),
				AuthorOID.Parse(Author),
				permissions
			);
		}
	}
}
