using AtelierTomato.Markov.Data.Model;

namespace AtelierTomato.Markov.Data
{
	public interface ISentenceAccess
	{
		Task<Sentence?> ReadSentence(ISentenceFilter filter);
		Task<Sentence?> ReadNextSentence(List<string> prevList, List<string> previousIDs, ISentenceFilter filter);
	}
}
