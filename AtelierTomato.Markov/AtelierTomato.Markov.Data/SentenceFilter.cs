using AtelierTomato.Markov.Data.Model;

namespace AtelierTomato.Markov.Data
{
	public class SentenceFilter(IObjectOID? OID, AuthorOID? author, string? keyword)
	{
		public IObjectOID? OID { get; set; } = OID;
		public AuthorOID? Author { get; set; } = author;
		public string? Keyword { get; set; } = keyword;
	}
}
