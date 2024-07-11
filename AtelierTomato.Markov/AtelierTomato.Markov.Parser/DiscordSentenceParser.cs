using Discord;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace AtelierTomato.Markov.Parser
{
	public class DiscordSentenceParser(IOptions<SentenceParserOptions> options) : SentenceParser(options)
	{
		private readonly Regex replaceEmojiPattern = new Regex(@"(<:)(.+)(:[0-9]+>)", RegexOptions.Compiled);
		public IEnumerable<string> ParseIntoSentenceTexts(string input, IEnumerable<ITag> tags)
		{
			input = ReplaceTagEntities(input, tags);
			return ParseIntoSentenceTexts(input);
		}

		public override IEnumerable<string> ParseIntoSentenceTexts(string input)
		{
			IEnumerable<string> result = base.ParseIntoSentenceTexts(input);
			result = result.Select(ReplaceEmoji);
			return result;
		}

		private string ReplaceEmoji(string input)
		{
			return replaceEmojiPattern.Replace(input, m => "e:" + m.Groups[2].Value + ":");
		}

		private static string ReplaceTagEntities(string input, IEnumerable<ITag> tags)
		{
			foreach (ITag tag in tags.OrderByDescending(e => e.Index))
			{
				if (tag.Value is null || tag.Value.ToString() is null)
				{
					input = input.Remove(tag.Index, tag.Length).Insert(tag.Index, "unknown");
					continue;
				}

				switch (tag.Type)
				{
					case TagType.UserMention:
						input = input.Remove(tag.Index, tag.Length).Insert(tag.Index, ((IUser)tag.Value).Username);
						break;
					case TagType.RoleMention:
					case TagType.ChannelMention:
						input = input.Remove(tag.Index, tag.Length).Insert(tag.Index, tag.Value.ToString()!);
						break;
					case TagType.EveryoneMention:
						input = input.Remove(tag.Index, tag.Length).Insert(tag.Index, "everyone");
						break;
					case TagType.HereMention:
						input = input.Remove(tag.Index, tag.Length).Insert(tag.Index, "here");
						break;
					case TagType.Emoji:
						// Ignore, emojis will be processed later using the raw text, the tags are unecessary. This is just here to mark that.
						// Note: This MUST be done later, because we must do it after tokenization that splits off some formatting, in order to store emojis in the database.
						break;
					default:
						throw new NotSupportedException("Unknown tag type.");
				}
			}

			return input;
		}
	}
}
