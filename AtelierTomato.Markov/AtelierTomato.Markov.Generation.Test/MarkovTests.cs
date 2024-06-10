using AtelierTomato.Markov.Data;
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

		[Fact]
		public async Task DatabaseFailure()
		{
			var sentenceAccess = Mock.Of<ISentenceAccess>();


			Func<Task> action = async () =>
			{
				var target = new GenerateMarkovSentence(sentenceAccess);

				var result = await target.Generate();

			};
			await action.Should().ThrowAsync<Exception>().WithMessage("Couldn't query any messages.");
		}
	}
}