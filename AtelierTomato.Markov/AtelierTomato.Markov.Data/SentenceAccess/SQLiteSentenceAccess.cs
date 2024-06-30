using AtelierTomato.Markov.Data.Model;

namespace AtelierTomato.Markov.Data.SentenceAccess
{
	public class SQLiteSentenceAccess : ISentenceAccess
	{
		public Task DeleteSentenceRange(SentenceFilter filter)
		{
			throw new NotImplementedException();
		}

		public Task<Sentence?> ReadNextRandomSentence(List<string> prevList, List<string> previousIDs, SentenceFilter filter)
		{
			throw new NotImplementedException();
		}

		public Task<Sentence?> ReadRandomSentence(SentenceFilter filter)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<Sentence>?> ReadSentenceRange(SentenceFilter filter)
		{
			throw new NotImplementedException();
		}

		public async Task WriteSentence(Sentence sentence) => await WriteSentenceRange([sentence]);

		public async Task WriteSentenceRange(IEnumerable<Sentence> sentenceRange)
		{
			throw new NotImplementedException();
		}
	}
}
