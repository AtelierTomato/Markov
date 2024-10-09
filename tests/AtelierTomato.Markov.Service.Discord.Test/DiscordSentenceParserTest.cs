using AtelierTomato.Markov.Core;
using Discord;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

namespace AtelierTomato.Markov.Service.Discord.Test
{
	public class DiscordSentenceParserTest
	{
		[Theory]
		[InlineData(@"hello world how are *you*", @"hello world how are you")]
		[InlineData(@"we didn't have one for _single underscore italics_ so i added one in!", @"we didn 't have one for single underscore italics so i added one in !")]
		[InlineData(@"my favorite c function `puts` writes text", @"my favorite c function writes text")]
		[InlineData(@"i separated out the __underscore__ test because it made me mad", @"i separated out the underscore test because it made me mad")]
		[InlineData(@"b||elgiu||m and the netherlands are countries in europe", @"belgium and the netherlands are countries in europe")]
		[InlineData(@"wow **bold** and ***italicized bold***", @"wow bold and italicized bold")]
		[InlineData(@"this is an ~~apple~~ wait no it's an orange", @"this is an apple wait no it 's an orange")]
		public void ParseSurroundingMarkdownTest(string input, string output)
		{
			var options = new SentenceParserOptions();
			var discordOptions = new DiscordSentenceParserOptions();
			var target = new DiscordSentenceParser(Options.Create(options), Options.Create(discordOptions));

			var result = target.ParseIntoSentenceTexts(input);

			result.Should().ContainSingle().And.Contain(output);
		}

		[Theory]
		[InlineData(@"wal\*mart anime idol group in Pri\*Chan wow", @"wal*mart anime idol group in Pri*Chan wow")]
		[InlineData(@"this is how to italicize: \_italics\_ now u know", @"this is how to italicize : _italics_ now u know")]
		[InlineData(@"this is how to code snippet \`apple\`", @"this is how to code snippet `apple`")]
		[InlineData(@"nobody has ever underlined before but you do it like this: \_\_yeah\_\_", @"nobody has ever underlined before but you do it like this : __yeah__")]
		[InlineData(@"this is how to spoiler : \|\|snape killed dumbledore\|\|", @"this is how to spoiler : ||snape killed dumbledore||")]
		[InlineData(@"wow \*\*bold\*\* and \*\*\*italicized bold\*\*\*", @"wow **bold** and ***italicized bold***")]
		[InlineData(@"this is an \~\~apple\~\~ wait no it's an orange", @"this is an ~~apple~~ wait no it 's an orange")]
		public void ParseEscapedSurroundingMarkdownTest(string input, string output)
		{
			var options = new SentenceParserOptions();
			var discordOptions = new DiscordSentenceParserOptions();
			var target = new DiscordSentenceParser(Options.Create(options), Options.Create(discordOptions));

			var result = target.ParseIntoSentenceTexts(input);

			result.Should().ContainSingle().And.Contain(output);
		}

		[Theory]
		[InlineData(@"# lol this text is so big!", @"lol this text is so big !")]
		[InlineData(@"-# lol this text is so small!", @"lol this text is so small !")]
		[InlineData(@"## for no reason they also support this", @"for no reason they also support this")]
		[InlineData(@"### and nobody even uses this one, it's just bold", @"and nobody even uses this one , it 's just bold")]
		[InlineData(@">>> this is a big block quote multi-line just kill it", @"this is a big block quote multi - line just kill it")]
		public void ParsePrecedingMarkdownTest(string input, string output)
		{
			var options = new SentenceParserOptions();
			var discordOptions = new DiscordSentenceParserOptions();
			var target = new DiscordSentenceParser(Options.Create(options), Options.Create(discordOptions));

			var result = target.ParseIntoSentenceTexts(input);

			result.Should().ContainSingle().And.Contain(output);
		}

