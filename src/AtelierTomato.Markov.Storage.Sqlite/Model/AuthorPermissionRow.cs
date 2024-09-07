using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Model.ObjectOID.Parser;

namespace AtelierTomato.Markov.Storage.Sqlite.Model
{
	public class AuthorPermissionRow
	{
		public string Author { get; set; }
		public string QueryScope { get; set; }
		public string AllowedScope { get; set; }
		private readonly MultiParser<IObjectOID> ObjectOIDParser = new([new SpecialObjectOIDParser(), new BookObjectOIDParser(), new DiscordObjectOIDParser()]);
		public AuthorPermissionRow(string author, string queryScope, string allowedScope)
		{
			Author = author;
			QueryScope = queryScope;
			AllowedScope = allowedScope;
		}
		public AuthorPermissionRow(AuthorPermission authorPermission)
		{
			Author = authorPermission.Author.ToString();
			QueryScope = authorPermission.QueryScope.ToString();
			AllowedScope = authorPermission.AllowedScope.ToString();
		}
		public AuthorPermission ToAuthorPermission() => new AuthorPermission(AuthorOID.Parse(Author), ObjectOIDParser.Parse(QueryScope), ObjectOIDParser.Parse(AllowedScope));
	}
}
