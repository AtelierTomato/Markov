using AtelierTomato.Markov.Database;
using FluentAssertions;
using Moq;

namespace AtelierTomato.Markov.Generation.Test
{
	public class MarkovTests
	{
		[Fact]
		public void ConstructorTest()
		{
			var sentenceAccess = Mock.Of<ISentenceAccess>();

			var target = new GenerateMarkovSentence(sentenceAccess);

			target.Should().NotBeNull().And.BeOfType<GenerateMarkovSentence>();
		}
	}
}