namespace AtelierTomato.Markov.Model
{
	public class SentenceFilter(IEnumerable<IObjectOID> OIDs, IEnumerable<AuthorOID> authors)
	{
		public IEnumerable<IObjectOID> OIDs { get; init; } = OIDs;
		public IEnumerable<AuthorOID> Authors { get; init; } = authors;
	}
}
