using FluentAssertions;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Parser.Test
{
	public class SentenceParserTests
	{
		[Theory]
		[InlineData(@"hello world how are you", @"hello world how are you")]
		[InlineData(@"I'm fine   and how are you", @"I \'m fine and how are you")]
		[InlineData(@"wal*mart anime idol group wow", @"wal\*mart anime idol group wow")]
		[InlineData(@"you don't like cheese .. .. .. not sure if i trust you anymore ...", @"you don \'t like cheese ...... not sure if i trust you anymore ...")]
		[InlineData(@"the students' council decided you die today", @"the students \' council decided you die today")]
		[InlineData(@"check out http://zombo.com the best website", @"check out the best website")]
		[InlineData(@"funny money :laala: time yo", @"funny money \:laala \: time yo")]
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
				@"Only this time , the spoils of war were also its weapons \: Petroleum and Uranium .",
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
	}
}