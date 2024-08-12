using AtelierTomato.Markov.Model.ObjectOID;
using Discord;

namespace AtelierTomato.Markov.Service.Discord
{
	public class DiscordObjectOIDBuilder
	{
		/// <summary>
		/// Builds a DiscordObjectOID at scope of <see cref="DiscordObjectOID.Channel"/> or <see cref="DiscordObjectOID.Thread"/> <paramref name="guild"/> and <paramref name="channel"/> are passed.
		/// If <paramref name="messageID"/> is passed, builds at scope of <see cref="DiscordObjectOID.Message"/>.
		/// </summary>
		/// <param name="guild">The <see cref="IGuild"/> of the desired DiscordObjectOID. Must contain the <paramref name="channel"/>.</param>
		/// <param name="channel">The <see cref="IChannel"/> of the desired DiscordObjectOID. Can be <see cref="IThreadChannel"/>. Must be inside the <paramref name="guild"/>.</param>
		/// <param name="messageID">Optional parameter indicating <see cref="IMessage"/> ID, used if intending to build at scope <see cref="DiscordObjectOID.Message"/>.</param>
		/// <param name="instance">Optional parameter, used if on an alternative instance of Discord.</param>
		/// <returns></returns>
		public static async Task<DiscordObjectOID> Build(IGuild? guild, IChannel channel, string instance = "discord.com")
		{
			ulong serverID = guild?.Id ?? 0;
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

			if (threadID is not null)
			{
				return DiscordObjectOID.ForThread(instance, serverID, categoryID, channelID, threadID.Value);
			}
			else
			{
				return DiscordObjectOID.ForChannel(instance, serverID, categoryID, channelID);
			}
		}
	}
}
