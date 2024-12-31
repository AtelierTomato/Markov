using AtelierTomato.Markov.Bot.Discord.Core.CommandModules.ParameterTypes;
using AtelierTomato.Markov.Core.Cooldown;
using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Service.Discord;
using AtelierTomato.Markov.Storage;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Bot.Discord.Core.CommandModules
{
	public class PermissionsModule : ModuleBase<SocketCommandContext>
	{
		private readonly IAuthorPermissionAccess authorPermissionAccess;
		private readonly IAuthorAccess authorAccess;
		private readonly DiscordBotOptions options;
		private readonly DiscordObjectOIDBuilder objectOIDBuilder;
		private readonly Cooldown cooldown;
		public PermissionsModule(IAuthorPermissionAccess authorPermissionAccess, IAuthorAccess authorAccess, IOptions<DiscordBotOptions> options, Cooldown cooldown, DiscordObjectOIDBuilder objectOIDBuilder)
		{
			this.authorPermissionAccess = authorPermissionAccess;
			this.authorAccess = authorAccess;
			this.options = options.Value;
			this.cooldown = cooldown;
			this.objectOIDBuilder = objectOIDBuilder;
		}

		[Command("optin")]
		[Alias("oi")]
		[Summary("Lets the user opt in to having their messages gathered by the bot.")]
		public async Task OptIn(DiscordLocationType from, DiscordLocationType to)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			if (from is DiscordLocationType.Message or DiscordLocationType.Sentence || to is DiscordLocationType.Message or DiscordLocationType.Sentence)
			{
				await ReplyAsync("you cannot opt in to or from the scope of an individual message or sentence!");
				return;
			}
			if (Context.Channel is not IThreadChannel && (from is DiscordLocationType.Thread || to is DiscordLocationType.Thread))
			{
				await ReplyAsync("you're not currently in a thread!");
				return;
			}
			var fromOID = location.ForLocationType(from);
			var toOID = location.ForLocationType(to);
			await authorPermissionAccess.WriteAuthorPermission(new AuthorPermission(authorOID, fromOID, toOID));
			var author = new Author(authorOID, Context.User.GlobalName ?? Context.User.Username);
			await authorAccess.WriteAuthor(author);
			await ReplyAsync($"""opted user "{author.Name}" into {options.BotName} from {from} to {to}!""");
		}
	}
}
