using AtelierTomato.Markov.Data.Model;

namespace AtelierTomato.Markov.Data
{
	public interface IMarkovFilter
	{
		AuthorOID? Author { get; set; }
		ServiceType? Service { get; set; }
		string? Instance { get; set; }
		string? Guild { get; set; }
		string? Channel { get; set; }
		string? Subchannel { get; set; }
		string? Keyword { get; set; }
	}
}
