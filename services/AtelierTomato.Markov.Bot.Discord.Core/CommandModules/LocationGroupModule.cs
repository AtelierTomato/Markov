using AtelierTomato.Markov.Core;
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
	public class LocationGroupModule : ModuleBase<SocketCommandContext>
	{
		private readonly ILocationGroupAccess locationGroupAccess;
		private readonly ILocationGroupPermissionAccess locationGroupPermissionAccess;
		private readonly ILocationGroupRequestAccess locationGroupRequestAccess;
		private readonly DiscordBotOptions options;
		private readonly Cooldown cooldown;
		private readonly DiscordObjectOIDBuilder objectOIDBuilder;
		private readonly LocationGroupManager locationGroupManager;

		public LocationGroupModule(ILocationGroupAccess locationGroupAccess, ILocationGroupPermissionAccess locationGroupPermissionAccess, ILocationGroupRequestAccess locationGroupRequestAccess, IOptions<DiscordBotOptions> options, Cooldown cooldown, DiscordObjectOIDBuilder objectOIDBuilder, LocationGroupManager locationGroupManager)
		{
			this.locationGroupAccess = locationGroupAccess;
			this.locationGroupPermissionAccess = locationGroupPermissionAccess;
			this.locationGroupRequestAccess = locationGroupRequestAccess;
			this.options = options.Value;
			this.cooldown = cooldown;
			this.objectOIDBuilder = objectOIDBuilder;
			this.locationGroupManager = locationGroupManager;
		}

		[Command("createlocationgroup")]
		[Alias("clg")]
		[Summary("Creates a LocationGroup and returns its name and ID")]
		public async Task CreateLocationGroup(DiscordLocationType locationDepth, [Remainder] string name)
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
				DiscordObjectOID groupLocation = locationDepth switch
				{
					DiscordLocationType.Global => throw new ArgumentException($"{nameof(LocationGroup)}s cannot include {DiscordLocationType.Global}!", nameof(locationDepth)),
					DiscordLocationType.Discord => DiscordObjectOID.ForService(),
					DiscordLocationType.Instance => DiscordObjectOID.ForInstance(location.Instance!),
					DiscordLocationType.Server => DiscordObjectOID.ForServer(location.Instance!, location.Server!.Value),
					DiscordLocationType.Category => DiscordObjectOID.ForCategory(location.Instance!, location.Server!.Value, location.Category!.Value),
					DiscordLocationType.Channel => DiscordObjectOID.ForChannel(location.Instance!, location.Server!.Value, location.Category!.Value, location.Channel!.Value),
					DiscordLocationType.Thread => DiscordObjectOID.ForThread(location.Instance!, location.Server!.Value, location.Category!.Value, location.Channel!.Value, location.Thread ?? 0),
					DiscordLocationType.Message => throw new ArgumentException($"{nameof(LocationGroup)}s cannot include {DiscordLocationType.Message}!", nameof(locationDepth)),
					DiscordLocationType.Sentence => throw new ArgumentException($"{nameof(LocationGroup)}s cannot include {DiscordLocationType.Sentence}!", nameof(locationDepth)),
					_ => throw new NotImplementedException($"this {nameof(DiscordLocationType)} is not implemented!")
				};
				var id = await locationGroupManager.CreateGroup(authorOID, groupLocation, name);
				await ReplyAsync($"created new {nameof(LocationGroup)} with ID \"{id}\" and name \"{name}\"!");
			}
			catch (Exception ex)
			{
				await ReplyAsync(message: ex.Message);
				return;
			}
		}

		[Command("renamelocationgroup")]
		[Alias("rlg")]
		[Summary("Renames a LocationGroup")]
		public async Task RenameAuthorGroup(Guid id, [Remainder] string newName)
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
				await locationGroupManager.RenameGroup(authorOID, id, newName);
				await ReplyAsync($"renamed {nameof(LocationGroup)} with ID \"{id}\" to \"{newName}\"");
			}
			catch (Exception ex)
			{
				await ReplyAsync(ex.Message);
			}
		}

		[Command("deletelocationgroup")]
		[Alias("dlg")]
		[Summary("Deletes a LocationGroup")]
		public async Task DeleteLocationGroup([Remainder] string name)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			var group = await locationGroupManager.GetValidGroupFromNameAndPermission(authorOID, name, LocationGroupPermissionType.DeleteGroup);
			if (group is null)
			{
				await ReplyAsync($"could not find a group named \"{name}\" where author with ID \"{authorOID}\" has permission \"{AuthorGroupPermissionType.DeleteGroup}\"");
				return;
			}
			try
			{
				await locationGroupManager.DeleteGroup(authorOID, group.ID);
				await ReplyAsync($"deleted group with ID \"{group.ID}\"");
			}
			catch (Exception ex)
			{
				await ReplyAsync(ex.Message);
			}
		}

		[Command("deletelocationgroup")]
		[Alias("dlg")]
		[Summary("Deletes a LocationGroup")]
		public async Task DeleteLocationGroup(Guid id)
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
				await locationGroupManager.DeleteGroup(authorOID, id);
				await ReplyAsync($"deleted group with ID \"{id}\"");
			}
			catch (Exception ex)
			{
				await ReplyAsync(ex.Message);
			}
		}
	}
}
