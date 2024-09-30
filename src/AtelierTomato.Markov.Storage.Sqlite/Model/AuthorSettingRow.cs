using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage.Sqlite.Model
{
	public class AuthorSettingRow
	{
		public string Author { get; set; }
		public string Location { get; set; }
		public string DisplayOption { get; set; }
		public string FilterOIDs { get; set; }
		public string FilterAuthors { get; set; }
		public string? AuthorGroup { get; set; }
		public string? LocationGroup { get; set; }
		public string? Keyword { get; set; }
		public string? FirstWord { get; set; }
		public AuthorSettingRow(string author, string location, string displayOption, string filterOIDs, string filterAuthors, string? authorGroup = null, string? locationGroup = null, string? keyword = null, string? firstWord = null)
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
		public AuthorSettingRow(AuthorSetting authorSetting)
		{
			Author = authorSetting.Author.ToString();
			Location = authorSetting.Location?.ToString() ?? string.Empty;
			DisplayOption = authorSetting.DisplayOption.ToString();
			FilterOIDs = string.Join("::", authorSetting.Filter.OIDs);
			FilterAuthors = string.Join("::", authorSetting.Filter.Authors);
			AuthorGroup = authorSetting.AuthorGroup?.ToString();
			LocationGroup = authorSetting.LocationGroup?.ToString();
			Keyword = authorSetting.Keyword;
			FirstWord = authorSetting.FirstWord;
		}
		public AuthorSetting ToAuthorSetting(MultiParser<IObjectOID> objectOIDParser)
		{
			// LOCATION
			IObjectOID? location;
			if (string.IsNullOrEmpty(Location))
			{
				location = null;
			}
			else
			{
				location = objectOIDParser.Parse(Location);
			}

			// DISPLAY OPTION
			if (!Enum.TryParse<DisplayOptionType>(DisplayOption, out var displayOption))
			{
				throw new InvalidOperationException($"{DisplayOption} is not a valid type of {nameof(DisplayOptionType)}");
			}

			// AUTHOR GROUP
			Guid? authorGroup;
			if (string.IsNullOrEmpty(AuthorGroup))
			{
				authorGroup = null;
			}
			else if (!Guid.TryParse(AuthorGroup, out Guid guid))
			{
				throw new InvalidOperationException($"{AuthorGroup} is not a valid {nameof(Guid)} value.");
			}
			else
			{
				authorGroup = guid;
			}

			// LOCATION GROUP
			Guid? locationGroup;
			if (string.IsNullOrEmpty(LocationGroup))
			{
				locationGroup = null;
			}
			else if (!Guid.TryParse(LocationGroup, out Guid guid))
			{
				throw new InvalidOperationException($"{LocationGroup} is not a valid {nameof(Guid)} value.");
			}
			else
			{
				locationGroup = guid;
			}

			// RETURN
			return new AuthorSetting(
				AuthorOID.Parse(Author),
				location,
				displayOption,
				new SentenceFilter(
					FilterOIDs.Split("::").Select(objectOIDParser.Parse).ToList(),
					FilterAuthors.Split("::").Select(AuthorOID.Parse).ToList()
				),
				authorGroup,
				locationGroup,
				Keyword,
				FirstWord
			);
		}
	}
}
