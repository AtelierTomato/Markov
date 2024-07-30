using System.Text.RegularExpressions;

namespace AtelierTomato.Markov.Core
{
	public class SentenceRenderer
	{
		protected readonly Regex renderEmojiRegex = new Regex(@"e:([^:]+):", RegexOptions.Compiled); // Matches to "e:anyCharactersExceptColon:", our custom emoji storage format.
		private readonly Regex attachToPreviousWord = new Regex(@"(?: |^)([.\}\)\];:]|[?!,]+)(?: |$)", RegexOptions.Compiled);
		private readonly Regex attachToNextWord = new Regex(@"(?: |^)([#\[\{\(]|[¿¡]+)(?: |$)", RegexOptions.Compiled);
		private readonly Regex attachQuotes = new Regex(@"(?: |^)("")(?: |$)", RegexOptions.Compiled);
		private readonly Regex attachGuillemets = new Regex(@"(?: |^)([«»])(?: |$)", RegexOptions.Compiled);
		private readonly Regex attachContractions = new Regex(@"(?: )('\w+)", RegexOptions.Compiled);
		private readonly Regex attachPluralContractions = new Regex(@"(?: )(')(?: |$)", RegexOptions.Compiled);
		private readonly Regex attachDashes = new Regex(@"(\w)(?: )([-—])(?: )(\w)", RegexOptions.Compiled);
		private readonly Regex attachQuoteArrow = new Regex(@"(?:^)(>)(?: )", RegexOptions.Compiled);

		public virtual string Render(string text)
		{
			text = RenderDetachCharacters(text);
			text = RenderEmojis(text);
			return text;
		}

		private string RenderDetachCharacters(string text)
		{
			text = attachToPreviousWord.Replace(text, m => m.Groups[1] + " ");
			text = attachToNextWord.Replace(text, m => " " + m.Groups[1]);
			bool opening = false;
			text = attachQuotes.Replace(text, m =>
			{
				opening = !opening;
				if (opening) { return " " + m.Groups[1]; } else { return m.Groups[1] + " "; }
			});
			opening = false;
			text = attachGuillemets.Replace(text, m =>
			{
				opening = !opening;
				if (opening) { return " " + m.Groups[1]; } else { return m.Groups[1] + " "; }
			});
			text = attachContractions.Replace(text, m => "" + m.Groups[1]);
			text = attachPluralContractions.Replace(text, m => m.Groups[1] + " ");
			text = attachDashes.Replace(text, m => m.Groups[1].Value + m.Groups[2] + m.Groups[3]);
			text = attachQuoteArrow.Replace(text, m => m.Groups[1] + "");

			return text.Trim();
		}

		private string RenderEmojis(string text) => renderEmojiRegex.Replace(text, m => m.Groups[1].Value);
	}
}
