using AtelierTomato.Markov.Model.ObjectOID;
using Discord;

namespace AtelierTomato.Markov.Service.Discord
{
	public class DiscordObjectOIDBuilder
	{
		/// <summary>
		/// Builds a DiscordObjectOID at scope of <see cref="DiscordObjectOID.Channel"/> or <see cref="DiscordObjectOID.Thread"/>.
		/// </summary>
		/// <param name="guild">The <see cref="IGuild"/> of the desired DiscordObjectOID. Must contain the <paramref name="channel"/>.</param>
		/// <param name="channel">The <see cref="IGuildChannel"/> of the desired DiscordObjectOID. Can be <see cref="IThreadChannel"/>. Must be inside the <paramref name="guild"/>.</param>
		/// <param name="instance">Optional parameter, used if on an alternative instance of Discord.</param>
		/// <remarks>This uses Discord's API, and thus incurs a big cost.</remarks>
		/// <returns></returns>
		public static async Task<DiscordObjectOID> Build(IGuild guild, IGuildChannel channel, string instance = "discord.com")
		{
			if (channel.GuildId != guild.Id)
			{
				throw new ArgumentException($"The {nameof(guild)} provided does not contain the {nameof(channel)} provided.", $"{nameof(guild)}, {nameof(channel)}");
			}
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
				if (threadChannel.CategoryId is null)
					throw new ArgumentException($"A parent channel for the {nameof(IThreadChannel)} provided could not be found, this should not happen.", nameof(channel));

				if (await guild.GetChannelAsync(threadChannel.CategoryId!.Value) is not null and INestedChannel parentChannel && parentChannel.CategoryId is not null)
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
				return DiscordObjectOID.ForThread(instance, guild.Id, categoryID, channelID, threadID.Value);
			}
			else
			{
				return DiscordObjectOID.ForChannel(instance, guild.Id, categoryID, channelID);
			}
		}
	}
}
