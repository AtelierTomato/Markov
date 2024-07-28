using FluentAssertions;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Core.Test
{
	public class SentenceParserTests
	{
		[Theory]
		[InlineData(@"hello world how are you", @"hello world how are you")]
		[InlineData(@"I'm fine   and how are you", @"I 'm fine and how are you")]
		[InlineData(@"wal*mart anime idol group wow", @"wal*mart anime idol group wow")]
		[InlineData(@"you don't like cheese .. .. .. not sure if i trust you anymore ...", @"you don 't like cheese ...... not sure if i trust you anymore ...")]
		[InlineData(@"the students' council decided you die today", @"the students ' council decided you die today")]
		[InlineData(@"check out http://zombo.com the best website", @"check out the best website")]
		[InlineData(@"funny money :laala: time yo", @"funny money :laala : time yo")]
		public void ParseSimpleText(string input, string output)
		{
			var options = new SentenceParserOptions();
			var target = new SentenceParser(Options.Create(options));

			var result = target.ParseIntoSentenceTexts(input);

			result.Should().ContainSingle().And.Contain(output);
		}

		[Fact]
		public void ParseMultiSentenceText()
		{
			var options = new SentenceParserOptions();
			var target = new SentenceParser(Options.Create(options));

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
				@"For these resources , China would invade Alaska , the US would annex Canada , and the European Commonwealth would dissolve into quarreling , bickering nation- states , bent on controlling the last remaining resources on Earth .",
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
			var target = new SentenceParser(Options.Create(options));

			var result = target.ParseIntoSentenceTexts(input);

			result.Should().BeEquivalentTo(output);
		}

		public static IEnumerable<object[]> CaptureSentenceEndersData()
		{
			return new List<object[]> {
				new object[] { "hello and welcome to the server.", new string[] { "hello and welcome to the server ." } },
				new object[] { "i like trains a lot!", new string[] { "i like trains a lot !" } },
				new object[] { "i like cars a lot. do you also like cars?", new string[] { "i like cars a lot .", "do you also like cars ?" } },
				new object[] { "megaman.exe 3 is considered to be a modern classic", new string[] { "megaman.exe 3 is considered to be a modern classic" } },
				new object[] { "i think you're..... kinda stupid bro", new string[] { "i think you 're ..... kinda stupid bro" } },
				new object[] { "where all the gamers at tho????????? wtf where are y'all", new string[] { "where all the gamers at tho ?????????", "wtf where are y 'all" } },
				new object[] { "remember to b!optin all so that your messages are stored in the database!!!!", new string[] { "remember to b!optin all so that your messages are stored in the database !!!!" } },
				new object[] { "you know, i was an adventurer too once. but then i got shot death with a gun", new string[] { "you know , i was an adventurer too once .", "but then i got shot death with a gun" } },
				new object[] { "this is how you win (this is how you die forever)", new string[] { "this is how you win ( this is how you die forever )" } },
				new object[] { "and so i was talking about \"anime girls\", and this guy comes in and starts screaming", new string[] { "and so i was talking about \" anime girls \" , and this guy comes in and starts screaming" } },
				new object[] { "\"so too doth the man cree,ch\"", new string[] { "\" so too doth the man cree,ch \"" } },
				new object[] { "this code is making me cringe [citation needed]", new string[] { "this code is making me cringe [ citation needed ]" } },
				new object[] { "i like the .hack// franchise and the steins;gate franchise", new string[] { "i like the .hack// franchise and the steins;gate franchise" } },
				new object[] { "sometimes people even use {this} alice ! did you know that yet ?", new string[] { "sometimes people even use { this } alice !", "did you know that yet ?" } },
				new object[] { "this one can be ignored . . . if it's too hard . i guess  wretg res b dfs fgwd g da", new string[] { "this one can be ignored ... if it 's too hard .", "i guess wretg res b dfs fgwd g da" } },
				new object[] { "me: trying to live my life; you: dying", new string[] { "me : trying to live my life ; you : dying" } },
				new object[] { "i;m too tired to make anything more,,,,,,, djustr get these ones done please", new string[] { "i;m too tired to make anything more ,,,,,,, djustr get these ones done please" } },
				new object[] { "i've thought of some more that alice's branch should be handling, what're the chances of getting this?", new string[] { "i 've thought of some more that alice 's branch should be handling , what 're the chances of getting this ?" } },
				new object[] { "nani the fuck ?!?!?!?!wtf ?!?!?!?! why", new string[] { "nani the fuck ?!?!?!?! wtf ?!?!?!?! why" } },
				new object[] { " i am #dying over here", new string[] { "i am # dying over here" } },
				new object[] { "listen to sleater-kinney on spotify", new string[] { "listen to sleater- kinney on spotify" } },
				new object[] { "and i can tell that doki doki literature club is shitty faux-anime normie garbage", new string[] { "and i can tell that doki doki literature club is shitty faux- anime normie garbage" } },
				new object[] { "dotlipses fu*king suck ...and so do commalipses", new string[] { "dotlipses fu*king suck ... and so do commalipses" } },
				new object[] { "girlfriend in ,,,several commas i know", new string[] { "girlfriend in ,,, several commas i know" } },
				new object[] { "lisp is (fun), unless you (defun x).", new string[] { "lisp is ( fun ) , unless you ( defun x ) ." } },
				new object[] { ">implying that i'm implying", new string[] { "> implying that i 'm implying" } },
				new object[] { ">implying that i am implying", new string[] { "> implying that i am implying" } },
			};
		}

		[Theory]
		[InlineData("")]
		[InlineData(" ")]
		public void TokenizeEdgeCasesTest(string input)
		{
			var options = new SentenceParserOptions();
			var target = new SentenceParser(Options.Create(options));


			var result = target.ParseIntoSentenceTexts(input);

			result.Should().BeEquivalentTo(Enumerable.Empty<string>());
		}

		[Theory]
		[InlineData("¿Dónde está mi gran sombrero?", "¿ Dónde está mi gran sombrero ?")]
		[InlineData("¡No encuentro mi pierna izquierda!", "¡ No encuentro mi pierna izquierda !")]
		[InlineData("¡¿Alguien puede ayudarme por favor?!", "¡¿ Alguien puede ayudarme por favor ?!")]
		[InlineData("¡¡¡¿¿¿Alguien puede ayudarme por favor???!!!", "¡¡¡¿¿¿ Alguien puede ayudarme por favor ???!!!")]
		[InlineData("¡ ¡ ¡ ¿ ¿ ¿Alguien puede ayudarme por favor? ? ? ! ! !", "¡¡¡¿¿¿ Alguien puede ayudarme por favor ???!!!")]
		public void ForeignTests(string input, string output)
		{
			var options = new SentenceParserOptions();
			var target = new SentenceParser(Options.Create(options));

			var result = target.ParseIntoSentenceTexts(input);

			result.Should().ContainSingle().And.Contain(output);
		}
	}
}