using AtelierTomato.Markov.Parser.MarkdigExtensions;
using Discord;
using Markdig;
using Markdig.Extensions.EmphasisExtras;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace AtelierTomato.Markov.Parser
{
	public class DiscordSentenceParser : SentenceParser
	{
		private readonly Regex codeBlockPattern = new Regex(@"(```)((?:(?!```)[\s\S])+|)(```)", RegexOptions.Compiled);
		private readonly Regex inlineCodeBlockPattern = new Regex(@"((^|[^\\])`([^`]|\\`)*[^\\]`|(^|[^\\])``)", RegexOptions.Compiled);
		private readonly Regex escapeQuoteArrowPattern = new Regex(@"(?<=^|\n)(>)(?=\S)(?!>)", RegexOptions.Compiled);
		private readonly Regex replaceEmojiPattern = new Regex(@"<a?:([^:]+):[0-9]+>", RegexOptions.Compiled);

		private readonly DiscordSentenceParserOptions discordOptions;
		private readonly MarkdownPipeline pipeline;
		public DiscordSentenceParser(IOptions<SentenceParserOptions> options, IOptions<DiscordSentenceParserOptions> discordOptions) : base(options)
		{
			this.discordOptions = discordOptions.Value;
			pipeline = new MarkdownPipelineBuilder().UseEmphasisExtras(EmphasisExtraOptions.Strikethrough).Use<SpoilerExtension>().Build();
		}

		public IEnumerable<string> ParseIntoSentenceTexts(string text, IEnumerable<ITag> tags)
		{
			text = ReplaceTagEntities(text, tags);
			return ParseIntoSentenceTexts(text);
		}

		public override IEnumerable<string> ParseIntoSentenceTexts(string text)
		{
			text = ReplacePrefixes(text);
			text = RemovePrefixes(text);
			// If this message is prefixed with a prefix we want to ignore, cancel processing and instead return an empty list.
			if (discordOptions.IgnorePrefixes.Any(text.StartsWith))
			{
				return [];
			}
			text = DeleteCodeBlocks(text);
			text = DeleteInlineCodeBlocks(text);
			text = EscapeQuoteArrow(text);
			text = Markdown.ToPlainText(text, pipeline);

			return base.ParseIntoSentenceTexts(text).Select(ReplaceEmoji);
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

		private string ReplacePrefixes(string text)
		{
			foreach (ReplacePrefix replacePrefix in discordOptions.ReplacePrefixes)
			{
				text = text.Replace(replacePrefix.From, replacePrefix.To);
			}
			return text;
		}
		private string RemovePrefixes(string text)
		{
			while (discordOptions.RemovePrefixes.FirstOrDefault(p => text.StartsWith(p)) is not null and var prefix)
			{
				text = text.Remove(0, prefix.Length).TrimStart();
			}
			return text;
		}
		private string DeleteCodeBlocks(string text) => codeBlockPattern.Replace(text, Environment.NewLine);
		private string DeleteInlineCodeBlocks(string text) => inlineCodeBlockPattern.Replace(text, " ");
		private string EscapeQuoteArrow(string text) => escapeQuoteArrowPattern.Replace(text, m => "\\" + m.Groups[1].Value);
		private string ReplaceEmoji(string text) => replaceEmojiPattern.Replace(text, m => "e:" + m.Groups[1].Value + ":");
		protected override IEnumerable<string> TokenizeProcessedSentence(string s) => s.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Where(w => !w.Contains("discord.gg"));
	}
}
