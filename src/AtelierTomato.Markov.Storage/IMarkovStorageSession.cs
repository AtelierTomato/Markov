using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage
{
	public interface IMarkovStorageSession : IDisposable
	{
		Task CreateTempTable(SentenceFilter filter, IObjectOID? queryScope = null);
		Task<Sentence?> ReadRandomSentence(string? keyword = null);
		Task<IEnumerable<Sentence>> ReadNextRandomSentences(int amount, List<string> prevList, List<IObjectOID> previousIDs, string? keyword = null);
	}
}
