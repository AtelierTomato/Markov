using AtelierTomato.Markov.Model.ObjectOID;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace AtelierTomato.Markov.Service.Discord
{
	public class DiscordObjectOIDBuilder
	{
		public static DiscordObjectOID Build(ICommandContext context, string instance = "discord.com")
		{
			ulong server = context.Guild?.Id ?? 0;
			ulong category = 0;
			ulong channel = 0;
			ulong thread = 0;
			if (context.Channel is INestedChannel nestedChannel and not IThreadChannel)
			{
				category = nestedChannel.CategoryId ?? 0;
				channel = context.Channel.Id;
			} else if (context.Channel is IThreadChannel threadChannel)
			{
				// In IThreadChannels, the CategoryId always refers to the parent channel's ID rather than the actual category.
				// Unfortunately Discord does not have ParentChannel as a property of IThreadChannel, only SocketThreadChannel.
				if (context.Channel is SocketThreadChannel socketThreadChannel)
				{
					if (socketThreadChannel.ParentChannel is INestedChannel parentChannel)
					{
						category = parentChannel.CategoryId ?? 0;
					}
				} else
				{
					category = 0;
				}
				channel = threadChannel.CategoryId ?? 0;
				thread = threadChannel.Id;
			}
			ulong message = context.Message.Id;

			return DiscordObjectOID.ForMessage(instance, server, category, channel, thread, message);
		}
	}
}
