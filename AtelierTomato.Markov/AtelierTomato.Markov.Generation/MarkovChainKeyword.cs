using AtelierTomato.Markov.Data;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Generation
{
	public class MarkovChainKeyword(ISentenceAccess sentenceAccess, IOptions<MarkovGenerationOptions> options, KeywordProvider keywordProvider) : MarkovChain(sentenceAccess, options)
	{
		private readonly KeywordProvider keywordProvider = keywordProvider;
		public async Task<string> Generate(SentenceFilter filter, string str, string? firstWord)
		{
			if (filter.SearchString is not null)
			{
				throw new ArgumentException("You cannot generate a keyword if there is a keyword already present in the filter.", nameof(filter));
			}
			filter.SearchString = await keywordProvider.Find(str);
			return await base.Generate(filter, firstWord);
		}
	}
}
