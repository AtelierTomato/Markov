using AtelierTomato.Markov.Data;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Generation
{
	public class KeywordMarkovChain(ISentenceAccess sentenceAccess, IOptions<MarkovGenerationOptions> options, KeywordProvider keywordProvider) : MarkovChain(sentenceAccess, options)
	{
		private readonly KeywordProvider keywordProvider = keywordProvider;
		/// <summary>
		/// Used to generate a sentence without manually setting a keyword. The <paramref name="filter"/> MUST not have a SearchString.
		/// The <paramref name="str"/> will be used as an input to keywordProvider.Find() to determine the keyword.
		/// Then the return of that will be set as <paramref name="filter"/>.SearchString and used to call the base class's Generate().
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="str"></param>
		/// <param name="firstWord"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		public async Task<string> Generate(SentenceFilter filter, string str, string? firstWord)
		{
			if (filter.SearchString is not null)
				throw new ArgumentException("You cannot generate a keyword if there is a keyword already present in the filter.", nameof(filter));

			filter.SearchString = await keywordProvider.Find(str);
			return await base.Generate(filter, firstWord);
		}
	}
}
