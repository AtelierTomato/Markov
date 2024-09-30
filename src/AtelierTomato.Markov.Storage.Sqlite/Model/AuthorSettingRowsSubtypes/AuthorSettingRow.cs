namespace AtelierTomato.Markov.Storage.Sqlite.Model.AuthorSettingRowsSubtypes
{
	public record AuthorSettingRow
	(
		string Author, string Location, string DisplayOption, string? Keyword = null, string? FirstWord = null
	);
}
