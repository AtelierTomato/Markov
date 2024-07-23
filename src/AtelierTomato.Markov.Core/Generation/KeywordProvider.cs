using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Core.Generation
{
	public class KeywordProvider(IWordStatisticAccess wordStatisticAccess, IOptions<KeywordOptions> options)
	{
		private readonly IWordStatisticAccess wordStatisticAccess = wordStatisticAccess;
		private readonly KeywordOptions options = options.Value;

		/// <summary>
		/// Finds the keyword of <paramref name="str"/> by determining what the least common word in the string is.
		/// </summary>
		/// <param name="str">The string that will be used to determine the keyword.</param>
		/// <returns></returns>
		public async Task<string> Find(string str)
		{
			var sentenceWords = (await wordStatisticAccess.ReadWordStatisticRange(str.Split(' '))).Where(x => x.Appearances > options.MinimumAppearancesForKeyword && options.IgnoreKeyword.Contains(x.Name));
			return sentenceWords.MinBy(w => w.Appearances)?.Name ?? string.Empty;
		}
	}
}