		[Theory]
		[InlineData(@"\# lol this text is so big!", @"# lol this text is so big !")]
		[InlineData(@"\#\# for no reason they also support this", @"# # for no reason they also support this")]
		[InlineData(@"\#\#\# and nobody even uses this one, it's just bold", @"# ## and nobody even uses this one , it 's just bold")]
		[InlineData(@"\>\>\> this is a big block quote multi-line just kill it", @"> >> this is a big block quote multi - line just kill it")]
		public void ParseEscapedPrecedingMarkdownTest(string input, string output)
		{
			var options = new SentenceParserOptions();
			var discordOptions = new DiscordSentenceParserOptions();
			var target = new DiscordSentenceParser(Options.Create(options), Options.Create(discordOptions));

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
		[InlineData(
			@"
				okay so here's code block 1
				```wow gaming```
				get ready for code block 2!
				```hello wrodl!```
				that's all for today i guess
			",
			new string[] {
				@"okay so here 's code block 1",
				@"get ready for code block 2 !",
				@"that 's all for today i guess"
			})]
		public void ParseCodeBlocksTest(string input, string[] output)
		{
			var options = new SentenceParserOptions();
			var discordOptions = new DiscordSentenceParserOptions();
			var target = new DiscordSentenceParser(Options.Create(options), Options.Create(discordOptions));

			var result = target.ParseIntoSentenceTexts(input);

			result.Should().HaveCount(output.Length);
			result.Should().Contain(output);
		}

		[Fact]
		public void ParseTextWithTagsTest()
		{
			var options = new SentenceParserOptions();
			var discordOptions = new DiscordSentenceParserOptions();
			var target = new DiscordSentenceParser(Options.Create(options), Options.Create(discordOptions));

			var input = @"yo <@!302915036492333067> and <@&308492793825853441>  can you <:ShihoLook:402558230427074560>  in <#614165694090838035> and get me some burgers or something";
			var output = @"yo Sandra and ice fairy can you e:ShihoLook: in fairy - land and get me some burgers or something";

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
			Mock.Get(emojiTag).SetupGet(t => t.Length).Returns(31);
			Mock.Get(emojiTag).SetupGet(t => t.Type).Returns(TagType.Emoji).Verifiable();
			Mock.Get(emojiTag).SetupGet(t => t.Value).Returns("ShihoLook").Verifiable();

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

		[Theory]
		[InlineData("<:ShihoLook:402558230427074560> <:ShihoLook:402558230427074560> <:ShihoLook:402558230427074560> <:ShihoLook:402558230427074560> <:ShihoLook:402558230427074560>", "e:ShihoLook: e:ShihoLook: e:ShihoLook: e:ShihoLook: e:ShihoLook:")]
		[InlineData("<:2311emoji:234593190> 2 3 4 5", "e:2311emoji: 2 3 4 5")]
		[InlineData("<:__:402558230427074560> 2 3 4 5", "e:__: 2 3 4 5")]
		[InlineData("<:ShihoLook:402558230427074560><:ShihoLook:402558230427074560> 2 3 4 5", "e:ShihoLook:e:ShihoLook: 2 3 4 5")]
		[InlineData("app<:ShihoLook:402558230427074560>le 2 3 4 5", "appe:ShihoLook:le 2 3 4 5")]
		[InlineData("<a:ShihoLook:402558230427074560> <a:ShihoLook:402558230427074560> <a:ShihoLook:402558230427074560> <a:ShihoLook:402558230427074560> <a:ShihoLook:402558230427074560>", "e:ShihoLook: e:ShihoLook: e:ShihoLook: e:ShihoLook: e:ShihoLook:")]
		[InlineData("<a:213412:234593190> 2 3 4 5", "e:213412: 2 3 4 5")]
		[InlineData("<a:__:402558230427074560> 2 3 4 5", "e:__: 2 3 4 5")]
		[InlineData("<a:ShihoLook:402558230427074560><:ShihoLook:402558230427074560> 2 3 4 5", "e:ShihoLook:e:ShihoLook: 2 3 4 5")]
		[InlineData("app<a:ShihoLook:402558230427074560>le 2 3 4 5", "appe:ShihoLook:le 2 3 4 5")]
		public void AdvancedEmojiTest(string input, string output)
		{
			var options = new SentenceParserOptions();
			var discordOptions = new DiscordSentenceParserOptions();
			var target = new DiscordSentenceParser(Options.Create(options), Options.Create(discordOptions));

			var result = target.ParseIntoSentenceTexts(input);

			result.Should().BeEquivalentTo(output);
		}

		[Theory]
		[InlineData("t!remindme to get dailies in 1 day 2 hours and 32 minutes", "remind me to get dailies in 1 day 2 hours and 32 minutes")]
		[InlineData("t!remind to get dailies in 1 day 2 hours and 32 minutes", "remind me to get dailies in 1 day 2 hours and 32 minutes")]
		public void ReplacePrefixesTest(string input, string output)
		{
			var options = new SentenceParserOptions();
			var discordOptions = new DiscordSentenceParserOptions();
			var target = new DiscordSentenceParser(Options.Create(options), Options.Create(discordOptions));

			var result = target.ParseIntoSentenceTexts(input);

			result.Should().BeEquivalentTo(output);
		}

		[Theory]
		[InlineData("whs")]
		[InlineData("whr")]
		[InlineData("\u0026meme")]
		[InlineData("\u0026caption2")]
		[InlineData("\u0026caption")]
		[InlineData("\u0026motivation")]
		[InlineData("Septapus wormhole send")]
		[InlineData("Septapus wormhole reply")]
		[InlineData("t!8ball")]
		[InlineData("666^8ball")]
		[InlineData("777^8ball")]
		[InlineData(".meme")]
		[InlineData(".caption")]
		[InlineData(".recaption")]
		public void RemovePrefixesTest(string prefix)
		{
			var options = new SentenceParserOptions();
			var discordOptions = new DiscordSentenceParserOptions();
			var target = new DiscordSentenceParser(Options.Create(options), Options.Create(discordOptions));

			var text = "1 2 3 4 5 6";
			var result = target.ParseIntoSentenceTexts(prefix + text);

			result.Should().BeEquivalentTo(text);
		}

		[Theory]
		[InlineData("t!")]
		[InlineData("t@")]
		[InlineData("m?")]
		[InlineData(")")]
		[InlineData("!")]
		[InlineData(".")]
		[InlineData("&")]
		[InlineData("dlm!")]
		[InlineData("y!")]
		[InlineData("s?")]
		[InlineData("Septapus")]
		[InlineData("s.")]
		[InlineData("ch!")]
		[InlineData("$")]
		[InlineData("/")]
		[InlineData("%")]
		[InlineData("666^")]
		[InlineData("777^")]
		[InlineData("?")]
		[InlineData("e!")]
		[InlineData("ed!")]
		[InlineData("d!")]
		[InlineData("h!")]
		[InlineData("a!")]
		[InlineData("o!")]
		[InlineData("p!")]
		[InlineData("m!")]
		[InlineData("b!")]
		[InlineData("~>")]
		[InlineData("->")]
		public void IgnorePrefixesTest(string prefix)
		{
			var options = new SentenceParserOptions();
			var discordOptions = new DiscordSentenceParserOptions();
			var target = new DiscordSentenceParser(Options.Create(options), Options.Create(discordOptions));

			var text = "1 2 3 4 5 6";
			var result = target.ParseIntoSentenceTexts(prefix + text);

			result.Should().BeEquivalentTo(Enumerable.Empty<string>());
		}

		[Theory]
		[InlineData("lol this is my server discord.gg/horseradish", "lol this is my server")]
		public void RemoveDiscordServersTest(string input, string output)
		{
			var options = new SentenceParserOptions();
			var discordOptions = new DiscordSentenceParserOptions();
			var target = new DiscordSentenceParser(Options.Create(options), Options.Create(discordOptions));

			var result = target.ParseIntoSentenceTexts(input);

			result.Should().BeEquivalentTo(output);
		}

		[Theory]
		[InlineData("[Google](<https://google.com>) is my favorite search engine", "Google is my favorite search engine")]
		[InlineData("[Google](https://google.com) is my favorite search engine", "Google is my favorite search engine")]
		public void ReplaceLinksTest(string input, string output)
		{
			var options = new SentenceParserOptions();
			var discordOptions = new DiscordSentenceParserOptions();
			var target = new DiscordSentenceParser(Options.Create(options), Options.Create(discordOptions));

			var result = target.ParseIntoSentenceTexts(input);

			result.Should().BeEquivalentTo(output);
		}

		[Theory]
		[InlineData("1 2 3 4 5 <t:2396027640:R>", "1 2 3 4 5 in 21 years")]
		[InlineData("1 2 3 4 5 <t:2396027640:D>", "1 2 3 4 5 December 4 , 2045")]
		[InlineData("1 2 3 4 5 <t:2396027640:d>", "1 2 3 4 5 12/04/2045")]
		[InlineData("1 2 3 4 5 <t:2396027640:T>", "1 2 3 4 5 11:14:00 AM")]
		[InlineData("1 2 3 4 5 <t:2396027640:t>", "1 2 3 4 5 11:14 AM")]
		[InlineData("1 2 3 4 5 <t:2396027640:F>", "1 2 3 4 5 Monday , December 4 , 2045 11:14 AM")]
		[InlineData("1 2 3 4 5 <t:2396027640:f>", "1 2 3 4 5 December 4 , 2045 11:14 AM")]
		public void ReplaceTimestampsTest(string input, string output)
		{
			var options = new SentenceParserOptions();
			var discordOptions = new DiscordSentenceParserOptions();
			var target = new DiscordSentenceParser(Options.Create(options), Options.Create(discordOptions));

			var result = target.ParseIntoSentenceTexts(input);

			result.Should().BeEquivalentTo(output);
		}

		// Below are tests copied from SentenceParserTests, but using the DiscordSentenceParser. All should pass or those that should not pass have been removed.
		[Theory]
		[InlineData(@"hello world how are you", @"hello world how are you")]
		[InlineData(@"I'm fine   and how are you", @"I 'm fine and how are you")]
		[InlineData(@"I’m fine   and how are you", @"I ’m fine and how are you")]
		[InlineData(@"wal*mart anime idol group wow", @"wal*mart anime idol group wow")]
		[InlineData(@"you don't like cheese .. .. .. not sure if i trust you anymore ...", @"you don 't like cheese ...... not sure if i trust you anymore ...")]
		[InlineData(@"the students' council decided you die today", @"the students ' council decided you die today")]
		[InlineData(@"check out http://zombo.com the best website", @"check out the best website")]
		[InlineData(@"funny money :laala: time yo", @"funny money :laala : time yo")]
		[InlineData(@":madou: 1 2 3 4 :mikanScreech: :gregor:", @":madou : 1 2 3 4 :mikanScreech : :gregor :")]
		public void ParseSimpleText(string input, string output)
		{
			var options = new SentenceParserOptions();
			var discordOptions = new DiscordSentenceParserOptions();
			var target = new DiscordSentenceParser(Options.Create(options), Options.Create(discordOptions));

			var result = target.ParseIntoSentenceTexts(input);

			result.Should().ContainSingle().And.Contain(output);
		}

		[Fact]
		public void ParseMultiSentenceText()
		{
			var options = new SentenceParserOptions();
			var discordOptions = new DiscordSentenceParserOptions();
			var target = new DiscordSentenceParser(Options.Create(options), Options.Create(discordOptions));

			var input = @"
War. War never changes.
The Romans waged war to gather slaves and wealth.
Spain built an empire from its lust for gold and territory.
Hitler shaped a battered Germany into an economic superpower.

But war never changes.

In the 21st century, war was still waged over the resources that could be acquired. Only this time, the spoils of war were also its weapons: Petroleum and Uranium. For these resources, China would invade Alaska, the US would annex Canada, and the European Commonwealth would dissolve into quarreling, bickering nation-states, bent on controlling the last remaining resources on Earth.

In 2077, the storm of world war had come again. In two brief hours, most of the planet was reduced to cinders. And from the ashes of nuclear devastation, a new civilization would struggle to arise.

A few were able to reach the relative safety of the large underground vaults. Your family was part of that group that entered Vault Thirteen. Imprisoned safely behind the large Vault door, under a mountain of stone, a generation has lived without knowledge of the outside world.

Life in the Vault is about to change.";
			var output = new string[]
			{
				@"The Romans waged war to gather slaves and wealth .",
				@"Spain built an empire from its lust for gold and territory .",
				@"Hitler shaped a battered Germany into an economic superpower .",
				@"In the 21st century , war was still waged over the resources that could be acquired .",
				@"Only this time , the spoils of war were also its weapons : Petroleum and Uranium .",
				@"For these resources , China would invade Alaska , the US would annex Canada , and the European Commonwealth would dissolve into quarreling , bickering nation - states , bent on controlling the last remaining resources on Earth .",
				@"In 2077 , the storm of world war had come again .",
				@"In two brief hours , most of the planet was reduced to cinders .",
				@"And from the ashes of nuclear devastation , a new civilization would struggle to arise .",
				@"A few were able to reach the relative safety of the large underground vaults .",
				@"Your family was part of that group that entered Vault Thirteen .",
				@"Imprisoned safely behind the large Vault door , under a mountain of stone , a generation has lived without knowledge of the outside world .",
				@"Life in the Vault is about to change ."
			};

			var result = target.ParseIntoSentenceTexts(input);

			result.Should().BeEquivalentTo(output);
		}

		[Theory]
		[MemberData(nameof(CaptureSentenceEndersData))]
		public void CaptureSentenceEnders(string input, IEnumerable<string> output)
		{
			var options = new SentenceParserOptions();
			var discordOptions = new DiscordSentenceParserOptions();
			var target = new DiscordSentenceParser(Options.Create(options), Options.Create(discordOptions));

			var result = target.ParseIntoSentenceTexts(input);

			result.Should().BeEquivalentTo(output);
		}

		public static IEnumerable<object[]> CaptureSentenceEndersData()
		{
			return [
				["hello and welcome to the server.", new string[] { "hello and welcome to the server ." }],
				["i like trains a lot!", new string[] { "i like trains a lot !" }],
				["i like cars a lot. do you also like cars?", new string[] { "i like cars a lot .", "do you also like cars ?" }],
				["megaman.exe 3 is considered to be a modern classic", new string[] { "megaman.exe 3 is considered to be a modern classic" }],
				["i think you're..... kinda stupid bro", new string[] { "i think you 're ..... kinda stupid bro" }],
				["where all the gamers at tho????????? wtf where are y'all", new string[] { "where all the gamers at tho ?????????", "wtf where are y 'all" }],
				["remember to b!optin all so that your messages are stored in the database!!!!", new string[] { "remember to b!optin all so that your messages are stored in the database !!!!" }],
				["you know, i was an adventurer too once. but then i got shot death with a gun", new string[] { "you know , i was an adventurer too once .", "but then i got shot death with a gun" }],
				["this is how you win (this is how you die forever)", new string[] { "this is how you win ( this is how you die forever )" }],
				["and so i was talking about \"anime girls\", and this guy comes in and starts screaming", new string[] { "and so i was talking about \" anime girls \" , and this guy comes in and starts screaming" }],
				["\"so too doth the man cree,ch\"", new string[] { "\" so too doth the man cree,ch \"" }],
				["this code is making me cringe [citation needed]", new string[] { "this code is making me cringe [ citation needed ]" }],
				["i like the .hack// franchise and the steins;gate franchise", new string[] { "i like the .hack// franchise and the steins;gate franchise" }],
				["sometimes people even use {this} alice ! did you know that yet ?", new string[] { "sometimes people even use { this } alice !", "did you know that yet ?" }],
				["this one can be ignored . . . if it's too hard . i guess  wretg res b dfs fgwd g da", new string[] { "this one can be ignored ... if it 's too hard .", "i guess wretg res b dfs fgwd g da" }],
				["me: trying to live my life; you: dying", new string[] { "me : trying to live my life ; you : dying" }],
				["i;m too tired to make anything more,,,,,,, djustr get these ones done please", new string[] { "i;m too tired to make anything more ,,,,,,, djustr get these ones done please" }],
				["i've thought of some more that alice's branch should be handling, what're the chances of getting this?", new string[] { "i 've thought of some more that alice 's branch should be handling , what 're the chances of getting this ?" }],
				["nani the fuck ?!?!?!?!wtf ?!?!?!?! why", new string[] { "nani the fuck ?!?!?!?! wtf ?!?!?!?! why" }],
				[" i am #dying over here", new string[] { "i am # dying over here" }],
				["listen to sleater-kinney on spotify", new string[] { "listen to sleater - kinney on spotify" }],
				["and i can tell that doki doki literature club is shitty faux-anime normie garbage", new string[] { "and i can tell that doki doki literature club is shitty faux - anime normie garbage" }],
				["dotlipses fu*king suck ...and so do commalipses", new string[] { "dotlipses fu*king suck ... and so do commalipses" }],
				["girlfriend in ,,,several commas i know", new string[] { "girlfriend in ,,, several commas i know" }],
				["lisp is (fun), unless you (defun x).", new string[] { "lisp is ( fun ) , unless you ( defun x ) ." }],
				["lisp is “fun”, unless you “defun x”.", new string[] { "lisp is “ fun ” , unless you “ defun x ” ." }],
				[">implying that i'm implying", new string[] { "> implying that i 'm implying" }],
				[">implying that i am implying", new string[] { "> implying that i am implying" }],
				["i want to eat—drink water", new string[] { "i want to eat — drink water" }]
			];
		}

		[Theory]
		[InlineData("")]
		[InlineData(" ")]
		public void TokenizeEdgeCasesTest(string input)
		{
			var options = new SentenceParserOptions();
			var discordOptions = new DiscordSentenceParserOptions();
			var target = new DiscordSentenceParser(Options.Create(options), Options.Create(discordOptions));


			var result = target.ParseIntoSentenceTexts(input);

			result.Should().BeEquivalentTo(Enumerable.Empty<string>());
		}


		[Theory]
		[InlineData("¿Dónde está mi gran sombrero?", "¿ Dónde está mi gran sombrero ?")]
		[InlineData("¡No encuentro mi pierna izquierda!", "¡ No encuentro mi pierna izquierda !")]
		[InlineData("¡¿Alguien puede ayudarme por favor?!", "¡¿ Alguien puede ayudarme por favor ?!")]
		[InlineData("¡¡¡¿¿¿Alguien puede ayudarme por favor???!!!", "¡¡¡¿¿¿ Alguien puede ayudarme por favor ???!!!")]
		[InlineData("¡ ¡ ¡ ¿ ¿ ¿Alguien puede ayudarme por favor? ? ? ! ! !", "¡¡¡¿¿¿ Alguien puede ayudarme por favor ???!!!")]
		[InlineData("«Je m'appelle Marinette, une fille comme les autres»", "« Je m 'appelle Marinette , une fille comme les autres »")]
		[InlineData("»Je m'appelle Marinette, une fille comme les autres«", "» Je m 'appelle Marinette , une fille comme les autres «")]
		public void ForeignTests(string input, string output)
		{
			var options = new SentenceParserOptions();
			var discordOptions = new DiscordSentenceParserOptions();
			var target = new DiscordSentenceParser(Options.Create(options), Options.Create(discordOptions));

			var result = target.ParseIntoSentenceTexts(input);

			result.Should().ContainSingle().And.Contain(output);
		}

		[Theory]
		[InlineData("(this is a sentence in parentheses.) This is sentence number two", new string[] { "( this is a sentence in parentheses . )", "This is sentence number two" })]
		[InlineData("[this is a sentence in brackets.] This is sentence number two", new string[] { "[ this is a sentence in brackets . ]", "This is sentence number two" })]
		[InlineData("{this is a sentence in curly braces.} This is sentence number two", new string[] { "{ this is a sentence in curly braces . }", "This is sentence number two" })]
		[InlineData("\"this is a sentence in quotes.\" This is sentence number two", new string[] { "\" this is a sentence in quotes . \"", "This is sentence number two" })]
		[InlineData("»this is a sentence in quotes.« This is sentence number two", new string[] { "» this is a sentence in quotes . «", "This is sentence number two" })]
		[InlineData("«this is a sentence in quotes.» This is sentence number two", new string[] { "« this is a sentence in quotes . »", "This is sentence number two" })]
		[InlineData("“this is a sentence in quotes.” This is sentence number two", new string[] { "“ this is a sentence in quotes . ”", "This is sentence number two" })]
		[InlineData("[(this is a sentence in multiple closers.)] This is sentence number two", new string[] { "[ ( this is a sentence in multiple closers . ) ]", "This is sentence number two" })]
		[InlineData("(this is a sentence in parentheses. ) This is sentence number two", new string[] { "( this is a sentence in parentheses .", ") This is sentence number two" })]
		public void CaptureTrailingPunctuationTests(string input, IEnumerable<string> output)
		{
			var options = new SentenceParserOptions();
			var discordOptions = new DiscordSentenceParserOptions();
			var target = new DiscordSentenceParser(Options.Create(options), Options.Create(discordOptions));

			var result = target.ParseIntoSentenceTexts(input);

			result.Should().BeEquivalentTo(output);
		}
	}
}
