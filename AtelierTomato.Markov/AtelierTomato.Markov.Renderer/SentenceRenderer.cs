
using System.Text.RegularExpressions;

namespace AtelierTomato.Markov.Renderer
{
	public class SentenceRenderer
	{
		private readonly Regex renderEmojiRegex = new Regex(@"e:((?:(?!:).)+):", RegexOptions.Compiled);
		private readonly Regex attachToPreviousWord = new Regex(@"(?: |^)([.\}\)\];:]|[?!,]+)(?: |$)", RegexOptions.Compiled);
		private readonly Regex attachToNextWord = new Regex(@"(?: |^)([#\[\{\(])(?: |$)", RegexOptions.Compiled);

		public string Render(string text)
		{
			text = RenderDetachCharacters(text);
			text = RenderEmojis(text);
			return text;
		}

		private string RenderDetachCharacters(string text)
		{
			text = attachToPreviousWord.Replace(text, m => m.Groups[1] + " ");
			text = attachToNextWord.Replace(text, m => " " + m.Groups[1]);

			return text.Trim();
		}

		private string RenderEmojis(string text) => renderEmojiRegex.Replace(text, m => m.Groups[1].Value);
	}
}
