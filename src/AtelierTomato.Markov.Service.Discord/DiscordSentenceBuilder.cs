using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Model.ObjectOID;
using Discord;

namespace AtelierTomato.Markov.Service.Discord
{
	public class DiscordSentenceBuilder
	{
		public static async Task<IEnumerable<Sentence>> Build(IGuild? guild, IChannel channel, ulong messageID, ulong userID, DateTimeOffset date, IEnumerable<string> sentenceTexts, string instance = "discord.com")
		{
			DiscordObjectOID OID = await DiscordObjectOIDBuilder.Build(guild, channel, messageID, instance);
			AuthorOID author = new(ServiceType.Discord, instance, userID.ToString());
			return sentenceTexts.Select(text =>
				new Sentence(
					OID.IncrementSentence(),
					author,
					date,
					text
				)
			);
		}
	}
}
