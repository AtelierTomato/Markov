using AtelierTomato.Markov.Data.Model;

namespace AtelierTomato.Markov.Data
{
	public interface IMarkovFilter
	{
		AuthorOID? Author { get; set; }
		IObjectOID? OID { get; set; }
		string? Keyword { get; set; }
	}
}
