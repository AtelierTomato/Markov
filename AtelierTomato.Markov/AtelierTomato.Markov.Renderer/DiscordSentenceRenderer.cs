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

		public string RenderEmojis(string text, IGuild currentGuild, IEnumerable<IGuild> allGuilds)
		{
			return text;
		}
	}
}
