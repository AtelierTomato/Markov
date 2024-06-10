using AtelierTomato.Markov.Data.Model;

namespace AtelierTomato.Markov.Data
{
	public interface ISentenceAccess
	{
		Task<Sentence?> ReadSentence(IFilterHandler filter);
		Task<Sentence?> ReadNextSentence(List<string> prevList, List<ulong> previousIDs, IFilterHandler filter);
	}
}
