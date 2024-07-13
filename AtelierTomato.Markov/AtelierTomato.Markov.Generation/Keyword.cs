using AtelierTomato.Markov.Data;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Generation
{
	public class Keyword(IWordAccess wordAccess, IOptions<KeywordOptions> options)
	{
		private readonly IWordAccess wordAccess = wordAccess;
		private readonly KeywordOptions options = options.Value;

		public async Task<string> Find(string str)
		{
			var sentenceWords = (await wordAccess.ReadWordRange(str.Split(' '))).Where(x => x.Appearances > options.MinimumAppearancesForKeyword && options.IgnoreKeyword.Contains(x.Name));
			return sentenceWords.MinBy(w => w.Appearances)?.Name ?? string.Empty;
		}
	}
}
