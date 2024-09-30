using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage.Sqlite.Model
{
	public class AuthorPermissionRow
	{
		public string Author { get; set; }
		public string QueryScope { get; set; }
		public string? AllowedScope { get; set; }
		public AuthorPermissionRow(string author, string queryScope, string? allowedScope)
		{
			Author = author;
			QueryScope = queryScope;
			AllowedScope = allowedScope;
		}
		public AuthorPermissionRow(AuthorPermission authorPermission)
		{
			Author = authorPermission.Author.ToString();
			QueryScope = authorPermission.QueryScope?.ToString() ?? string.Empty;
			AllowedScope = authorPermission.AllowedScope?.ToString();
		}
		public AuthorPermission ToAuthorPermission(MultiParser<IObjectOID> objectOIDParser)
		{
			IObjectOID? queryScope;
			if (string.IsNullOrEmpty(QueryScope))
			{
				queryScope = null;
			}
			else
			{
				queryScope = objectOIDParser.Parse(QueryScope);
			}
			return new(AuthorOID.Parse(Author), queryScope, AllowedScope switch
			{
				null => null,
				_ => objectOIDParser.Parse(AllowedScope)
			});
		}
	}
}
