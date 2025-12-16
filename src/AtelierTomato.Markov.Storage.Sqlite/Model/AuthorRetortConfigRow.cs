using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage.Sqlite.Model
{
	public class AuthorRetortConfigRow
	{
		public string Author { get; set; } = string.Empty;
		public string Location { get; set; } = string.Empty;
		public string DisplayOption { get; set; } = string.Empty;
		public string FilterOIDs { get; set; } = string.Empty;
		public string FilterAuthors { get; set; } = string.Empty;
		public ulong? AuthorGroup { get; set; }
		public ulong? LocationGroup { get; set; }
		public string? Keyword { get; set; }
		public string? FirstWord { get; set; }
		public AuthorRetortConfigRow() { }
		public AuthorRetortConfigRow(string author, string location, string displayOption, string filterOIDs, string filterAuthors, ulong? authorGroup = null, ulong? locationGroup = null, string? keyword = null, string? firstWord = null)
		{
			Author = author;
			Location = location;
			DisplayOption = displayOption;
			FilterOIDs = filterOIDs;
			FilterAuthors = filterAuthors;
			AuthorGroup = authorGroup;
			LocationGroup = locationGroup;
			Keyword = keyword;
			FirstWord = firstWord;
		}
		public AuthorRetortConfigRow(AuthorRetortConfig authorRetortConfig)
		{
			Author = authorRetortConfig.Author.ToString();
			Location = authorRetortConfig.Location.ToString();
			DisplayOption = authorRetortConfig.DisplayOption.ToString();
			FilterOIDs = string.Join(":::", authorRetortConfig.Filter.OIDs);
			FilterAuthors = string.Join(":::", authorRetortConfig.Filter.Authors);
			AuthorGroup = authorRetortConfig.AuthorGroup;
			LocationGroup = authorRetortConfig.LocationGroup;
			Keyword = authorRetortConfig.Keyword;
			FirstWord = authorRetortConfig.FirstWord;
		}
		public AuthorRetortConfig ToAuthorRetortConfig(MultiParser<IObjectOID> objectOIDParser)
		{
			// DISPLAY OPTION
			if (!Enum.TryParse<DisplayOptionType>(DisplayOption, out var displayOption))
			{
				throw new InvalidOperationException($"{DisplayOption} is not a valid type of {nameof(DisplayOptionType)}");
			}

			// RETURN
			return new AuthorRetortConfig(
				AuthorOID.Parse(Author),
				objectOIDParser.Parse(Location),
				displayOption,
				new SentenceFilter(
					FilterOIDs.Split(":::").Where(s => !string.IsNullOrEmpty(s)).Select(objectOIDParser.Parse).ToList(),
					FilterAuthors.Split(":::").Where(s => !string.IsNullOrEmpty(s)).Select(AuthorOID.Parse).ToList()
				),
				AuthorGroup,
				LocationGroup,
				Keyword,
				FirstWord
			);
		}
	}
}
