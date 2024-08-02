using System.Text.RegularExpressions;
using AtelierTomato.Markov.Core;
using Discord;

namespace AtelierTomato.Markov.Service.Discord
{
	public class DiscordSentenceRenderer : SentenceRenderer
	{
		private readonly Regex escapeRegex = new(@"
(<a?:[^:]+:[0-9]+>)?	# Match to Discord emojis
(\p{So}					# Exclude OtherSymbol, like ⏸ and ✅
|\p{Cs}\p{Cs}			# OR two Surrogate
 \uD83C\p{Cs}			# with color-modifier, like 👍🏿 and 👍
						# (Hacky special case of Multibyte Character Set? It works.)
|\p{Cs}\p{Cs}			# OR two Surrogate, like 🔀 and 🧊
 (\p{Cf}				# followed by a Format
 \p{Cs}\p{Cs}))?		# and two Surrogate, like 👩‍💻 and 👨‍💻.
([^\d\sa-zA-Z])?		# Match to symbols, excluding digits, spaces, and letters
", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
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

		private string Escape(string text) => escapeRegex.Replace(text, m =>
		{
			if (m.Groups[1].Success && m.Groups[4].Success)
			{
				throw new NotImplementedException("Developers don't understand regex oops.");
			}
			else if (m.Groups[4].Success)
			{
				return "\\" + m.Groups[4].Value;
			}
			else if (m.Groups[1].Success)
			{
				return m.Groups[1].Value;
			}
			return m.Value;
		}).Trim();

		public string RenderEmojis(string text, IEnumerable<Emote> currentEmojis, IEnumerable<Emote> allEmojis) => RenderEmojiRegex.Replace(text, m =>
		{
			string emojiName = m.Groups[1].Value;
			Emote? emoji = currentEmojis.FirstOrDefault(e => e.Name == emojiName);
			if (emoji is not null)
			{
				return emoji.ToString();
			}
			else
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
