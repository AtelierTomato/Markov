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
			try
			{
				await ReplyAsync(embed: helpContentBuilder.BuildForSubject(helpContentBuilder.ParseSubject(parameter)).Build());
			}
			catch
			{
				await ReplyAsync($"there is no command named \"{parameter}\"");
			}
		}

		[Command("gettingstarted")]
		[Alias("gs")]
		public async Task GettingStarted([Remainder] string? _ = null)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			await ReplyAsync(embed: helpContentBuilder.BuildForSubject(HelpSubject.GettingStarted).Build());
		}

		[Command("faq")]
		public async Task FAQ([Remainder] string? _ = null)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			await ReplyAsync(embed: helpContentBuilder.BuildForSubject(HelpSubject.FAQ).Build());
		}

		[Command("retort")]
		[Alias("authorretortconfig", "arc")]
		public async Task Retort([Remainder] string? _ = null)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			await ReplyAsync(embed: helpContentBuilder.BuildForSubject(HelpSubject.Retort).Build());
		}

		[Command("querysentences")]
		[Alias("qs")]
		public async Task QuerySentences([Remainder] string? _ = null)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			await ReplyAsync(embed: helpContentBuilder.BuildForSubject(HelpSubject.QuerySentences).Build());
		}

		[Command("deletesentences")]
		[Alias("ds")]
		public async Task DeleteSentences([Remainder] string? _ = null)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			await ReplyAsync(embed: helpContentBuilder.BuildForSubject(HelpSubject.DeleteSentences).Build());
		}
	}
}
