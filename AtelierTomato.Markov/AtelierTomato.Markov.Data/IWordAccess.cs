using AtelierTomato.Markov.Data.Model;

namespace AtelierTomato.Markov.Data
{
	public interface IWordAccess
	{
		Task<Word> ReadWord(string name);
		Task<IEnumerable<Word>> ReadWordRange(IEnumerable<string> names);
		Task WriteWord(Word word);
		Task WriteWordRange(IEnumerable<Word> words);
		public async Task WriteWordsFromString(string str)
		{
			var tokenizedStr = str.Split(' ');
			var storedWords = (await ReadWordRange(tokenizedStr.Distinct())).ToDictionary(w => w.Name, w => w.Appearances);

			foreach (string wordName in tokenizedStr)
			{
				_ = storedWords.TryGetValue(wordName, out var appearances);
				storedWords[wordName] = appearances + 1;
			}

			await WriteWordRange(storedWords.Select(kv => new Word(kv.Key, kv.Value)));
		}
	}
}
