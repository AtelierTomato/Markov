using System.Globalization;
using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Model.ObjectOID;
using Discord;

namespace AtelierTomato.Markov.Service.Discord
{
	public class DiscordSentenceBuilder
	{
		private readonly DiscordObjectOIDBuilder discordObjectOIDBuilder;
		public DiscordSentenceBuilder(DiscordObjectOIDBuilder discordObjectOIDBuilder)
		{
			this.discordObjectOIDBuilder = discordObjectOIDBuilder;
		}

		public async Task<IEnumerable<Sentence>> Build(IGuild guild, IGuildChannel channel, ulong messageID, ulong userID, DateTimeOffset date, IEnumerable<string> sentenceTexts, string instance = "discord.com")
		{
			DiscordObjectOID OID = (await discordObjectOIDBuilder.Build(guild, channel, instance)).WithMessage(messageID);
			AuthorOID author = new(ServiceType.Discord, instance, userID.ToString(CultureInfo.InvariantCulture));
			return sentenceTexts.Select((text, index) =>
				new Sentence(
					OID.WithSentence(index),
					author,
					date,
					text
				)
			);
		}
		public static IEnumerable<Sentence> Build(IChannel channel, ulong messageID, ulong userID, DateTimeOffset date, IEnumerable<string> sentenceTexts, string instance = "discord.com")
		{
			if (channel is IGuildChannel)
			{
				throw new ArgumentException($"An {nameof(IGuildChannel)} was passed while this function is only used for non-guild channels. Please use the correct overload.", nameof(channel));
			}
			DiscordObjectOID OID = DiscordObjectOID.ForMessage(instance, 0, 0, channel.Id, 0, messageID);
			AuthorOID author = new(ServiceType.Discord, instance, userID.ToString(CultureInfo.InvariantCulture));
			return sentenceTexts.Select((text, index) =>
				new Sentence(
					OID.WithSentence(index),
					author,
					date,
					text
				)
			);
		}
	}
}
