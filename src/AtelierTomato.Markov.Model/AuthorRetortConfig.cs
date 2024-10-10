namespace AtelierTomato.Markov.Model
{
	public record AuthorRetortConfig
	(
		AuthorOID Author,
		IObjectOID Location,
		DisplayOptionType DisplayOption,
		SentenceFilter Filter,
		Guid? AuthorGroup = null,
		Guid? LocationGroup = null,
		string? Keyword = null,
		string? FirstWord = null
	);
}
