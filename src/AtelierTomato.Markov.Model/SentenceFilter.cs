namespace AtelierTomato.Markov.Model
{
	public class SentenceFilter(IReadOnlyList<IObjectOID> OIDs, IReadOnlyList<AuthorOID> authors)
	{
		public IReadOnlyList<IObjectOID> OIDs { get; init; } = OIDs;
		public IReadOnlyList<AuthorOID> Authors { get; init; } = authors;
	}
}
