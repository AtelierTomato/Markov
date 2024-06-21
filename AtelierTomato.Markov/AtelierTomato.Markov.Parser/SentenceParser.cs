using System.Text.RegularExpressions;

namespace AtelierTomato.Markov.Parser
{
	public class SentenceParser
	{
		private readonly Regex sentenceSeparatorPattern = new Regex(@"
((?:
[^.!?\r\n]               # neither sentence punctuation nor newlines
|                        # nor
\.\.+                    # ellipses
|                        # nor
(?<=\s[!?]*)[!?][!?]+    # questionexciteclusters
|                        # nor
[.!?]\w                  # punctuation with word after it
)+)                      # 1 or multiple
(\.|[.!?]+|\r?\n)", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
		private readonly Regex spaceifyEllipsesPattern = new Regex(@"(?<=[^\s.,?!])([.,?!])(?=[.,?!])", RegexOptions.Compiled);
		private readonly Regex ignoreCountPattern = new Regex(@"^[\p{P}]*$", RegexOptions.Compiled);
		private static readonly int minimumInputLength = 5; // TODO: make this a configurable option

		public IEnumerable<string> ParseIntoSentenceTexts(string text)
		{
			text = ProcessText(text);

			var sentences = SplitIntoSentences(text);
			sentences = sentences.Select(SpaceifyEllipses);

			var tokenizedSentences = sentences.Select(s => TokenizeProcessedSentence(s));
			// Remove sentences where the minimum length of the sentence (not including punctuation) is less than the minimum input length for a sentence.
			tokenizedSentences = tokenizedSentences.Where(s => s.Count(w => !ignoreCountPattern.IsMatch(w)) >= minimumInputLength);

			var sentenceTexts = tokenizedSentences.Select(ts => string.Join(" ", ts));

			return sentenceTexts;
		}
		public string ProcessText(string text)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Splits a message's content into its sentences.
		/// </summary>
		/// <param name="messageContent"></param>
		private IEnumerable<string> SplitIntoSentences(string messageContent)
		{
			return sentenceSeparatorPattern.Matches(messageContent).Select(m => m.Groups[1].Value.Trim() + " " + m.Groups[2].Value);
		}

		private string SpaceifyEllipses(string messageText)
		{
			return spaceifyEllipsesPattern.Replace(messageText,
				m => " " + m.Groups[1].Value);
		}
		private static IEnumerable<string> TokenizeProcessedSentence(string s) => s.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

	}
}
