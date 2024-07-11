using Discord;

namespace AtelierTomato.Markov.Renderer
{
	public class DiscordSentenceRenderer : SentenceRenderer
	{
		public string Render(string text, IGuild currentGuild, IEnumerable<IGuild> allGuilds)
		{
			text = RenderEmojis(text, currentGuild, allGuilds);
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

		public string RenderEmojis(string text, IGuild currentGuild, IEnumerable<IGuild> allGuilds) => renderEmojiRegex.Replace(text, m =>
		{
			string emojiName = m.Groups[1].Value;
			GuildEmote? emoji = currentGuild.Emotes.FirstOrDefault(e => e.Name == emojiName);
			if (emoji is not null)
			{
				return emoji.ToString();
			} else
			{
				foreach (IGuild guild in allGuilds)
				{
					emoji = guild.Emotes.FirstOrDefault(e => e.Name == emojiName);
					if (emoji is not null)
					{
						return emoji.ToString();
					}
				}
			}
			return emojiName;
		});
	}
}
