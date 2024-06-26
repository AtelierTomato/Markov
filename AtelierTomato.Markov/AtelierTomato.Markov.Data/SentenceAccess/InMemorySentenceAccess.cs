using AtelierTomato.Markov.Data.Model;

namespace AtelierTomato.Markov.Data.SentenceAccess
{
	public class InMemorySentenceAccess : ISentenceAccess
	{
		public List<Sentence> SentenceRange = [];
		public Task DeleteSentenceRange(SentenceFilter filter)
		{
			SentenceRange = SentenceRange.Where(s =>
				(filter.OID is null || (!s.OID.ToString().StartsWith(filter.OID.ToString()))) &&
				(filter.Author is null || s.Author.ToString() != filter.Author.ToString()) &&
				(filter.Keyword is null || !s.Text.Contains(filter.Keyword))
			).ToList();
			return Task.CompletedTask;
		}

		public Task<Sentence?> ReadNextRandomSentence(List<string> prevList, List<string> previousIDs, SentenceFilter filter)
		{
			throw new NotImplementedException();
		}

		public Task<Sentence?> ReadRandomSentence(SentenceFilter filter)
		{
			throw new NotImplementedException();
		}

		public async Task<IEnumerable<Sentence>?> ReadSentenceRange(SentenceFilter filter)
		{
			return SentenceRange.Where(s =>
				(filter.OID is null || s.OID.ToString().StartsWith(filter.OID.ToString())) &&
				(filter.Author is null || s.Author.ToString() == filter.Author.ToString()) &&
				(filter.Keyword is null || s.Text.Contains(filter.Keyword))
			).ToList();
		}

		public Task WriteSentence(Sentence sentence)
		{
			SentenceRange.Add(sentence);
			return Task.CompletedTask;
		}

		public Task WriteSentenceRange(IEnumerable<Sentence> sentenceRange)
		{
			foreach (Sentence sentence in sentenceRange)
			{
				WriteSentence(sentence);
			}
			return Task.CompletedTask;
		}
	}
}
