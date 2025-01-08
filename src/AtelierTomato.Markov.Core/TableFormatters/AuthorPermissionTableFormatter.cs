using System.Text;
using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Storage;

namespace AtelierTomato.Markov.Core.TableFormatters
{
	public class AuthorPermissionTableFormatter
	{
		private const int ExtraPadding = 2;
		private const string AuthorColumn = "Authors";
		private const string QueryScopeColumn = "Query Scope";
		private const string AllowedScopeColumn = "Allowed Scope";
		private readonly IAuthorAccess authorAccess;
		private readonly ILocationAccess locationAccess;
		public AuthorPermissionTableFormatter(IAuthorAccess authorAccess, ILocationAccess locationAccess)
		{
			this.authorAccess = authorAccess;
			this.locationAccess = locationAccess;
		}

		public async Task<string> Format(List<AuthorPermission> authorPermissions, string header)
		{
			var authors = (await authorAccess.ReadAuthorRange(authorPermissions.Select(a => a.Author))).ToList();
			var locations = (await locationAccess.ReadLocationRange(authorPermissions.Select(a => a.QueryScope).Concat(authorPermissions.Select(a => a.AllowedScope)).Where(l => l is not null).Select(l => l!))).ToList();
			var authorLength = Math.Max(FormattingUtils.GetAuthorLength(authorPermissions.Select(ap => ap.Author), authors), AuthorColumn.Length) + ExtraPadding;
			var queryScopeLength = Math.Max(FormattingUtils.GetLocationLength(authorPermissions.Select(ap => ap.QueryScope), locations), QueryScopeColumn.Length) + ExtraPadding;
			var allowedScopeLength = Math.Max(FormattingUtils.GetLocationLength(authorPermissions.Select(ap => ap.AllowedScope), locations), AllowedScopeColumn.Length) + ExtraPadding;
			var sb = new StringBuilder();
			sb.AppendLine(header);
			sb.AppendLine();
			var columnNames = AuthorColumn.PadRight(authorLength) + QueryScopeColumn.PadRight(queryScopeLength) + AllowedScopeColumn.PadRight(allowedScopeLength);
			sb.AppendLine(columnNames);
			sb.AppendLine(new string('-', Math.Max(columnNames.Length, header.Length)));
			sb.AppendLine();
			AuthorPermission? lastAuthorPermission = null;
			authorPermissions = authorPermissions
				.OrderBy(ap => ap.QueryScope)
				.ThenBy(ap => ap.AllowedScope)
				.ThenBy(ap => ap.Author)
				.ToList();
			foreach (var authorPermission in authorPermissions)
			{
				string authorString, authorOIDString, queryScopeString, queryScopeOIDString, allowedScopeString, allowedScopeOIDString;
				if (lastAuthorPermission is not null && authorPermission.Author == lastAuthorPermission.Author)
				{
					int currentAuthorLength = Math.Max(
						FormattingUtils.GetAuthorNameFromID(lastAuthorPermission.Author, authors).Length,
						lastAuthorPermission.Author.ToString().Length
					);
					authorString = FormattingUtils.RepeatEntryReplacement.PadLeft(currentAuthorLength / 2).PadRight(authorLength);
					authorOIDString = FormattingUtils.RepeatEntryReplacement.PadLeft(currentAuthorLength / 2).PadRight(authorLength);
				}
				else
				{
					authorString = FormattingUtils.GetAuthorNameFromID(authorPermission.Author, authors).PadRight(authorLength);
					authorOIDString = authorPermission.Author.ToString().PadRight(authorLength);
				}
				if (lastAuthorPermission is not null && authorPermission.QueryScope == lastAuthorPermission.QueryScope)
				{
					int currentQueryScopeLength = Math.Max(
						FormattingUtils.GetLocationNameFromID(lastAuthorPermission.QueryScope, locations).Length,
						(lastAuthorPermission.QueryScope?.ToString() ?? FormattingUtils.NullLocation).Length
					);
					queryScopeString = FormattingUtils.RepeatEntryReplacement.PadLeft(currentQueryScopeLength / 2).PadRight(queryScopeLength);
					queryScopeOIDString = FormattingUtils.RepeatEntryReplacement.PadLeft(currentQueryScopeLength / 2).PadRight(queryScopeLength);
				}
				else
				{
					queryScopeString = FormattingUtils.GetLocationNameFromID(authorPermission.QueryScope, locations).PadRight(queryScopeLength);
					queryScopeOIDString = (authorPermission.QueryScope?.ToString() ?? FormattingUtils.RepeatEntryReplacement.PadLeft(FormattingUtils.NullLocation.Length / 2))
						.PadRight(queryScopeLength);
				}
				if (lastAuthorPermission is not null && authorPermission.AllowedScope == lastAuthorPermission.AllowedScope)
				{
					int currentAllowedScopeLength = Math.Max(
						FormattingUtils.GetLocationNameFromID(lastAuthorPermission.AllowedScope, locations).Length,
						(lastAuthorPermission.AllowedScope?.ToString() ?? FormattingUtils.NullLocation).Length
					);
					allowedScopeString = FormattingUtils.RepeatEntryReplacement.PadLeft(currentAllowedScopeLength / 2).PadRight(allowedScopeLength);
					allowedScopeOIDString = FormattingUtils.RepeatEntryReplacement.PadLeft(currentAllowedScopeLength / 2).PadRight(allowedScopeLength);
				}
				else
				{
					allowedScopeString = FormattingUtils.GetLocationNameFromID(authorPermission.AllowedScope, locations).PadRight(allowedScopeLength);
					allowedScopeOIDString = (authorPermission.AllowedScope?.ToString() ?? FormattingUtils.RepeatEntryReplacement.PadLeft(FormattingUtils.NullLocation.Length / 2))
						.PadRight(allowedScopeLength);
				}
				sb.AppendLine(authorString + queryScopeString + allowedScopeString);
				sb.AppendLine(authorOIDString + queryScopeOIDString + allowedScopeOIDString);
			}
			return sb.ToString();
		}
	}
}
