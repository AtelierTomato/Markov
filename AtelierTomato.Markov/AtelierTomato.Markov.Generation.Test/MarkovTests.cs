using AtelierTomato.Markov.Data;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

namespace AtelierTomato.Markov.Generation.Test
{
	public class MarkovTests
	{
		[Fact]
		public void ConstructorTest()
		{
			var options = Options.Create(new MarkovGenerationOptions { });
			var sentenceAccess = Mock.Of<ISentenceAccess>();

			var target = new GenerateMarkovSentence(sentenceAccess, options);

			target.Should().NotBeNull().And.BeOfType<GenerateMarkovSentence>();
		}

		[Fact]
		public async Task DatabaseFailure()
		{
			var options = Options.Create(new MarkovGenerationOptions { });
			var sentenceAccess = Mock.Of<ISentenceAccess>();
			var filter = Mock.Of<ISentenceFilter>();


			Func<Task> action = async () =>
			{
				var target = new GenerateMarkovSentence(sentenceAccess, options);

				var result = await target.Generate(filter);

			};
			await action.Should().ThrowAsync<Exception>().WithMessage("Couldn't query any messages.");
		}
	}
}