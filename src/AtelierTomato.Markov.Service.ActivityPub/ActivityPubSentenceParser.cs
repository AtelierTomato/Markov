using System.Text.RegularExpressions;
using AtelierTomato.Markov.Core;
using AtelierTomato.Markov.Service.ActivityPub.Model;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Service.ActivityPub
{
	public class ActivityPubSentenceParser : SentenceParser
	{
		private readonly Regex parsedEmojiPattern = new(@"(?::)([^ ]+)(?: :)", RegexOptions.Compiled);
		public ActivityPubSentenceParser(IOptions<SentenceParserOptions> options) : base(options)
		{
		}

		public IEnumerable<string> ParseIntoSentenceTexts(IReadOnlyList<ActivityPubMention> mentions, IReadOnlyList<ActivityPubEmoji> emojis, string text, ActivityPubPoll? poll = null, string? spoilerText = null)
		{
			List<string> sentenceTexts = [];
			if (!string.IsNullOrWhiteSpace(spoilerText))
			{
				sentenceTexts.AddRange(ParsePartOfPost(mentions, emojis, spoilerText));
			}
			sentenceTexts.AddRange(ParsePartOfPost(mentions, emojis, text));
			if (poll is not null)
			{
				foreach (ActivityPubPollOption option in poll.Options)
				{
					sentenceTexts.AddRange(ParsePartOfPost(mentions, emojis, option.Title));
				}
			}

			return sentenceTexts;
		}

		public IEnumerable<string> ParsePartOfPost(IReadOnlyList<ActivityPubMention> mentions, IReadOnlyList<ActivityPubEmoji> emojis, string text)
		{
			if (mentions is not [])
			{
				foreach (var mention in mentions)
				{
					// It is possible for a username on Fediverse to use the base domain instead of the sub domain, so we need to handle this.
					List<string> splitDomain = new Uri(mention.Url).Host.Split('.').ToList();
					string domainRegex = "";
					for (int i = 0; i < splitDomain.Count; i++)
					{
						if (splitDomain.Count - i >= 3)
						{
							// Sub domains are optional.
							domainRegex += $"(?:{splitDomain[i]}.)?";
						}
						else if (splitDomain.Count - i == 2)
						{
							// Base domain is mandatory.
							domainRegex += $"{splitDomain[i]}.";
						}
						else
						{
							domainRegex += splitDomain[i];
						}
					}
					Regex pattern = new(@$"@{mention.Username}@(?:{domainRegex})");
					text = pattern.Replace(text, mention.Username);
				}
			}
			var sentenceTexts = base.ParseIntoSentenceTexts(text).ToList();
			// It's simplest to parse emojis into emoji format after doing the rest of the parsing.
			if (emojis is not [])
			{
				foreach (ActivityPubEmoji emoji in emojis)
				{
					Regex parsedEmojiPattern = new(@$":{emoji.Shortcode} :", RegexOptions.Compiled);
					for (int i = 0; i < sentenceTexts.Count; i++)
					{
						sentenceTexts[i] = parsedEmojiPattern.Replace(sentenceTexts[i], $"e:{emoji.Shortcode}:");
					}
				}
			}
			return sentenceTexts;
		}
	}
}
