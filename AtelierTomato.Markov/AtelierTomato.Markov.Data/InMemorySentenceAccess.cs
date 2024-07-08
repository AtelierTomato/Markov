using AtelierTomato.Markov.Data.Model;

namespace AtelierTomato.Markov.Data
{
	public class InMemorySentenceAccess : ISentenceAccess
	{
		private readonly Random random = new Random();
		public List<Sentence> SentenceRange = [];
		public Task DeleteSentenceRange(SentenceFilter filter)
		{
			if (filter.OID is null && filter.Author is null && filter.SearchString is null)
				throw new ArgumentException("You cannot delete all sentences from the database through this command, at least one part of the filter must have a value.", nameof(filter));

			SentenceRange.RemoveAll(s =>
				(filter.OID is null || s.OID.ToString().StartsWith(filter.OID.ToString())) &&
				(filter.Author is null || s.Author.ToString() == filter.Author.ToString()) &&
				(filter.SearchString is null || s.Text.Contains(filter.SearchString)));
			return Task.CompletedTask;
		}

		public async Task<Sentence?> ReadNextRandomSentence(List<string> prevList, List<IObjectOID> previousIDs, SentenceFilter filter)
		{
			List<Sentence>? sentenceRange = ReadSentenceRange(filter).Result?.Where(s => s.Text.Contains(string.Join(' ', prevList))).Where(s => !previousIDs.Contains(s.OID)).ToList() ?? null;
			if (sentenceRange is null || sentenceRange.Count is 0)
			{
				sentenceRange = ReadSentenceRange(new SentenceFilter(filter.OID, filter.Author, null)).Result?.Where(s => s.Text.Contains(string.Join(' ', prevList))).Where(s => !previousIDs.Contains(s.OID)).ToList() ?? null;
			}
			if (sentenceRange is null || sentenceRange.Count is 0)
			{
				return null;
			}

			return sentenceRange[random.Next(sentenceRange.Count - 1)];
		}

		public async Task<Sentence?> ReadRandomSentence(SentenceFilter filter)
		{
			List<Sentence>? sentenceRange = ReadSentenceRange(filter).Result?.ToList() ?? null;
			if (sentenceRange is null || sentenceRange.Count is 0)
			{
				sentenceRange = ReadSentenceRange(new SentenceFilter(filter.OID, filter.Author, null)).Result?.ToList() ?? null;
			}
			if (sentenceRange is null || sentenceRange.Count is 0)
			{
				return null;
			}
			return sentenceRange[random.Next(sentenceRange.Count - 1)];
		}

		public async Task<IEnumerable<Sentence>?> ReadSentenceRange(SentenceFilter filter)
		{
			return SentenceRange.Where(s =>
				(filter.OID is null || s.OID.ToString().StartsWith(filter.OID.ToString())) &&
				(filter.Author is null || s.Author.ToString() == filter.Author.ToString()) &&
				(filter.SearchString is null || s.Text.Contains(filter.SearchString))
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
