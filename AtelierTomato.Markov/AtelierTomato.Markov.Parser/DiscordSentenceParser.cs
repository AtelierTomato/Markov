using Discord;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Parser
{
	public class DiscordSentenceParser(IOptions<SentenceParserOptions> options) : SentenceParser(options)
	{
		public IEnumerable<string> ParseIntoSentenceTexts(string input, IEnumerable<ITag> tags)
		{
			return ParseIntoSentenceTexts(input);
		}
	}
}
