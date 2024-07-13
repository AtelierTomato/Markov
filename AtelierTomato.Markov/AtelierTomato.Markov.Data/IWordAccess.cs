using AtelierTomato.Markov.Data.Model;

namespace AtelierTomato.Markov.Data
{
	public interface IWordAccess
	{
		Task<Word> ReadWord(string name);
		Task<IEnumerable<Word>> ReadWordRange(IEnumerable<string> names);
		Task WriteWord(Word word);
		Task WriteWordRange(IEnumerable<Word> words);
	}
}
