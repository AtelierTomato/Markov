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

		public async Task<IEnumerable<Sentence>> Build(IGuild? guild, IChannel channel, ulong messageID, ulong userID, DateTimeOffset date, IEnumerable<string> sentenceTexts, string instance = "discord.com")
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
	}
}
