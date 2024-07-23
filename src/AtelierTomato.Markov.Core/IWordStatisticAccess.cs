using AtelierTomato.Markov.Core.Model;

namespace AtelierTomato.Markov.Core
{
	public interface IWordStatisticAccess
	{
		Task<WordStatistic> ReadWordStatistic(string word);
		Task<IEnumerable<WordStatistic>> ReadWordStatisticRange(IEnumerable<string> words);
		Task WriteWordStatistic(WordStatistic wordStatistic);
		Task WriteWordStatisticRange(IEnumerable<WordStatistic> wordStatistics);
		public async Task WriteWordStatisticsFromString(string str)
		{
			var tokenizedStr = str.Split(' ');
			var storedWordStatistics = (await ReadWordStatisticRange(tokenizedStr.Distinct())).ToDictionary(w => w.Name, w => w.Appearances);

			foreach (string word in tokenizedStr)
			{
				_ = storedWordStatistics.TryGetValue(word, out var appearances);
				storedWordStatistics[word] = appearances + 1;
			}

			await WriteWordStatisticRange(storedWordStatistics.Select(kv => new WordStatistic(kv.Key, kv.Value)));
		}
	}
}
