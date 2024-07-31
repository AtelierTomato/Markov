using AtelierTomato.Markov.Model.ObjectOID;
using Discord;

namespace AtelierTomato.Markov.Service.Discord
{
	public class DiscordObjectOIDBuilder
	{
		public static async Task<DiscordObjectOID> Build(IGuild? guild, IChannel channel, ulong messageID, string instance = "discord.com")
		{
			ulong serverID = guild?.Id ?? 0;
			ulong categoryID, channelID, threadID;
			if (channel is INestedChannel nestedChannel and not IThreadChannel)
			{
				categoryID = nestedChannel.CategoryId ?? 0;
				channelID = channel.Id;
				threadID = 0;
			} else if (channel is IThreadChannel threadChannel)
			{
				if (guild is not null && await guild.GetChannelAsync(threadChannel.CategoryId!.Value) is not null and INestedChannel parentChannel && parentChannel.CategoryId is not null)
				{
					categoryID = parentChannel.CategoryId.Value;
				} else
				{
					categoryID = 0;
				}
				channelID = threadChannel.CategoryId ?? 0;
				threadID = threadChannel.Id;
			} else
			{
				categoryID = 0;
				channelID = channel.Id;
				threadID = 0;
			}

			return DiscordObjectOID.ForMessage(instance, serverID, categoryID, channelID, threadID, messageID);
		}
	}
}
