using Discord;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

namespace AtelierTomato.Markov.Parser.Test
{
	public class DiscordSentenceParserTest
	{
		[Theory]
		[InlineData(@"hello world how are *you*", @"hello world how are you")]
		[InlineData(@"we didn't have one for _single underscore italics_ so i added one in!", @"we didn 't have one for single apostrophe italics so i added one in !")]
		[InlineData(@"my favorite c function `puts` writes text", @"my favorite c function writes text")]
		[InlineData(@"i separated out the __underscore__ test because it made me mad", @"i separated out the underscore test because it made me mad")]
		[InlineData(@"b||elgiu||m and the netherlands are countries in europe", @"belgium and the netherlands are countries in europe")]
		[InlineData(@"wow **bold** and ***italicized bold***", @"wow bold and italicized bold")]
		[InlineData(@"this is an ~~apple~~ wait no it's an orange", @"this is an apple wait no it 's an orange")]
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
		[InlineData(@"this is an \~\~apple\~\~ wait no it's an orange", @"this is an ~~apple~~ wait no it 's an orange")]
		public void ParseEscapedSurroundingMarkdownTest(string input, string output)
		{
			var options = new SentenceParserOptions();
			var target = new DiscordSentenceParser(Options.Create(options));

			var result = target.ParseIntoSentenceTexts(input);

			result.Should().ContainSingle().And.Contain(output);
		}

		[Theory]
		[InlineData(@"# lol this text is so big!", @"lol this text is so big !")]
		[InlineData(@"# discord is so dumb and stupd#", @"discord is so dumb and stupd")]
		[InlineData(@"## for no reason they also support this", @"for no reason they also support this")]
		[InlineData(@"### and nobody even uses this one, it's just bold", @"and nobody even uses this one , it 's just bold")]
		[InlineData(@">>> this is a big block quote multi-line just kill it", @"this is a big block quote multi- line just kill it")]
		public void ParsePrecedingMarkdownTest(string input, string output)
		{
			var options = new SentenceParserOptions();
			var target = new DiscordSentenceParser(Options.Create(options));

			var result = target.ParseIntoSentenceTexts(input);

			result.Should().ContainSingle().And.Contain(output);
		}

		[Theory]
		[InlineData(@"\# lol this text is so big!", @"# lol this text is so big !")]
		[InlineData(@"\# discord is so dumb and stupd\#", @"# discord is so dumb and stupd#")]
		[InlineData(@"\#\# for no reason they also support this", @"## for no reason they also support this")]
		[InlineData(@"\#\#\# and nobody even uses this one, it's just bold", @"### and nobody even uses this one , it 's just bold")]
		[InlineData(@"\>\>\> this is a big block quote multi-line just kill it", @">>> this is a big block quote multi- line just kill it")]
		public void ParseEscapedPrecedingMarkdownTest(string input, string output)
		{
			var options = new SentenceParserOptions();
			var target = new DiscordSentenceParser(Options.Create(options));

			var result = target.ParseIntoSentenceTexts(input);


			result.Should().ContainSingle().And.Contain(output);
		}

		[Theory]
		[InlineData(
			@"
				i am gonna put a code block okay here ```yeah this is my code woo``` this is my under tale
			",
			new string[] {
				@"i am gonna put a code block okay here",
				@"this is my under tale"
			}
		)]
		[InlineData(
			@"
				yeah so if you want to use foreach it's like this
				```cs
				foreach (var str in stringRange)
				{
					// do something
				}```
				hope that helps and happy programming
			",
			new string[] {
				@"yeah so if you want to use foreach it 's like this",
				@"hope that helps and happy programming" }
		)]
		public void ParseCodeBlocksTest(string input, string[] output)
		{
			var options = new SentenceParserOptions();
			var target = new DiscordSentenceParser(Options.Create(options));

			var result = target.ParseIntoSentenceTexts(input);

			result.Should().HaveCount(output.Length);
			result.Should().Contain(output);
		}

		[Fact]
		public void ParseTextWithTagsTest()
		{
			var options = new SentenceParserOptions();
			var target = new DiscordSentenceParser(Options.Create(options));

			var input = @"yo <@!302915036492333067> and <@&308492793825853441>  can you <:ShihoLook:402558230427074560>  in <#614165694090838035> and get me some burgers or something";
			var output = @"yo Sandra and ice fairy can you e:ShihoLook: in fairy- land and get me some burgers or something";

			var mentionUser = Mock.Of<IUser>();
			Mock.Get(mentionUser).SetupGet(u => u.Username).Returns("Sandra").Verifiable();

			var userMentionTag = Mock.Of<ITag>();
			Mock.Get(userMentionTag).SetupGet(t => t.Index).Returns(3).Verifiable();
			Mock.Get(userMentionTag).SetupGet(t => t.Length).Returns(22).Verifiable();
			Mock.Get(userMentionTag).SetupGet(t => t.Type).Returns(TagType.UserMention).Verifiable();
			Mock.Get(userMentionTag).SetupGet(t => t.Value).Returns(mentionUser).Verifiable();

			var roleMentionTag = Mock.Of<ITag>();
			Mock.Get(roleMentionTag).SetupGet(t => t.Index).Returns(30).Verifiable();
			Mock.Get(roleMentionTag).SetupGet(t => t.Length).Returns(22).Verifiable();
			Mock.Get(roleMentionTag).SetupGet(t => t.Type).Returns(TagType.RoleMention).Verifiable();
			Mock.Get(roleMentionTag).SetupGet(t => t.Value).Returns("ice fairy").Verifiable();

			var emojiTag = Mock.Of<ITag>();
			Mock.Get(emojiTag).SetupGet(t => t.Index).Returns(62).Verifiable();
			Mock.Get(emojiTag).SetupGet(t => t.Length).Returns(31).Verifiable();
			Mock.Get(emojiTag).SetupGet(t => t.Type).Returns(TagType.Emoji).Verifiable();
			Mock.Get(emojiTag).SetupGet(t => t.Value).Returns("ShihoLook");

			var channelTag = Mock.Of<ITag>();
			Mock.Get(channelTag).SetupGet(t => t.Index).Returns(98).Verifiable();
			Mock.Get(channelTag).SetupGet(t => t.Length).Returns(21).Verifiable();
			Mock.Get(channelTag).SetupGet(t => t.Type).Returns(TagType.ChannelMention).Verifiable();
			Mock.Get(channelTag).SetupGet(t => t.Value).Returns("fairy-land").Verifiable();

			var tags = new List<ITag> { userMentionTag, roleMentionTag, emojiTag, channelTag };

			var result = target.ParseIntoSentenceTexts(input, tags);

			result.Should().ContainSingle().And.Contain(output);

			Mock.Get(mentionUser).Verify();
			Mock.Get(userMentionTag).Verify();
			Mock.Get(roleMentionTag).Verify();
			Mock.Get(emojiTag).Verify();
			Mock.Get(channelTag).Verify();
		}
	}
}
