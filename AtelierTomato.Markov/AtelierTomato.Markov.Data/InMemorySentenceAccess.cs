using AtelierTomato.Markov.Data.Model;

namespace AtelierTomato.Markov.Data
{
	public class InMemorySentenceAccess : ISentenceAccess
	{
		private readonly Random random = new Random();
		public List<Sentence> SentenceRange = [];
		public Task DeleteSentenceRange(SentenceFilter filter)
		{
			SentenceRange.RemoveAll(s =>
				(filter.OID is null || s.OID.ToString().StartsWith(filter.OID.ToString())) &&
				(filter.Author is null || s.Author.ToString() == filter.Author.ToString()) &&
				(filter.SearchString is null || s.Text.Contains(filter.SearchString)));
			return Task.CompletedTask;
		}

		public async Task<Sentence?> ReadNextRandomSentence(List<string> prevList, List<string> previousIDs, SentenceFilter filter)
		{
			List<Sentence> sentenceRange = ReadSentenceRange(filter).Result.Where(s => s.Text.Contains(string.Join(' ', prevList))).Where(s => !previousIDs.Contains(s.OID.ToString())).ToList();
			if (sentenceRange.Count == 0)
			{
				sentenceRange = ReadSentenceRange(new SentenceFilter(filter.OID, filter.Author, null)).Result.Where(s => s.Text.Contains(string.Join(' ', prevList))).Where(s => !previousIDs.Contains(s.OID.ToString())).ToList();
			}
			if (sentenceRange.Count == 0)
			{
				return null;
			}
			return sentenceRange[random.Next(sentenceRange.Count)];
		}

		public async Task<Sentence?> ReadRandomSentence(SentenceFilter filter)
		{
			List<Sentence> sentenceRange = ReadSentenceRange(filter).Result.ToList();
			if (sentenceRange.Count == 0)
			{
				sentenceRange = ReadSentenceRange(new SentenceFilter(filter.OID, filter.Author, null)).Result.ToList();
			}
			if (sentenceRange.Count == 0)
			{
				return null;
			}
			return sentenceRange[random.Next(sentenceRange.Count)];
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
