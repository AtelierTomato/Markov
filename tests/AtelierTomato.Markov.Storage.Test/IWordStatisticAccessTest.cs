using AtelierTomato.Markov.Model;
using Moq;

namespace AtelierTomato.Markov.Storage.Test
{
	public class IWordStatisticAccessTest
	{
		[Fact]
		public async Task WriteWordStatisticsFromStringTest()
		{
			string str = "i am but a mere sentence";
			IEnumerable<string> words = str.Split(' ').Distinct();
			var wordStatisticAccess = Mock.Of<IWordStatisticAccess>();
			Mock.Get(wordStatisticAccess).Setup(t => t.ReadWordStatisticRange(words)).ReturnsAsync(Enumerable.Empty<WordStatistic>()).Verifiable();
			Mock.Get(wordStatisticAccess).Setup(m => m.WriteWordStatisticsFromString(str)).CallBase();
			IEnumerable<WordStatistic> expectedWordStatistics =
			[
				new("i", 1),
				new("am", 1),
				new("but", 1),
				new("a", 1),
				new("mere", 1),
				new("sentence", 1)
			];

			await wordStatisticAccess.WriteWordStatisticsFromString(str);

			Mock.Get(wordStatisticAccess).Verify(m =>
				m.WriteWordStatisticRange(It.Is<IEnumerable<WordStatistic>>(stats =>
					stats.SequenceEqual(expectedWordStatistics))), Times.Once);
		}
	}
}
