
using System.Text.RegularExpressions;

namespace AtelierTomato.Markov.Renderer
{
	public class SentenceRenderer
	{
		private readonly Regex renderEmojiRegex = new Regex(@"(e:)((?:(?!:).)+)(:)", RegexOptions.Compiled);
		public string Render(string text)
		{
			text = RenderDetachCharacters(text);
			text = RenderEmojis(text);
			return text;
		}

		private string RenderDetachCharacters(string text)
		{
			return text;
		}

		private string RenderEmojis(string text) => renderEmojiRegex.Replace(text, m => m.Groups[2].Value);
	}
}
