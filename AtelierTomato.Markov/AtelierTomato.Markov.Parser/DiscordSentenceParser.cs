using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Parser
{
	public class DiscordSentenceParser(IOptions<SentenceParserOptions> options) : SentenceParser(options)
	{

	}
}
