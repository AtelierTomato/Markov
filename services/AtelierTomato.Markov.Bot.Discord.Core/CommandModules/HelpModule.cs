using AtelierTomato.Markov.Core.Cooldown;
using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Service.Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Bot.Discord.Core.CommandModules
{
	public class HelpModule : ModuleBase<SocketCommandContext>
	{
		private readonly DiscordBotOptions options;
		private readonly DiscordSocketClient client;
		private readonly HelpContentBuilder helpContentBuilder;
		private readonly DiscordObjectOIDBuilder objectOIDBuilder;
		private readonly Cooldown cooldown;

		public HelpModule(IOptions<DiscordBotOptions> options, DiscordSocketClient client, HelpContentBuilder helpContentBuilder, DiscordObjectOIDBuilder objectOIDBuilder, Cooldown cooldown)
		{
			this.options = options.Value;
			this.client = client;
			this.helpContentBuilder = helpContentBuilder;
			this.objectOIDBuilder = objectOIDBuilder;
			this.cooldown = cooldown;
		}

		[Command("help")]
		[Alias("h")]
		public async Task Help(string parameter = "help")
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}

			await ReplyAsync(embed: helpContentBuilder.BuildForSubject(helpContentBuilder.ParseSubject(parameter)).Build());
		}
	}
}
