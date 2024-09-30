namespace AtelierTomato.Markov.Model
{
	public record AuthorSetting
	(
		AuthorOID Author, IObjectOID? Location, IEnumerable<Guid> AuthorGroups, IEnumerable<Guid> LocationGroups, DisplayOptionType DisplayOption, SentenceFilter Filter, string? Keyword = null, string? FirstWord = null
	);
}
