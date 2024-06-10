using AtelierTomato.Markov.Data.Model;

namespace AtelierTomato.Markov.Data
{
	public interface ISentenceAccess
	{
		Task<Sentence?> ReadSentence();
		Task<Sentence?> ReadNextSentence(List<string> prevList, List<Guid> previousIDs);
	}
}
