using AtelierTomato.Markov.Core.Generation;
using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Storage;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

namespace AtelierTomato.Markov.Core.Test
{
	public class KeywordTests
	{
		[Fact]
		public async Task checkIgnoreKeywords()
		{
			var options = Options.Create(new KeywordOptions { });
			var wordStatisticAccess = Mock.Of<IWordStatisticAccess>();
			string str = "bukkibot";
			List<string> split = str.Split(' ').ToList();
			Mock.Get(wordStatisticAccess)
				.Setup(l => l.ReadWordStatisticRange(split))
				.Returns(Task.FromResult<IEnumerable<WordStatistic>>([new WordStatistic("bukkibot", 14030)]));
			var keywordProvider = new KeywordProvider(wordStatisticAccess, options);
			var result = await keywordProvider.Find(str);
			result.Should().NotBeEquivalentTo("bukkibot");
		}
	}
}
