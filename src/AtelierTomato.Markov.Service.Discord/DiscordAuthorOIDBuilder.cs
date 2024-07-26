using AtelierTomato.Markov.Model;
using Discord.Commands;

namespace AtelierTomato.Markov.Service.Discord
{
	public class DiscordAuthorOIDBuilder
	{
		public static AuthorOID Build(ICommandContext context, string instance = "discord.com") => new AuthorOID(ServiceType.Discord, instance, context.User.Id.ToString());
	}
}
