namespace AtelierTomato.Markov.Model
{
	public record SentenceFilter(
		IEnumerable<IObjectOID> OIDs, IEnumerable<AuthorOID> Authors
	);
}
