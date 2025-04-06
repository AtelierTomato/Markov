using AtelierTomato.Markov.Core.Cooldown;
using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Model.ObjectOID;
using AtelierTomato.Markov.Model.ObjectOID.LocationTypes;
using AtelierTomato.Markov.Service.Discord;
using AtelierTomato.Markov.Storage;
using Discord.Commands;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Bot.Discord.Core.CommandModules
{
	public class LocationSettingsModule : ModuleBase<SocketCommandContext>
	{
		private readonly DiscordBotOptions options;
		private readonly DiscordObjectOIDBuilder objectOIDBuilder;
		private readonly Cooldown cooldown;
		private readonly ILocationAccess locationAccess;
		private readonly ILocationSettingAccess locationSettingAccess;
		private readonly MultiParser<IObjectOID> objectOIDParser;
		public LocationSettingsModule(IOptions<DiscordBotOptions> options, DiscordObjectOIDBuilder objectOIDBuilder, Cooldown cooldown, ILocationSettingAccess locationSettingAccess, ILocationAccess locationAccess, MultiParser<IObjectOID> objectOIDParser)
		{
			this.options = options.Value;
			this.objectOIDBuilder = objectOIDBuilder;
			this.cooldown = cooldown;
			this.locationAccess = locationAccess;
			this.locationSettingAccess = locationSettingAccess;
			this.objectOIDParser = objectOIDParser;
		}

		[Command("global")]
		[Alias("g")]
		[Summary("Allows or forbids access to other server's messages when using commands in this server.")]
		public async Task Global(string? locationParam = null)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			IObjectOID location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}

			if (locationParam is not null)
			{
				try
				{
					if (Enum.TryParse<DiscordLocationType>(locationParam, true, out var locationDepth))
					{
						try
						{
							location = GetLocation(locationDepth, (DiscordObjectOID)location);
						}
						catch (Exception ex)
						{
							await ReplyAsync(ex.Message);
							return;
						}
					}
					else
					{
						location = objectOIDParser.Parse(locationParam);
					}
				}
				catch
				{
					await ReplyAsync($"location was not valid, please provide an {nameof(IObjectOID)} or a scope (such as server, channel)");
					return;
				}
			}
			else
			{
				location = DiscordObjectOID.ForServer(options.DiscordInstance, ((DiscordObjectOID)location).Server!.Value);
			}

			if (!options.DeveloperIDs.Contains(Context.User.Id))
			{
				var owner = await locationAccess.ReadLocationOwner(location);
				if (!(owner == authorOID))
				{
					await ReplyAsync("sorry, only the owner of the server is allowed to use this command!");
					return;
				}
			}

			var locationSetting = await locationSettingAccess.ReadLocationSetting(location) ?? new(location, [], [], [], false, null);
			locationSetting = new(locationSetting.ID, locationSetting.WriteReactions, locationSetting.DeleteReactions, locationSetting.FailReactions, !locationSetting.GlobalAllowed, locationSetting.LocationGroup);
			await locationSettingAccess.WriteLocationSetting(locationSetting);
			await ReplyAsync($"updated global allowed permissions for {location} to {locationSetting.GlobalAllowed}!");
		}

		private static DiscordObjectOID GetLocation(DiscordLocationType locationDepth, DiscordObjectOID location) => locationDepth switch
		{
			DiscordLocationType.Global => throw new ArgumentException($"Global cannot be set for {DiscordLocationType.Global}!", nameof(locationDepth)),
			DiscordLocationType.Discord => DiscordObjectOID.ForService(),
			DiscordLocationType.Instance => DiscordObjectOID.ForInstance(location.Instance!),
			DiscordLocationType.Server => DiscordObjectOID.ForServer(location.Instance!, location.Server!.Value),
			DiscordLocationType.Category => DiscordObjectOID.ForCategory(location.Instance!, location.Server!.Value, location.Category!.Value),
			DiscordLocationType.Channel => DiscordObjectOID.ForChannel(location.Instance!, location.Server!.Value, location.Category!.Value, location.Channel!.Value),
			DiscordLocationType.Thread => DiscordObjectOID.ForThread(location.Instance!, location.Server!.Value, location.Category!.Value, location.Channel!.Value, location.Thread ?? 0),
			DiscordLocationType.Message => throw new ArgumentException($"Global cannot be set for {DiscordLocationType.Message}!", nameof(locationDepth)),
			DiscordLocationType.Sentence => throw new ArgumentException($"Global cannot be set for {DiscordLocationType.Sentence}!", nameof(locationDepth)),
			_ => throw new NotImplementedException($"this {nameof(DiscordLocationType)} is not implemented!")
		};
	}
}
