using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Model.ObjectOID.Parser;

namespace AtelierTomato.Markov.Storage.Sqlite.Model
{
	public class AuthorPermissionRow
	{
		public string Author { get; set; }
		public string OriginScope { get; set; }
		public string AllowedScope { get; set; }
		private readonly MultiParser<IObjectOID> ObjectOIDParser = new([new SpecialObjectOIDParser(), new BookObjectOIDParser(), new DiscordObjectOIDParser()]);
		public AuthorPermissionRow(string author, string originScope, string allowedScope)
		{
			Author = author;
			OriginScope = originScope;
			AllowedScope = allowedScope;
		}
		public AuthorPermissionRow(AuthorPermission authorPermission)
		{
			Author = authorPermission.Author.ToString();
			OriginScope = authorPermission.OriginScope.ToString();
			AllowedScope = authorPermission.AllowedScope.ToString();
		}
		public AuthorPermission ToAuthorPermission() => new AuthorPermission(AuthorOID.Parse(Author), ObjectOIDParser.Parse(OriginScope), ObjectOIDParser.Parse(AllowedScope));
	}
}
