using AtelierTomato.Markov.Data.Model;

namespace AtelierTomato.Markov.Data
{
	public class SentenceFilter(IObjectOID? OID, AuthorOID? author, string? searchString)
	{
		public IObjectOID? OID { get; set; } = OID;
		public AuthorOID? Author { get; set; } = author;
		public string? SearchString { get; set; } = searchString;
	}
}
