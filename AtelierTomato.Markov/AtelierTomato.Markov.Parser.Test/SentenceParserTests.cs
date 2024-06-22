using FluentAssertions;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Parser.Test
{
	public class SentenceParserTests
	{
		[Theory]
		[InlineData(@"hello world how are you", @"hello world how are you")]
		[InlineData(@"I\'m fine   and how are you", @"I \'m fine and how are you")]
		[InlineData(@"wal*mart anime idol group wow", @"wal\*mart anime idol group wow")]
		[InlineData(@"you don\'t like cheese .. .. .. not sure if i trust you anymore ...", @"you don \'t like cheese ...... not sure if i trust you anymore ...")]
		[InlineData(@"the students\' council decided you die today", @"the students \' council decided you die today")]
		[InlineData(@"check out http://zombo.com the best website", @"check out the best website")]
		[InlineData(@"funny money :laala\: time yo", @"funny money \:laala \: time yo")]
		public void ParseSimpleText(string input, string output)
		{
			var options = new SentenceParserOptions();
			var target = new SentenceParser(Options.Create(options));

			var result = target.ParseIntoSentenceTexts(input);

			result.Should().ContainSingle().And.Contain(output);
		}
	}
}