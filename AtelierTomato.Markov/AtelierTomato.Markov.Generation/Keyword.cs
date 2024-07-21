using AtelierTomato.Markov.Data;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Generation
{
	public class Keyword(IWordStatisticAccess wordStatisticAccess, IOptions<KeywordOptions> options)
	{
		private readonly IWordStatisticAccess wordStatisticAccess = wordStatisticAccess;
		private readonly KeywordOptions options = options.Value;

		public async Task<string> Find(string str)
		{
			var sentenceWords = (await wordStatisticAccess.ReadWordStatisticRange(str.Split(' '))).Where(x => x.Appearances > options.MinimumAppearancesForKeyword && options.IgnoreKeyword.Contains(x.Name));
			return sentenceWords.MinBy(w => w.Appearances)?.Name ?? string.Empty;
		}
	}
}
