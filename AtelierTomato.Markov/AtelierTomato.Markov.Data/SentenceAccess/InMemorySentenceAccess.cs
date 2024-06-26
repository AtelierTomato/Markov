using AtelierTomato.Markov.Data.Model;

namespace AtelierTomato.Markov.Data.SentenceAccess
{
	public class InMemorySentenceAccess : ISentenceAccess
	{
		public Dictionary<IObjectOID, Sentence> SentenceDictionary = [];
		public Task DeleteSentenceRange(ISentenceFilter filter)
		{
			throw new NotImplementedException();
		}

		public Task<Sentence?> ReadNextRandomSentence(List<string> prevList, List<string> previousIDs, ISentenceFilter filter)
		{
			throw new NotImplementedException();
		}

		public Task<Sentence?> ReadRandomSentence(ISentenceFilter filter)
		{
			throw new NotImplementedException();
		}

		public Task<Sentence?> ReadSentenceRange(ISentenceFilter filter)
		{
			throw new NotImplementedException();
		}

		public Task WriteSentence(Sentence sentence)
		{
			throw new NotImplementedException();
		}

		public Task WriteSentenceRange(IEnumerable<Sentence> sentenceRange)
		{
			throw new NotImplementedException();
		}
	}
}
