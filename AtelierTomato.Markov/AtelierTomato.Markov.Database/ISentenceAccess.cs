using AtelierTomato.Markov.Database.Model;

namespace AtelierTomato.Markov.Database
{
	public interface ISentenceAccess
	{
		Task<Sentence?> ReadSentence();
		Task<Sentence?> ReadNextSentence(List<string> prevList, List<ulong> previousIDs);
	}
}
