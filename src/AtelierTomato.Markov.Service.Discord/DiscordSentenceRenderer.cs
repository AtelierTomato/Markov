﻿using System.Text.RegularExpressions;
using AtelierTomato.Markov.Core;
using Discord;

namespace AtelierTomato.Markov.Service.Discord
{
	public class DiscordSentenceRenderer : SentenceRenderer
	{
		private readonly Regex escapeRegex = new(@"
(<a?:[^:]+:[0-9]+>)?	# Match to Discord emojis
([*_~`>#|\\[\]()])?			# Match to symbols used in Markdown on Discord
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
			if (m.Groups[1].Success && m.Groups[2].Success)
			{
				throw new NotImplementedException("Developers don't understand regex oops.");
			}
			else if (m.Groups[2].Success)
			{
				return "\\" + m.Groups[2].Value;
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
