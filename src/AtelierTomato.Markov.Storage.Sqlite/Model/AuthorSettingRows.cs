using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Storage.Sqlite.Model.AuthorSettingRowsSubtypes;

namespace AtelierTomato.Markov.Storage.Sqlite.Model
{
	public class AuthorSettingRows
	{
		public AuthorSettingRow AuthorSetting { get; set; }
		public IReadOnlyList<AuthorSettingAuthorFilterRow> AuthorSettingAuthorFilter { get; set; }
		public IReadOnlyList<AuthorSettingAuthorGroupRow> AuthorSettingAuthorGroup { get; set; }
		public IReadOnlyList<AuthorSettingLocationFilterRow> AuthorSettingLocationFilter { get; set; }
		public IReadOnlyList<AuthorSettingLocationGroupRow> AuthorSettingLocationGroup { get; set; }
		public AuthorSettingRows(AuthorSettingRow authorSetting, IReadOnlyList<AuthorSettingAuthorFilterRow> authorSettingAuthorFilter, IReadOnlyList<AuthorSettingAuthorGroupRow> authorSettingAuthorGroup, IReadOnlyList<AuthorSettingLocationFilterRow> authorSettingLocationFilter, IReadOnlyList<AuthorSettingLocationGroupRow> authorSettingLocationGroup)
		{
			bool AreAuthorsEqual =
				authorSettingAuthorFilter.All(a => a.Author == authorSetting.Author) &&
				authorSettingAuthorGroup.All(a => a.Author == authorSetting.Author) &&
				authorSettingLocationFilter.All(a => a.Author == authorSetting.Author) &&
				authorSettingLocationGroup.All(a => a.Author == authorSetting.Author);
			if (!AreAuthorsEqual)
				throw new ArgumentException($"One or more of the {nameof(AuthorSettingRow.Author)} fields does not match the others.", nameof(authorSetting));
			bool AreLocationsEqual =
				authorSettingAuthorFilter.All(a => a.Location == authorSetting.Location) &&
				authorSettingAuthorGroup.All(a => a.Location == authorSetting.Location) &&
				authorSettingLocationFilter.All(a => a.Location == authorSetting.Location) &&
				authorSettingLocationGroup.All(a => a.Location == authorSetting.Location);
			if (!AreLocationsEqual)
				throw new ArgumentException($"One or more of the {nameof(AuthorSettingRow.Location)} fields does not match the others.", nameof(authorSetting));

			AuthorSetting = authorSetting;
			AuthorSettingAuthorFilter = authorSettingAuthorFilter;
			AuthorSettingAuthorGroup = authorSettingAuthorGroup;
			AuthorSettingLocationFilter = authorSettingLocationFilter;
			AuthorSettingLocationGroup = authorSettingLocationGroup;
		}
		public AuthorSettingRows(AuthorSetting authorSetting)
		{
			var author = authorSetting.Author.ToString();
			var location = authorSetting.Location?.ToString() ?? string.Empty;
			AuthorSetting = new(
				author,
				location,
				authorSetting.DisplayOption.ToString(),
				authorSetting.Keyword,
				authorSetting.FirstWord
			);
			AuthorSettingAuthorFilter = authorSetting.Filter.Authors.Select(a => new AuthorSettingAuthorFilterRow(
				author,
				location,
				a.ToString()
			)).ToList();
			AuthorSettingAuthorGroup = authorSetting.AuthorGroups.Select(a => new AuthorSettingAuthorGroupRow(
				author,
				location,
				a.ToString()
			)).ToList();
			AuthorSettingLocationFilter = authorSetting.Filter.OIDs.Select(a => new AuthorSettingLocationFilterRow(
				author,
				location,
				a.ToString()
			)).ToList();
			AuthorSettingLocationGroup = authorSetting.LocationGroups.Select(a => new AuthorSettingLocationGroupRow(
				author,
				location,
				a.ToString()
			)).ToList();
		}
		public AuthorSetting ToAuthorSetting(MultiParser<IObjectOID> oidParser)
		{
			if (!Enum.TryParse<DisplayOptionType>(AuthorSetting.DisplayOption, out var displayOption))
			{
				throw new InvalidOperationException($"{AuthorSetting.DisplayOption} is not a valid type of {nameof(DisplayOptionType)}");
			}

			IObjectOID? location;
			if (string.IsNullOrEmpty(AuthorSetting.Location))
			{
				location = null;
			}
			else
			{
				location = oidParser.Parse(AuthorSetting.Location);
			}

			return new(
				AuthorOID.Parse(AuthorSetting.Author),
				location,
				AuthorSettingAuthorGroup.Select(a => Guid.Parse(a.AuthorGroup)),
				AuthorSettingLocationGroup.Select(a => Guid.Parse(a.LocationGroup)),
				displayOption,
				new SentenceFilter(
					AuthorSettingLocationFilter.Select(a => oidParser.Parse(a.LocationFilter)).ToList(),
					AuthorSettingAuthorFilter.Select(a => AuthorOID.Parse(a.AuthorFilter)).ToList()
				),
				AuthorSetting.Keyword,
				AuthorSetting.FirstWord
			);
		}
	}
}
