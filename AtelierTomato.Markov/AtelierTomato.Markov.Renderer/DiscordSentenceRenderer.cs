using Discord;

namespace AtelierTomato.Markov.Renderer
{
	public class DiscordSentenceRenderer : SentenceRenderer
	{
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

		private string Escape(string text)
		{
			return text;
		}

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
