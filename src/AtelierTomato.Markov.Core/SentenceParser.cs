﻿using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Core
{
	/// <summary>
	/// A class that contains various methods for parsing message contents.
	/// </summary>
	public class SentenceParser
	{
		private readonly Regex sentenceSeparatorPattern = new(@"
((?:
[^.!?\r\n]               # neither sentence punctuation nor newlines
|                        # nor
\.\.+                    # ellipses
|                        # nor
(?<=\s[!?]*)[!?][!?]+    # questionexciteclusters
|                        # nor
[.!?]\w                  # punctuation with word after it
)+)                      # 1 or multiple
(\.|[.!?]+|$|\r?\n)", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
		private readonly Regex spaceifyEllipsesPattern = new(@"(?<=[^\s.,?!¿¡])([.,?!¿¡])(?=[.,?!¿¡])", RegexOptions.Compiled);
		private readonly Regex ignoreCountPattern = new(@"^[\p{P}]*$", RegexOptions.Compiled);
		private readonly Regex deleteLinkPattern = new(@"\S*://\S*", RegexOptions.Compiled);
		private readonly Regex whitespaceCleanerPattern = new(@"[^\S\r\n]+", RegexOptions.Compiled);
		private readonly Regex detachQuoteArrowsPattern = new(@"(?<=^|\n)(>)(?=\S)", RegexOptions.Compiled);
		private readonly Regex processHashtagPattern = new(@"(?<=^|\s)#(?=\S)", RegexOptions.Compiled);
		private readonly Regex processDetachCharactersPattern = new(@"
# first, the stuff we don't want to change: sentency characters surrounded by words and shit
# punctuation is not words [citation needed]
(?<!^|\s|[.?!]|[¿¡]|[()[\]{}«»‘“”]|""|[&]|(?<!,),(?!,)|-)
([()[\]{}«»]|""|[&]|(?<!,),(?!,)|-)
(?!$|\s|[.?!]|[¿¡]|[()[\]{}«»‘“”]|""|[&]|(?<!,),(?!,)|-)
|
# secondly, sentence characters again - this is according to that weird rexegg trick (http://www.rexegg.com/regex-best-trick.html)
([()[\]{}«»“”]|[¿¡]|""|[&]|(?<!,),(?!,)|-)
", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
		private readonly Regex processDetachFromPrecedingPattern = new(@"
(?<!\s|:|;)(:|;)(?=\s)
", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
		private readonly Regex processDetachFromSucceedingPattern = new(@"
(?<=\s)([.]{2,}|[,]{2,}|[?!]{2,})(?=\S)
", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
		private readonly Regex splitOffApostropheSequencesPattern = new(@"(?<=\S)(['’])", RegexOptions.Compiled);
		private readonly Regex splitOffDashSequencesPattern = new(@"(?<=\w)([-—])(?=\w)", RegexOptions.Compiled);
		private readonly Regex normalizeEllipsesPattern = new(@"(?<=[.,?!¿¡]) +(?=[.,?!¿¡])", RegexOptions.Compiled);

		private readonly SentenceParserOptions options;
		public SentenceParser(IOptions<SentenceParserOptions> options)
		{
			this.options = options.Value;
		}

		/// <summary>
		/// Parses the given text into multiple sentence texts.
		/// </summary>
		public virtual IEnumerable<string> ParseIntoSentenceTexts(string text)
		{
			text = ProcessText(text);

			var sentences = SplitIntoSentences(text);
			sentences = sentences.Select(SpaceifyEllipses);

			var tokenizedSentences = sentences.Select(s => TokenizeProcessedSentence(s));
			// Remove sentences where the minimum length of the sentence (not including punctuation) is less than the minimum input length for a sentence.
			tokenizedSentences = tokenizedSentences.Where(s => s.Count(w => !ignoreCountPattern.IsMatch(w)) >= options.MinimumInputLength);

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

			text = DetachQuoteArrows(text);

			text = ProcessHashtags(text);

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
		protected virtual IEnumerable<string> TokenizeProcessedSentence(string s) => s.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

		private string DeleteLinks(string text) => deleteLinkPattern.Replace(text, "");

		/// <summary>
		/// Cleans up whitespace. For now this means deleting duplicate whitespace characters (except line breaking ones).
		/// </summary>
		/// <param name="text"></param>
		private string CleanWhitespace(string text) => whitespaceCleanerPattern.Replace(text, " ");

		private string DetachQuoteArrows(string text) => detachQuoteArrowsPattern.Replace(text, m => m.Groups[1].Value + " ");

		private string ProcessHashtags(string text) => processHashtagPattern.Replace(text, "# ");

		private string ProcessDetachCharacters(string text) => processDetachCharactersPattern.Replace(text, m => m.Groups[1].Success ? m.Groups[1].Value : " " + m.Groups[2].Value + " ");

		private string ProcessDetachFromPreceding(string text) => processDetachFromPrecedingPattern.Replace(text, m => " " + m.Groups[1].Value);

		private string ProcessDetachFromSucceeding(string text) => processDetachFromSucceedingPattern.Replace(text, m => m.Groups[1].Value + " ");

		private string SplitOffApostropheSequences(string text) => splitOffApostropheSequencesPattern.Replace(text, m => " " + m.Groups[1]);

		private string SplitOffDashSequences(string text) => splitOffDashSequencesPattern.Replace(text, m => " " + m.Groups[1] + " ");

		private string NormalizeEllipses(string text) => normalizeEllipsesPattern.Replace(text, string.Empty);
	}
}
