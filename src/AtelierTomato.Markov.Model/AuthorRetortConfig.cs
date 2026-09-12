namespace AtelierTomato.Markov.Model
{
	public record AuthorRetortConfig
	(
		AuthorOID Author,
		IObjectOID Location,
		DisplayOptionType DisplayOption,
		SentenceFilter Filter,
		ulong? AuthorGroup = null,
		ulong? LocationGroup = null,
		string? Keyword = null,
		string? FirstWord = null
	);
}
