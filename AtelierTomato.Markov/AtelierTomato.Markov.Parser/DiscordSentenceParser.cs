using Discord;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace AtelierTomato.Markov.Parser
{
	public class DiscordSentenceParser(IOptions<SentenceParserOptions> options) : SentenceParser(options)
	{
		private readonly Regex replaceEmojiPattern = new Regex(@"(<:)(.+)(:[0-9]+>)", RegexOptions.Compiled);
		public IEnumerable<string> ParseIntoSentenceTexts(string text, IEnumerable<ITag> tags)
		{
			text = ReplaceTagEntities(text, tags);
			return ParseIntoSentenceTexts(text);
		}

		public override IEnumerable<string> ParseIntoSentenceTexts(string text)
		{
			IEnumerable<string> result = base.ParseIntoSentenceTexts(text);
			result = result.Select(ReplaceEmoji);
			return result;
		}

		private string ReplaceEmoji(string text)
		{
			return replaceEmojiPattern.Replace(text, m => "e:" + m.Groups[2].Value + ":");
		}

		private static string ReplaceTagEntities(string text, IEnumerable<ITag> tags)
		{
			foreach (ITag tag in tags.OrderByDescending(e => e.Index))
			{
				if (tag.Value is null || tag.Value.ToString() is null)
				{
					text = text.Remove(tag.Index, tag.Length).Insert(tag.Index, "unknown");
					continue;
				}

				switch (tag.Type)
				{
					case TagType.UserMention:
						text = text.Remove(tag.Index, tag.Length).Insert(tag.Index, ((IUser)tag.Value).Username);
						break;
					case TagType.RoleMention:
					case TagType.ChannelMention:
						text = text.Remove(tag.Index, tag.Length).Insert(tag.Index, tag.Value.ToString()!);
						break;
					case TagType.EveryoneMention:
						text = text.Remove(tag.Index, tag.Length).Insert(tag.Index, "everyone");
						break;
					case TagType.HereMention:
						text = text.Remove(tag.Index, tag.Length).Insert(tag.Index, "here");
						break;
					case TagType.Emoji:
						// Ignore, emojis will be processed later using the raw text, the tags are unnecessary. This is just here to mark that.
						// Note: This MUST be done later, because we must do it after tokenization that splits off some formatting, in order to store emojis in the database.
						break;
					default:
						throw new NotSupportedException("Unknown tag type.");
				}
			}

			return text;
		}
	}
}
