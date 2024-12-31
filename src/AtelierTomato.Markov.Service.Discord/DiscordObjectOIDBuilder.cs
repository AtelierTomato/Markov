using AtelierTomato.Markov.Model.ObjectOID;
using Discord;
using Microsoft.Extensions.Logging;

namespace AtelierTomato.Markov.Service.Discord
{
	public class DiscordObjectOIDBuilder
	{
		private readonly ILogger<DiscordObjectOIDBuilder> logger;

		public DiscordObjectOIDBuilder(ILogger<DiscordObjectOIDBuilder> logger)
		{
			this.logger = logger;
		}

		/// <summary>
		/// Builds a DiscordObjectOID at scope of <see cref="DiscordObjectOID.Channel"/> or <see cref="DiscordObjectOID.Thread"/>.
		/// </summary>
		/// <param name="guild">The <see cref="IGuild"/> of the desired DiscordObjectOID. Must contain the <paramref name="guildChannel"/>.</param>
		/// <param name="guildChannel">The <see cref="IGuildChannel"/> of the desired DiscordObjectOID. Can be <see cref="IThreadChannel"/>. Must be inside the <paramref name="guild"/>.</param>
		/// <param name="instance">Optional parameter, used if on an alternative instance of Discord.</param>
		/// <remarks>This uses Discord's API, and thus incurs a big cost.</remarks>
		/// <returns></returns>
		public async Task<DiscordObjectOID> Build(IGuild? guild, IChannel channel, string instance = "discord.com")
		{
			if (channel is IGuildChannel guildChannel && guild is not null)
			{
				if (guildChannel.GuildId != guild.Id)
				{
					throw new ArgumentException($"The {nameof(guild)} provided does not contain the {nameof(guildChannel)} provided.", $"{nameof(guild)}, {nameof(guildChannel)}");
				}
				ulong categoryID, channelID;
				ulong? threadID;
				if (guildChannel is INestedChannel nestedChannel and not IThreadChannel)
				{
					categoryID = nestedChannel.CategoryId ?? 0;
					channelID = guildChannel.Id;
					threadID = null;
				}
				else if (guildChannel is IThreadChannel threadChannel)
				{
					if (threadChannel.CategoryId is null)
					{
						_logCategoryIdWarning(logger, threadChannel.Id, null);
						categoryID = 0;
					}
					else
					{
						var parentChannel = await guild.GetChannelAsync(threadChannel.CategoryId.Value) as INestedChannel;
						categoryID = parentChannel?.CategoryId ?? 0;
					}
					channelID = threadChannel.CategoryId ?? 0;
					threadID = threadChannel.Id;
				}
				else
				{
					categoryID = 0;
					channelID = guildChannel.Id;
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
			else
			{
				return DiscordObjectOID.ForThread(instance, 0, 0, channel.Id, 0);
			}
		}

		private static readonly Action<ILogger, ulong, Exception?> _logCategoryIdWarning =
			LoggerMessage.Define<ulong>(
				LogLevel.Warning,
				new EventId(1, nameof(Build)),
				"The CategoryId for the thread {ThreadId} was null. This is unexpected.");
	}
}
