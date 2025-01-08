using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Core.TableFormatters
{
	public class FormattingUtils
	{
		public const string DefaultAuthor = "Unknown Author";
		public const string DefaultLocation = "Unknown Location";
		public const string NullLocation = "Global";
		public const string RepeatEntryReplacement = "|";
		public static int GetAuthorLength(IEnumerable<AuthorOID> authorOIDs, IEnumerable<Author> authors) => authorOIDs
			.Select(ai => ai.ToString())
			.Concat(authors.Where(ai => authorOIDs.Contains(ai.ID)).Select(ai => ai.Name))
			.Concat(authorOIDs.Any(ai => !authors.Any(a => a.ID == ai))
				? [DefaultAuthor] : [])
			.Select(s => s.Length)
			.Max();

		public static int GetLocationLength(IEnumerable<IObjectOID?> locationOIDs, IEnumerable<Location> locations) => locationOIDs
			.Select(li => li?.ToString() ?? NullLocation)
			.Concat(locations.Where(l => locationOIDs.Contains(l.ID)).Select(l => l.Name))
			.Concat(locationOIDs.Any(li => !locations.Any(l => l.ID == li))
				? [DefaultLocation] : [])
			.Select(s => s.Length)
			.Max();
		public static string GetAuthorNameFromID(AuthorOID authorOID, IEnumerable<Author> authors) =>
			authors.Where(a => a.ID == authorOID).FirstOrDefault()?.Name ?? DefaultAuthor;
		public static string GetLocationNameFromID(IObjectOID? locationOID, IEnumerable<Location> locations) => locationOID == null
			? NullLocation
			: (locations.Where(l => l.ID == locationOID).FirstOrDefault()?.Name ?? DefaultLocation);
	}
}
