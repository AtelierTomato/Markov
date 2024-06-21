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

		public IEnumerable<string> ParseIntoSentenceTexts(string text)
		{
			throw new NotImplementedException();
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
