using AtelierTomato.Markov.Core;
using AtelierTomato.Markov.Service.ActivityPub.Model;

namespace AtelierTomato.Markov.Service.ActivityPub
{
	public class ActivityPubSentenceRenderer : SentenceRenderer
	{
		public string Render(string text, IEnumerable<ActivityPubEmoji> emojis)
		{
			text = RenderEmojis(text, emojis);
			return base.Render(text);
		}
		public string RenderEmojis(string text, IEnumerable<ActivityPubEmoji> emojis) => RenderEmojiRegex.Replace(text, m =>
		{
			string emojiName = m.Groups[1].Value;
			ActivityPubEmoji? emoji = emojis.FirstOrDefault(e => e.Shortcode == emojiName);
			if (emoji is not null)
			{
				return $":{emoji.Shortcode}:";
			}
			else
			{
				return emojiName;
			}
		});
	}
}
