using AtelierTomato.Markov.Database;
using AtelierTomato.Markov.Database.Model;

namespace AtelierTomato.Markov.Generation
{
	public class GenerateMarkovSentence
	{
		private readonly ISentenceAccess sentenceAccess;

		public GenerateMarkovSentence(ISentenceAccess sentenceAccess)
		{
			this.sentenceAccess = sentenceAccess;
		}

		public async Task<string> Generate()
		{
			Sentence sentence = await GetFirstSentence();
			if (sentence is null)
			{
				throw new Exception("Couldn't query any messages.");
			}

			return sentence.Text;
		}

		private async Task<Sentence?> GetFirstSentence() => await sentenceAccess.ReadSentence();
	}
}
