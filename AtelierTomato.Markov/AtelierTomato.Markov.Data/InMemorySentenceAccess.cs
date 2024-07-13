using AtelierTomato.Markov.Data.Model;

namespace AtelierTomato.Markov.Data
{
	public class InMemorySentenceAccess : ISentenceAccess
	{
		private readonly Random random = new Random();
		private readonly List<Sentence> sentenceStorage = [];
		public IReadOnlyList<Sentence> SentenceStorage { get => sentenceStorage; }
		public Task DeleteSentenceRange(SentenceFilter filter)
		{
			if (filter.OID is null && filter.Author is null && filter.SearchString is null)
				throw new ArgumentException("You cannot delete all sentences from the database through this command, at least one part of the filter must have a value.", nameof(filter));

			sentenceStorage.RemoveAll(s =>
				(filter.OID is null || s.OID.ToString().StartsWith(filter.OID.ToString())) &&
				(filter.Author is null || s.Author.ToString() == filter.Author.ToString()) &&
				(filter.SearchString is null || s.Text.Contains(filter.SearchString)));
			return Task.CompletedTask;
		}

		public async Task<Sentence?> ReadNextRandomSentence(List<string> prevList, List<IObjectOID> previousIDs, SentenceFilter filter)
		{
			List<Sentence>? sentenceQueryResult = ReadSentenceRange(filter).Result?.Where(s => s.Text.Contains(string.Join(' ', prevList))).Where(s => !previousIDs.Contains(s.OID)).ToList();
			if (sentenceQueryResult is null or [])
			{
				sentenceQueryResult = (await ReadSentenceRange(new SentenceFilter(filter.OID, filter.Author, null)))?.Where(s => s.Text.Contains(string.Join(' ', prevList))).Where(s => !previousIDs.Contains(s.OID)).ToList();
			}
			if (sentenceQueryResult is null or [])
			{
				return null;
			}

			return sentenceQueryResult[random.Next(sentenceQueryResult.Count - 1)];
		}

		public async Task<Sentence?> ReadRandomSentence(SentenceFilter filter)
		{
			List<Sentence>? sentenceQueryResult = ReadSentenceRange(filter).Result?.ToList();
			if (sentenceQueryResult is null or [])
			{
				sentenceQueryResult = (await ReadSentenceRange(new SentenceFilter(filter.OID, filter.Author, null)))?.ToList();
			}
			if (sentenceQueryResult is null or [])
			{
				return null;
			}
			return sentenceQueryResult[random.Next(sentenceQueryResult.Count - 1)];
		}

		public async Task<IEnumerable<Sentence>?> ReadSentenceRange(SentenceFilter filter)
		{
			return sentenceStorage.Where(s =>
				(filter.OID is null || s.OID.ToString().StartsWith(filter.OID.ToString())) &&
				(filter.Author is null || s.Author.ToString() == filter.Author.ToString()) &&
				(filter.SearchString is null || s.Text.Contains(filter.SearchString))
			).ToList();
		}

		public Task WriteSentence(Sentence sentence)
		{
			sentenceStorage.Add(sentence);
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
