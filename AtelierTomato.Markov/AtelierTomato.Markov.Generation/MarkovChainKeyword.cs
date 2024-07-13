using AtelierTomato.Markov.Data;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Generation
{
	public class MarkovChainKeyword(ISentenceAccess sentenceAccess, IOptions<MarkovGenerationOptions> options, Keyword keyword) : MarkovChain(sentenceAccess, options)
	{
		private readonly Keyword keyword = keyword;
		public async Task<string> Generate(SentenceFilter filter, string str, string? firstWord = null)
		{
			if (filter.SearchString is not null)
			{
				throw new ArgumentException("You cannot generate a keyword if there is a keyword already present in the filter.", nameof(filter));
			}
			filter.SearchString = await keyword.Find(str);
			return await base.Generate(filter, firstWord);
		}
	}
}
