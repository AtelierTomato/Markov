using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Model.ObjectOID;
using Discord.Commands;

namespace AtelierTomato.Markov.Service.Discord
{
	public class DiscordSentenceBuilder
	{
		public static IEnumerable<Sentence> Build(IEnumerable<string> sentenceTexts, ICommandContext context, string instance = "discord.com")
		{
			DiscordObjectOID OID = DiscordObjectOIDBuilder.Build(context, instance);
			AuthorOID Author = new(ServiceType.Discord, instance, context.User.Id.ToString());
			return sentenceTexts.Select((text, index) =>
				new Sentence(
					DiscordObjectOID.ForSentence(OID.Instance, OID.Server!.Value, OID.Category!.Value, OID.Channel!.Value, OID.Thread!.Value, OID.Message!.Value, index),
					Author,
					context.Message.CreatedAt,
					text
				)
			);
		}
	}
}
