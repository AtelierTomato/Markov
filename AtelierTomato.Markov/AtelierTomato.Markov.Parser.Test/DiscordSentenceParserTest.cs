using FluentAssertions;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Parser.Test
{
	public class DiscordSentenceParserTest
	{
		[Theory]
		[InlineData(@"one two three four <:smimsy:1166315314548576306>", @"one two three four e:smimsy:")]
		[InlineData(@"satoko-chan is so cute <:hauuu:1082459047397171311>", @"satoko- chan is so cute e:hauuu:")]
		public void ParseEmojiTest(string input, string output)
		{
			var options = new SentenceParserOptions();
			var target = new DiscordSentenceParser(Options.Create(options));

			var result = target.ParseIntoSentenceTexts(input);

			result.Should().ContainSingle().And.Contain(output);
		}
	}
}
