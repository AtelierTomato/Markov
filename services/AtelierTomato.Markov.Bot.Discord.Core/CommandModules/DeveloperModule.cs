using System.Text;
using ConsoleTableExt;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Bot.Discord.Core.CommandModules
{
	public class DeveloperModule : ModuleBase<SocketCommandContext>
	{
		private readonly DiscordBotOptions options;
		private readonly DiscordSocketClient client;

		public DeveloperModule(IOptions<DiscordBotOptions> options, DiscordSocketClient client)
		{
			this.options = options.Value;
			this.client = client;
		}

		[Command("say")]
		public async Task Say([Remainder] string input)
		{
			if (options.DeveloperIDs.Contains(Context.User.Id))
			{
				await ReplyAsync(input);
			}
			else
			{
				await ReplyAsync("no, i don't think i will");
			}
		}

		[Command("announce")]
		public async Task Announce([Remainder] string input)
		{
			if (options.DeveloperIDs.Contains(Context.User.Id))
			{
				foreach (var guild in client.Guilds)
				{
					foreach (var channel in guild.TextChannels.Where(tc => tc is not IThreadChannel and not IVoiceChannel).OrderBy(x => x.Position))
					{
						try
						{
							using (channel.EnterTypingState()) await channel.SendMessageAsync(text: input);
							break;
						}
						catch (HttpException e) when (e.DiscordCode == DiscordErrorCode.InsufficientPermissions || e.DiscordCode == DiscordErrorCode.MissingPermissions)
						{
							// This is fine, we're looking for the first channel we can actually post in
						}
					}
				}
			}
			else
			{
				await ReplyAsync("no, i don't think i will");
			}
		}

		[Command("leave")]
		public async Task Leave(ulong? serverID = null)
		{
			if (!options.DeveloperIDs.Contains(Context.User.Id))
			{
				await ReplyAsync(message: "sorry, only the bot developers are allowed to use this command!");
				return;
			}
			SocketGuild guild;
			if (serverID is null)
			{
				guild = Context.Guild;
			}
			else
			{
				guild = client.GetGuild(serverID.Value);
			}
			if (guild is null)
			{
				await ReplyAsync(message: "sorry, that's not a valid server! or else, i'm not in it!!!!");
				return;
			}

			await ReplyAsync(message: $"leaving the server \"{guild.Name}\"!");
			await guild.LeaveAsync();
		}

		[Command("servers")]
		[Alias("serverlist", "sl")]
		public async Task ServerList([Remainder] string? _ = null)
		{
			if (!options.DeveloperIDs.Contains(Context.User.Id))
			{
				await ReplyAsync(message: "sorry, only the bot developers are allowed to use this command!");
				return;
			}
			var listBuilder = ConsoleTableBuilder
				.From(client.Guilds.Select(g => new GuildInfo(g.Id, g.Name)).ToList())
				.WithFormat(ConsoleTableBuilderFormat.Minimal)
				.Export();
			await ReplyAsync(message: "dming you a list of servers i'm in!");
			await Context.User.SendMessageAsync(listBuilder.Insert(0, "```").Append("```").ToString().TrimEnd());
		}

		[Command("channels")]
		[Alias("channellist", "cl")]
		public async Task ServerChannelInfo([Remainder] string? _ = null)
		{
			if (!options.DeveloperIDs.Contains(Context.User.Id))
			{
				await ReplyAsync(message: "sorry, only the bot developers are allowed to use this command!");
				return;
			}

			var channels = Context.Guild.TextChannels
				.OrderBy(channel => channel.Position)
				.Select(channel => new ChannelInfo(channel.Id, channel.Name, channel.GetType().Name));

			var table = ConsoleTableBuilder
				.From(channels.ToList())
				.WithFormat(ConsoleTableBuilderFormat.Minimal)
				.Export();
			var channelInfoText = table.Insert(0, $"All the text channels in the server {Context.Guild.Name} ({Context.Guild.Id}):{Environment.NewLine}").ToString();
			var stream = new MemoryStream(Encoding.UTF8.GetBytes(channelInfoText));
			await ReplyAsync(message: "sending you the output in DMs!!");
			await Context.User.SendFileAsync(stream, "query results.txt");
		}
	}

	public record GuildInfo(ulong Id, string Name);
	public record ChannelInfo(ulong Id, string Name, string Type);
}
