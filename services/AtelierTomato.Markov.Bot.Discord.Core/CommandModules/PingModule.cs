using Discord.Commands;

namespace AtelierTomato.Markov.Bot.Discord.Core.CommandModules
{
	public class PingModule : ModuleBase<SocketCommandContext>
	{
		[Command("ping")]
		public async Task Ping([Remainder] string? _ = null)
		{
			await ReplyAsync(message: "pong!!");
		}
	}
}
