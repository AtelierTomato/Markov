using FluentAssertions;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Parser.Test
{
	public class DiscordSentenceParserTest
	{
		[Fact]
		public void ParseEmojiTest()
		{
			var options = new SentenceParserOptions();
			var target = new DiscordSentenceParser(Options.Create(options));

			var result = target.ParseIntoSentenceTexts(@"one two three four <:smimsy:1166315314548576306>");

			result.Should().ContainSingle().And.Contain(@"one two three four e:smimsy:");
		}
	}
}
