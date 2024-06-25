using AtelierTomato.Markov.Data.Model;

namespace AtelierTomato.Markov.Data
{
	public interface ISentenceAccess
	{
		Task<Sentence?> ReadSentence(IMarkovFilter filter);
		Task<Sentence?> ReadNextSentence(List<string> prevList, List<string> previousIDs, IMarkovFilter filter);
	}
}
