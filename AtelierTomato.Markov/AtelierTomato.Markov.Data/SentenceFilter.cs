using AtelierTomato.Markov.Data.Model;

namespace AtelierTomato.Markov.Data
{
	public class SentenceFilter(IObjectOID? OID, AuthorOID? author, string? searchString)
	{
		public IObjectOID? OID { get; init; } = OID;
		public AuthorOID? Author { get; init; } = author;
		public string? SearchString { get; init; } = searchString;
	}
}
