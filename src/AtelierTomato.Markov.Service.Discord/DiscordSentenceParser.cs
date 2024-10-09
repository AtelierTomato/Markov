using System.Globalization;
using System.Text.RegularExpressions;
using AtelierTomato.Markov.Core;
using AtelierTomato.Markov.Service.Discord.MarkdigExtensions;
using Discord;
using Humanizer;
using Humanizer.Localisation;
using Markdig;
using Markdig.Extensions.EmphasisExtras;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Service.Discord
{
	public class DiscordSentenceParser : SentenceParser
	{
		private readonly Regex codeBlockPattern = new(@"(```)((?:(?!```)[\s\S])+|)(```)", RegexOptions.Compiled);
		private readonly Regex inlineCodeBlockPattern = new(@"((^|[^\\])`([^`]|\\`)*[^\\]`|(^|[^\\])``)", RegexOptions.Compiled);
		private readonly Regex escapeQuoteArrowPattern = new(@"(?<=^|\n)(>)(?=\S)(?!>)", RegexOptions.Compiled);
		private readonly Regex replaceEmojiPattern = new(@"<a?:([^:]+):[0-9]+>", RegexOptions.Compiled);
		private readonly Regex removeNegativeHeaderPattern = new(@"(?<=^|\n)(-# )(?=\S)", RegexOptions.Compiled);
		private readonly Regex replaceTimestampPattern = new(@"<t:(\d+):([RDdTtFf])>", RegexOptions.Compiled);

		private readonly DiscordSentenceParserOptions discordOptions;
		private readonly MarkdownPipeline pipeline;
		public DiscordSentenceParser(IOptions<SentenceParserOptions> options, IOptions<DiscordSentenceParserOptions> discordOptions) : base(options)
		{
			this.discordOptions = discordOptions.Value;
			pipeline = new MarkdownPipelineBuilder().UseEmphasisExtras(EmphasisExtraOptions.Strikethrough).Use<SpoilerExtension>().Build();
		}

		public IEnumerable<string> ParseIntoSentenceTexts(string text, IEnumerable<ITag> tags, DateTimeOffset dateSent)
		{
			text = ReplaceTagEntities(text, tags);
			text = ReplaceTimestamps(text, dateSent);
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
			text = RemoveNegativeHeader(text);
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
			while (discordOptions.RemovePrefixes.FirstOrDefault(p => text.StartsWith(p, StringComparison.InvariantCulture)) is not null and var prefix)
			{
				text = text.Remove(0, prefix.Length).TrimStart();
			}
			return text;
		}
		private string DeleteCodeBlocks(string text) => codeBlockPattern.Replace(text, Environment.NewLine);
		private string DeleteInlineCodeBlocks(string text) => inlineCodeBlockPattern.Replace(text, " ");
		private string EscapeQuoteArrow(string text) => escapeQuoteArrowPattern.Replace(text, m => "\\" + m.Groups[1].Value);
		private string RemoveNegativeHeader(string text) => removeNegativeHeaderPattern.Replace(text, m => "");
		private string ReplaceTimestamps(string text, DateTimeOffset dateSent) => replaceTimestampPattern.Replace(text, m =>
		{
			if (!long.TryParse(m.Groups[1].Value, out long dateNumber))
			{
				throw new ArgumentException("Could not parse a long value from the input.", nameof(text));
			}
			var date = DateTimeOffset.FromUnixTimeSeconds(dateNumber).UtcDateTime;
			TimeSpan timeSpan = date - dateSent;
			return m.Groups[2].Value switch
			{
				"R" => date > dateSent
					? "in " + timeSpan.Humanize(maxUnit: TimeUnit.Year, minUnit: TimeUnit.Second)
					: timeSpan.Humanize(maxUnit: TimeUnit.Year, minUnit: TimeUnit.Second) + " ago",
				"D" => date.ToString("MMMM d, yyyy", CultureInfo.InvariantCulture),
				"d" => date.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
				"T" => date.ToString("h:mm:ss tt", CultureInfo.InvariantCulture),
				"t" => date.ToString("h:mm tt", CultureInfo.InvariantCulture),
				"F" => date.ToString("dddd, MMMM d, yyyy h:mm tt", CultureInfo.InvariantCulture),
				"f" => date.ToString("MMMM d, yyyy h:mm tt", CultureInfo.InvariantCulture),
				_ => throw new ArgumentException("Could not parse a valid display format from the input.", nameof(text))
			};
		});
		private string ReplaceEmoji(string text) => replaceEmojiPattern.Replace(text, m => "e:" + m.Groups[1].Value + ":");
		protected override IEnumerable<string> TokenizeProcessedSentence(string s) => s.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Where(w => !w.Contains("discord.gg"));
	}
}
