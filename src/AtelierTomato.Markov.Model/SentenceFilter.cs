namespace AtelierTomato.Markov.Model
{
	public class SentenceFilter(IReadOnlyCollection<IObjectOID> OIDs, IReadOnlyCollection<AuthorOID> authors)
	{
		public IReadOnlyCollection<IObjectOID> OIDs { get; init; } = OIDs;
		public IReadOnlyCollection<AuthorOID> Authors { get; init; } = authors;
	}
}
