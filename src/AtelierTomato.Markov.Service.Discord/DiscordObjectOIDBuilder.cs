using AtelierTomato.Markov.Model.ObjectOID;
using Discord;

namespace AtelierTomato.Markov.Service.Discord
{
	public class DiscordObjectOIDBuilder
	{
		public static async Task<DiscordObjectOID> BuildForMessage(IGuild? guild, IChannel channel, ulong messageID, string instance = "discord.com")
		{
			ulong serverID = guild?.Id ?? 0;
			var categoryToThread = await BuildCore(guild, channel);
			return DiscordObjectOID.ForMessage(instance, serverID, categoryToThread.Category, categoryToThread.Channel, categoryToThread.Thread ?? 0, messageID);
		}

		public static async Task<DiscordObjectOID> BuildForThreadOrChannel(IGuild? guild, IChannel channel, string instance = "discord.com")
		{
			ulong server = guild?.Id ?? 0;
			var categoryToThread = await BuildCore(guild, channel);
			if (categoryToThread.Thread is not null)
			{
				return DiscordObjectOID.ForThread(instance, server, categoryToThread.Category, categoryToThread.Channel, categoryToThread.Thread.Value);
			}
			else
			{
				return DiscordObjectOID.ForChannel(instance, server, categoryToThread.Category, categoryToThread.Channel);
			}
		}

		private static async Task<DiscordCategoryToThread> BuildCore(IGuild? guild, IChannel channel)
		{
			ulong categoryID, channelID;
			ulong? threadID;
			if (channel is INestedChannel nestedChannel and not IThreadChannel)
			{
				categoryID = nestedChannel.CategoryId ?? 0;
				channelID = channel.Id;
				threadID = null;
			}
			else if (channel is IThreadChannel threadChannel)
			{
				if (guild is not null && await guild.GetChannelAsync(threadChannel.CategoryId!.Value) is not null and INestedChannel parentChannel && parentChannel.CategoryId is not null)
				{
					categoryID = parentChannel.CategoryId.Value;
				}
				else
				{
					categoryID = 0;
				}
				channelID = threadChannel.CategoryId ?? 0;
				threadID = threadChannel.Id;
			}
			else
			{
				categoryID = 0;
				channelID = channel.Id;
				threadID = null;
			}
			return new DiscordCategoryToThread(categoryID, channelID, threadID);
		}
	}
}
