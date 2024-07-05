using AtelierTomato.Markov.Data.Model;

namespace AtelierTomato.Markov.Data
{
	public interface ISentenceAccess
	{
		Task<Sentence?> ReadSentence(SentenceFilter filter);
		Task<Sentence?> ReadNextSentence(List<string> prevList, List<string> previousIDs, SentenceFilter filter);
	}
}
