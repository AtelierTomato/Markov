﻿using AtelierTomato.Markov.Data;
using AtelierTomato.Markov.Data.Model;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Generation
{
	public class MarkovChain(ISentenceAccess sentenceAccess, IOptions<MarkovGenerationOptions> options)
	{
		private readonly ISentenceAccess sentenceAccess = sentenceAccess;
		private readonly MarkovGenerationOptions options = options.Value;
		private static readonly Random random = new();

		public async Task<string> Generate(SentenceFilter filter, string? firstWord = null)
		{
			// Tracks the IDs of previously used sentences so that we don't recreate an existing sentence or ping pong between two sentences.
			List<IObjectOID> prevIDs = [];
			Sentence? sentence;
			if (firstWord is null)
			{
				sentence = await GetFirstSentence(filter) ?? throw new Exception("Couldn't query any messages.");
				firstWord = sentence.Text.Substring(0, sentence.Text.IndexOf(' '));
				prevIDs.Add(sentence.OID);
			}
			List<string> tokenizedSentence = [firstWord];
			List<string> prevList = [firstWord];

			var rerolls = 0;

			// Gets reset whenever the list of previous words has to be modified or the randomized copypasta killer takes action
			var currentPastaLength = 0;

			while (tokenizedSentence.Count < options.maximumOutputLength)
			{
				if (KillCopypasta(currentPastaLength))
				{
					prevList.RemoveRange(0, prevList.Count - 1);
					currentPastaLength = 0;
				}

				sentence = await GetNextSentence(prevList, prevIDs, filter);
				if (sentence is not null)
				{
					prevIDs.Add(sentence.OID);
					currentPastaLength++;

					var spacedText = ' ' + sentence.Text + ' ';

					// Keep only the parts of the found sentence after the last occurrence of prevlist
					var prevListLocation = spacedText.LastIndexOf(' ' + string.Join(' ', prevList) + ' ', StringComparison.CurrentCultureIgnoreCase);

					if (prevListLocation < 0)
					{
						// TODO: This should be logged, it should not be possible for the prevList to not be matched in the found sentence.
						prevListLocation = 0;
					}

					var sentenceWithoutPrevList = spacedText
						.Substring(prevListLocation)
						.Split(' ', StringSplitOptions.RemoveEmptyEntries)
						.Skip(prevList.Count);

					var nextWord = sentenceWithoutPrevList.FirstOrDefault();

					if (nextWord is not null)
					{
						// Get just the next word after the last instance of the prevList, add to both tS and pL.
						tokenizedSentence.Add(nextWord);
						prevList.Add(nextWord);

						// Trim prevList if it gets too long.
						if (prevList.Count > options.maximumPrevListLength)
						{
							prevList.RemoveAt(0);
						}
					} else
					{
						// Rerolls a few times if it hits the end of the sentence, allowing formation of longer sentences with the tradeoff of taking longer to generate
						if (rerolls > options.maximumMarkovRerolls || tokenizedSentence.Count > options.maximumLengthForReroll)
						{
							return string.Join(' ', tokenizedSentence);
						}
						rerolls++;
						currentPastaLength = 0;
					}
				} else if (prevList.Count != 0)
				{
					// If the prevList can't be used to match to anything, remove the first word from the prevList.
					prevList.RemoveAt(0);
					currentPastaLength = 0;
				} else
				{
					// If the prevList reaches 0, that means that there's only one instance of the word in the database/query, so output the message.
					return string.Join(' ', tokenizedSentence);
				}
			}

			return string.Join(' ', tokenizedSentence);
		}

		private bool KillCopypasta(int currentPastaLength)
		{
			var discardThreshold = 1 - Math.Pow(1 - options.copyPastaKillingProbability, currentPastaLength);
			return random.NextDouble() < discardThreshold;
		}

		private async Task<Sentence?> GetNextSentence(List<string> prevList, List<IObjectOID> previousIDs, SentenceFilter filter) => await sentenceAccess.ReadNextRandomSentence(prevList, previousIDs, filter);

		private async Task<Sentence?> GetFirstSentence(SentenceFilter filter) => await sentenceAccess.ReadRandomSentence(filter);
	}
}