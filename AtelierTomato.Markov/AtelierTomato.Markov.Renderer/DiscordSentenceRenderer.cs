using Discord;
using System.Text.RegularExpressions;

namespace AtelierTomato.Markov.Renderer
{
	public class DiscordSentenceRenderer : SentenceRenderer
	{
		private readonly Regex escapeRegex = new Regex(@"(^|[^\d\sa-zA-Z])?([\d\sa-zA-Z]+)?((<a?:.+?(?=:[0-9]+>):[0-9]+>)+)?", RegexOptions.Compiled);
		public string Render(string text, IEnumerable<Emote> currentEmojis, IEnumerable<Emote> allEmojis)
		{
			text = RenderEmojis(text, currentEmojis, allEmojis);
			return Render(text);

		}

		public override string Render(string text)
		{
			text = base.Render(text);
			return Escape(text);
		}

		private string Escape(string text) => escapeRegex.Replace(" " + text, m =>
		{
			if (!(m.Groups[1].Value == string.Empty))
			{
				return "\\" + m.Groups[1].Value + m.Groups[2].Value + m.Groups[3].Value;
			} else
			{
				return m.Groups[1].Value + m.Groups[2].Value + m.Groups[3].Value;
			}
		}).Trim();

		public string RenderEmojis(string text, IEnumerable<Emote> currentEmojis, IEnumerable<Emote> allEmojis) => renderEmojiRegex.Replace(text, m =>
		{
			string emojiName = m.Groups[1].Value;
			Emote? emoji = currentEmojis.FirstOrDefault(e => e.Name == emojiName);
			if (emoji is not null)
			{
				return emoji.ToString();
			} else
			{
				emoji = allEmojis.FirstOrDefault(e => e.Name == emojiName);
				if (emoji is not null)
				{
					return emoji.ToString();
				}
			}
			return emojiName;
		});
	}
}
