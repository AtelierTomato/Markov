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

		// These two tests only have what I remember off the top of my head, check if Discord has any others later.
		[Theory]
		[InlineData(@"hello world how are *you*", @"hello world how are you")]
		[InlineData(@"we didn't have one for _single underscore italics_ so i added one in!", @"we didn 't have one for single apostrophe italics so i added one in !")]
		[InlineData(@"my favorite c function `puts` writes text", @"my favorite c function writes text")]
		[InlineData(@"i separated out the __underscore__ test because it made me mad", @"i separated out the underscore test because it made me mad")]
		[InlineData(@"b||elgiu||m and the netherlands are countries in europe", @"belgium and the netherlands are countries in europe")]
		[InlineData(@"wow **bold** and ***italicized bold***", @"wow bold and italicized bold")]
		public void ParseSurroundingMarkdownTest(string input, string output)
		{
			var options = new SentenceParserOptions();
			var target = new DiscordSentenceParser(Options.Create(options));

			var result = target.ParseIntoSentenceTexts(input);

			result.Should().ContainSingle().And.Contain(output);
		}

		[Theory]
		[InlineData(@"wal\*mart anime idol group in Pri\*Chan wow", @"wal*mart anime idol group in Pri*Chan wow")]
		[InlineData(@"this is how to italicize: \_italics\_ now u know", @"this is how to italicize: _italics_ now u know")]
		[InlineData(@"this is how to code snippet \`apple\`", @"this is how to code snippet `apple`")]
		[InlineData(@"nobody has ever underlined before but you do it like this: \_\_yeah\_\_", @"nobody has ever underlined before but you do it like this : __yeah__")]
		[InlineData(@"this is how to spoiler: \|\|snape killed dumbledore\|\|", @"this is how to spoiler : ||snape killed dumbledore||")]
		[InlineData(@"wow \*\*bold\*\* and \*\*\*italicized bold\*\*\*", @"wow **bold** and ***italicized bold***")]
		public void ParseEscapedSurroundingMarkdownTest(string input, string output)
		{
			var options = new SentenceParserOptions();
			var target = new DiscordSentenceParser(Options.Create(options));

			var result = target.ParseIntoSentenceTexts(input);

			result.Should().ContainSingle().And.Contain(output);
		}
	}
}
