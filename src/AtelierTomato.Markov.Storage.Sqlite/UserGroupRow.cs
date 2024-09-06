using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage.Sqlite
{
	public class UserGroupRow
	{
		public string ID { get; set; }
		public string Author { get; set; }
		public string Permissions { get; set; }
		public UserGroupRow(string ID, string author, string permissions)
		{
			this.ID = ID;
			Author = author;
			Permissions = permissions;
		}
		public UserGroupRow(UserGroup userGroup)
		{
			ID = userGroup.ID;
			Author = userGroup.Author.ToString();
			Permissions = string.Join(' ', userGroup.Permissions.Distinct().Select(p => p.ToString()));
		}
		public UserGroup ToUserGroup() => new(
			ID,
			AuthorOID.Parse(Author),
			Permissions.Split(' ')
					   .Distinct()
					   .Select(p =>
					   {
						   if (Enum.TryParse(p, out UserGroupPermissionType permission))
							   return permission;
						   throw new InvalidOperationException($"Invalid permission type: {p}");
					   })
		);
	}
}
