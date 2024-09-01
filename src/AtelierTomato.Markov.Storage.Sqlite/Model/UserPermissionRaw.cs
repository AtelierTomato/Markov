using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Model.ObjectOID.Parser;

namespace AtelierTomato.Markov.Storage.Sqlite.Model
{
	public class UserPermissionRaw
	{
		public string Author { get; set; }
		public string OriginScope { get; set; }
		public string AllowedScope { get; set; }
		private readonly MultiParser<IObjectOID> ObjectOIDParser = new([new InvalidObjectOIDParser(), new BookObjectOIDParser(), new DiscordObjectOIDParser()]);
		public UserPermissionRaw(string author, string originScope, string allowedScope)
		{
			Author = author;
			OriginScope = originScope;
			AllowedScope = allowedScope;
		}
		public UserPermissionRaw(UserPermission userPermission)
		{
			Author = userPermission.Author.ToString();
			OriginScope = userPermission.OriginScope.ToString();
			AllowedScope = userPermission.AllowedScope.ToString();
		}
		public UserPermission ToUserPermission() => new UserPermission(AuthorOID.Parse(Author), ObjectOIDParser.Parse(OriginScope), ObjectOIDParser.Parse(AllowedScope));
	}
}
