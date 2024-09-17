using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage
{
	public class InMemorySentenceAccess : ISentenceAccess
	{
		private readonly Random random = new();
		private readonly List<Sentence> sentenceStorage = [];
		public IReadOnlyList<Sentence> SentenceStorage { get => sentenceStorage; }
		public Task DeleteSentenceRange(SentenceFilter filter, string? searchString = null)
		{
			if (filter.OIDs.Count is 0 && filter.Authors.Count is 0 && string.IsNullOrWhiteSpace(searchString))
				throw new ArgumentException("You cannot delete all sentences from the database through this command, at least one part of the filter must have a value.", nameof(filter));

			sentenceStorage.RemoveAll(s =>
				(filter.OIDs.Count is 0 || filter.OIDs.Any(oid => s.OID.ToString().StartsWith(oid.ToString(), StringComparison.InvariantCultureIgnoreCase))) &&
				(filter.Authors.Count is 0 || filter.Authors.Any(author => s.Author.ToString() == author.ToString())) && (searchString is null || s.Text.Contains(searchString)));
			return Task.CompletedTask;
		}

		public async Task<IEnumerable<Sentence>> ReadNextRandomSentences(int amount, List<string> prevList, List<IObjectOID> previousIDs, SentenceFilter filter, string? keyword = null)
		{
			List<Sentence>? sentenceQueryResult = (await ReadSentenceRange(filter, keyword)).Where(s => s.Text.Contains(string.Join(' ', prevList))).Where(s => !previousIDs.Contains(s.OID)).ToList();
			if (sentenceQueryResult is null or [])
			{
				sentenceQueryResult = (await ReadSentenceRange(filter)).Where(s => s.Text.Contains(string.Join(' ', prevList))).Where(s => !previousIDs.Contains(s.OID)).ToList();
			}
			if (sentenceQueryResult is null or [])
			{
				return [];
			}
			sentenceQueryResult = sentenceQueryResult.OrderBy(x => random.Next()).ToList();
			var resultCount = Math.Min(amount, sentenceQueryResult.Count);

			return sentenceQueryResult.Take(resultCount);
		}

		public async Task<Sentence?> ReadRandomSentence(SentenceFilter filter, string? keyword = null)
		{
			List<Sentence> sentenceQueryResult = (await ReadSentenceRange(filter, keyword)).ToList();
			if (sentenceQueryResult is null or [])
			{
				sentenceQueryResult = (await ReadSentenceRange(filter)).ToList();
			}
			if (sentenceQueryResult is null or [])
			{
				return null;
			}
			return sentenceQueryResult[random.Next(sentenceQueryResult.Count - 1)];
		}

		public Task<IEnumerable<Sentence>> ReadSentenceRange(SentenceFilter filter, string? searchString = null)
		{
			return Task.FromResult(sentenceStorage.Where(s =>
				(filter.OIDs.Count is 0 || filter.OIDs.Any(oid => s.OID.ToString().StartsWith(oid.ToString(), StringComparison.InvariantCultureIgnoreCase))) &&
				(filter.Authors.Count is 0 || filter.Authors.Any(author => s.Author.ToString() == author.ToString())) &&
				(searchString is null || s.Text.Contains(searchString))
			));
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
