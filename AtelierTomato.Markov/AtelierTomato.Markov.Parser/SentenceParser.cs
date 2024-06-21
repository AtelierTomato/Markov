using System.Text.RegularExpressions;

namespace AtelierTomato.Markov.Parser
{
	/// <summary>
	/// A class that contains various methods for parsing message contents.
	/// </summary>
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
		private readonly Regex deleteLinkPattern = new Regex(@"\S*://\S*", RegexOptions.Compiled);
		private readonly Regex whitespaceCleanerPattern = new Regex(@"[^\S\r\n]+", RegexOptions.Compiled);
		private readonly Regex escapeAndDetachQuoteArrowsPattern = new Regex(@"(?<=^|\n)(>)(?=\S)", RegexOptions.Compiled);
		private readonly Regex processHashtagPattern = new Regex(@"(?<=^|\s)#(?=\S)", RegexOptions.Compiled);
		private readonly Regex unescapedPattern = new Regex(@"(
(?<!\\)\\(?![\\~|_*>`:]) # lone \
|
(?<!\\)\* # * with no \
|
(?<!\\)_ # _ with no \
| 
(?<!\\)\| # | with no \
|
(?<!\\)~ # ~ with no \
|
(?<!\\)> # > with no \
|
(?<!\\)` # ` with no \
|
(?<!\\): # : with no \
|
(?<!\\)' # ' with no \
|
(?<!\\)"" # "" with no \
)", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
		private readonly Regex processDetachCharactersPattern = new Regex(@"
# first, the stuff we don't want to change: sentency characters surrounded by words and shit
# punctuation is not words [citation needed]
(?<!^|\s|[.?!]|[()[\]{}]|\\""|[&]|(?<!,),(?!,)|-)
([()[\]{}]|\\""|[&]|(?<!,),(?!,)|-)
(?!$|\s|[.?!]|[()[\]{}]|\\""|[&]|(?<!,),(?!,)|-)
|
# secondly, sentence characters again - this is according to that weird rexegg trick (http://www.rexegg.com/regex-best-trick.html)
([()[\]{}]|\\""|[&]|(?<!,),(?!,)|-)
", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
		private readonly Regex processDetachFromPrecedingPattern = new Regex(@"
(?<!\s|\\:|;)(\\:|;)(?=\s)
", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
		private readonly Regex processDetachFromSucceedingPattern = new Regex(@"
(?<=\s)([.]{2,}|[,]{2,}|[?!]{2,})(?=\S)
", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
		private readonly Regex splitOffApostropheSequencesPattern = new Regex(@"(?<=\S)(?:\\')", RegexOptions.Compiled);
		private readonly Regex splitOffDashSequencesPattern = new Regex(@"(?<=\S)(?:-)(?=\S)", RegexOptions.Compiled);
		private readonly Regex normalizeEllipsesPattern = new Regex(@"(?<=[.,?!]) (?=[.,?!])", RegexOptions.Compiled);

		private static readonly int minimumInputLength = 5; // TODO: make this a configurable option

		/// <summary>
		/// Parses the given text into multiple sentence texts.
		/// </summary>
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

		/// <summary>
		/// Processes text parts such as punctuation and prepares spacing for tokenization.
		/// </summary>
		public string ProcessText(string text)
		{
			text = DeleteLinks(text);
			text = CleanWhitespace(text);

			text = EscapeAndDetachQuoteArrows(text);

			text = ProcessHashtags(text);

			text = EscapeUnescapeds(text);

			text = ProcessDetachCharacters(text);
			text = ProcessDetachFromPreceding(text);
			text = ProcessDetachFromSucceeding(text);

			text = SplitOffApostropheSequences(text);
			text = SplitOffDashSequences(text);

			text = NormalizeEllipses(text);
			return text;
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

		private string DeleteLinks(string text) => deleteLinkPattern.Replace(text, "");

		/// <summary>
		/// Cleans up whitespace. For now this means deleting duplicate whitespace characters (except line breaking ones).
		/// </summary>
		/// <param name="text"></param>
		private string CleanWhitespace(string text) => whitespaceCleanerPattern.Replace(text, " ");

		private string EscapeAndDetachQuoteArrows(string messageText) => escapeAndDetachQuoteArrowsPattern.Replace(messageText, m => "\\" + m.Groups[1].Value + " ");

		private string ProcessHashtags(string messageText) => processHashtagPattern.Replace(messageText, "# ");

		/// <summary>
		/// Escapes unescaped characters.
		/// </summary>
		private string EscapeUnescapeds(string text) => unescapedPattern.Replace(text, m => "\\" + m.Value);

		private string ProcessDetachCharacters(string messageText) => processDetachCharactersPattern.Replace(messageText, m => m.Groups[1].Success ? m.Groups[1].Value : " " + m.Groups[2].Value + " ");

		private string ProcessDetachFromPreceding(string messageText) => processDetachFromPrecedingPattern.Replace(messageText, m => " " + m.Groups[1].Value);

		private string ProcessDetachFromSucceeding(string messageText) => processDetachFromSucceedingPattern.Replace(messageText, m => m.Groups[1].Value + " ");

		private string SplitOffApostropheSequences(string messageText) => splitOffApostropheSequencesPattern.Replace(messageText, " \\'");

		private string SplitOffDashSequences(string messageText) => splitOffDashSequencesPattern.Replace(messageText, "- ");

		private string NormalizeEllipses(string messageText) => normalizeEllipsesPattern.Replace(messageText, string.Empty);
	}
}
