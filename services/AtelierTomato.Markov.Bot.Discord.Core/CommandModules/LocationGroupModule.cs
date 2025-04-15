using AtelierTomato.Markov.Core;
using AtelierTomato.Markov.Core.Cooldown;
using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Model.ObjectOID;
using AtelierTomato.Markov.Model.ObjectOID.LocationTypes;
using AtelierTomato.Markov.Service.Discord;
using AtelierTomato.Markov.Storage;
using ConsoleTableExt;
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
		private readonly MultiParser<IObjectOID> objectOIDParser;
		private readonly HelpContentBuilder helpContentBuilder;
		public LocationGroupModule(ILocationGroupAccess locationGroupAccess, ILocationGroupPermissionAccess locationGroupPermissionAccess, ILocationGroupRequestAccess locationGroupRequestAccess, IOptions<DiscordBotOptions> options, Cooldown cooldown, DiscordObjectOIDBuilder objectOIDBuilder, LocationGroupManager locationGroupManager, MultiParser<IObjectOID> objectOIDParser, HelpContentBuilder helpContentBuilder)
		{
			this.locationGroupAccess = locationGroupAccess;
			this.locationGroupPermissionAccess = locationGroupPermissionAccess;
			this.locationGroupRequestAccess = locationGroupRequestAccess;
			this.options = options.Value;
			this.cooldown = cooldown;
			this.objectOIDBuilder = objectOIDBuilder;
			this.locationGroupManager = locationGroupManager;
			this.objectOIDParser = objectOIDParser;
			this.helpContentBuilder = helpContentBuilder;
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
				if (locationDepth is DiscordLocationType.Global or DiscordLocationType.Message or DiscordLocationType.Sentence)
				{
					await ReplyAsync($"{nameof(LocationGroup)}s cannot include {locationDepth}!");
					return;
				}
				var groupLocation = location.ForLocationType(locationDepth)!;
				var id = await locationGroupManager.CreateGroup(authorOID, groupLocation, name, options.DeveloperIDs.Contains(Context.User.Id));
				await ReplyAsync($"created new {nameof(LocationGroup)} with ID \"{id}\" and name \"{name}\"!");
			}
			catch (Exception ex)
			{
				await ReplyAsync(message: ex.Message);
				return;
			}
		}

		[Command("createlocationgroup")]
		[Alias("clg")]
		[Summary("Creates a LocationGroup and returns its name and ID")]
		public async Task CreateLocationGroup([Remainder] string? _ = null)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			await ReplyAsync(embed: helpContentBuilder.BuildForSubject(HelpSubject.CreateLocationGroup).Build());
		}

		[Command("renamelocationgroup")]
		[Alias("rlg")]
		[Summary("Renames a LocationGroup")]
		public async Task RenameAuthorGroup(ulong id, [Remainder] string newName)
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
				await locationGroupManager.RenameGroup(authorOID, id, newName, options.DeveloperIDs.Contains(Context.User.Id));
				await ReplyAsync($"renamed {nameof(LocationGroup)} with ID \"{id}\" to \"{newName}\"");
			}
			catch (Exception ex)
			{
				await ReplyAsync(ex.Message);
			}
		}

		[Command("renamelocationgroup")]
		[Alias("rlg")]
		[Summary("Renames a LocationGroup")]
		public async Task RenameAuthorGroup([Remainder] string? _ = null)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			await ReplyAsync(embed: helpContentBuilder.BuildForSubject(HelpSubject.RenameLocationGroup).Build());
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
		public async Task DeleteLocationGroup(ulong id)
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
				await locationGroupManager.DeleteGroup(authorOID, id, options.DeveloperIDs.Contains(Context.User.Id));
				await ReplyAsync($"deleted group with ID \"{id}\"");
			}
			catch (Exception ex)
			{
				await ReplyAsync(ex.Message);
			}
		}

		[Command("deletelocationgroup")]
		[Alias("dlg")]
		[Summary("Deletes a LocationGroup")]
		public async Task DeleteLocationGroup()
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			await ReplyAsync(embed: helpContentBuilder.BuildForSubject(HelpSubject.DeleteLocationGroup).Build());
		}

		[Command("invitelocation")]
		[Alias("il")]
		[Summary("Invites a Location to a LocationGroup")]
		public async Task InviteLocation(params string[] parameters)
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
				var locationGroupPermission = ParseLocationGroupPermission(parameters, location);
				if (locationGroupPermission is null)
				{
					await ReplyAsync(message: $"you did not include all the necessary parameters, please provide a group ID, a location to invite (either as a scope relative to the current channel, or the raw {nameof(IObjectOID)}), and one or more permissions");
					return;
				}
				await locationGroupManager.SendOrUpdateLocationGroupRequest(authorOID, locationGroupPermission, options.DeveloperIDs.Contains(Context.User.Id));
				await ReplyAsync($"invited location with id \"{locationGroupPermission.Location}\" to group with id \"{locationGroupPermission.ID}\" with permissions: {locationGroupPermission.Permissions}");
			}
			catch (Exception ex)
			{
				await ReplyAsync(ex.Message);
			}
		}

		[Command("acceptlocationinvite")]
		[Alias("ali", "acceptlocationgroupinvite", "algi")]
		[Summary("Accepts a LocationGroup invitation")]
		public async Task AcceptLocationInvite(string inviteLocationParam, ulong id)
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
				IObjectOID inviteLocation = new SpecialObjectOID(Model.ObjectOID.Types.SpecialObjectOIDType.Invalid);
				try
				{
					inviteLocation = objectOIDParser.Parse(inviteLocationParam);
				}
				catch
				{
					if (Enum.TryParse<DiscordLocationType>(inviteLocationParam, true, out var locationDepth))
					{
						if (locationDepth is DiscordLocationType.Global or DiscordLocationType.Message or DiscordLocationType.Sentence)
						{
							await ReplyAsync($"{nameof(LocationGroup)}s cannot include {locationDepth}!");
							return;
						}
						inviteLocation = location.ForLocationType(locationDepth)!;

					}
					else
					{
						await ReplyAsync(message: $"invite location was not a valid {nameof(IObjectOID)} or {nameof(DiscordLocationType)}");
					}
				}
				if (inviteLocation == new SpecialObjectOID(Model.ObjectOID.Types.SpecialObjectOIDType.Invalid))
				{
					await ReplyAsync("invite location was not set");
				}
				await locationGroupManager.AcceptInvitation(authorOID, inviteLocation, id, options.DeveloperIDs.Contains(Context.User.Id));
				await ReplyAsync($"accepted invitation for location with id \"{inviteLocation}\" to group with id \"{id}\"");
			}
			catch (Exception ex)
			{
				await ReplyAsync(ex.Message);
			}
		}

		[Command("acceptlocationinvite")]
		[Alias("ali", "acceptlocationgroupinvite", "algi")]
		[Summary("Accepts a LocationGroup invitation")]
		public async Task AcceptLocationInvite([Remainder] string? _ = null)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			await ReplyAsync(embed: helpContentBuilder.BuildForSubject(HelpSubject.AcceptLocationGroupInvite).Build());
		}

		[Command("denylocationinvite")]
		[Alias("dli", "denylocationgroupinvite", "dlgi")]
		[Summary("Denies a LocationGroup invite")]
		public async Task DenyLocationInvite(string inviteLocationParam, ulong id)
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
				IObjectOID inviteLocation = new SpecialObjectOID(Model.ObjectOID.Types.SpecialObjectOIDType.Invalid);
				try
				{
					inviteLocation = objectOIDParser.Parse(inviteLocationParam);
				}
				catch
				{
					if (Enum.TryParse<DiscordLocationType>(inviteLocationParam, true, out var locationDepth))
					{
						if (locationDepth is DiscordLocationType.Global or DiscordLocationType.Message or DiscordLocationType.Sentence)
						{
							await ReplyAsync($"{nameof(LocationGroup)}s cannot include {locationDepth}!");
							return;
						}
						inviteLocation = location.ForLocationType(locationDepth)!;
					}
					else
					{
						await ReplyAsync(message: $"invite location was not a valid {nameof(IObjectOID)} or {nameof(DiscordLocationType)}");
					}
				}
				if (inviteLocation == new SpecialObjectOID(Model.ObjectOID.Types.SpecialObjectOIDType.Invalid))
				{
					await ReplyAsync("invite location was not set");
				}
				await locationGroupManager.DenyInvitation(authorOID, inviteLocation, id, options.DeveloperIDs.Contains(Context.User.Id));
				await ReplyAsync($"denied invitation for location with id \"{inviteLocation}\" to group with id \"{id}\"");
			}
			catch (Exception ex)
			{
				await ReplyAsync(ex.Message);
			}
		}

		[Command("denylocationinvite")]
		[Alias("dli", "denylocationgroupinvite", "dlgi")]
		[Summary("Denies a LocationGroup invite")]
		public async Task DenyLocationInvite([Remainder] string? _ = null)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			await ReplyAsync(embed: helpContentBuilder.BuildForSubject(HelpSubject.DenyLocationGroupInvite).Build());
		}

		[Command("updatelocation")]
		[Alias("ul")]
		[Summary("Updates a location in a LocationGroup")]
		public async Task UpdateLocation(params string[] parameters)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			var locationGroupPermission = ParseLocationGroupPermission(parameters, location);
			if (locationGroupPermission is null)
			{
				await ReplyAsync(message: $"you did not include all the necessary parameters, please provide a group ID, a location to invite (either as a scope relative to the current channel, or the raw {nameof(IObjectOID)}), and one or more permissions");
				return;
			}
			try
			{
				await locationGroupManager.UpdateLocation(authorOID, locationGroupPermission, options.DeveloperIDs.Contains(Context.User.Id));
				await ReplyAsync($"updated location with id \"{locationGroupPermission.Location}\" in group with id \"{locationGroupPermission.ID}\" to have permissions: {locationGroupPermission.Permissions}");
			}
			catch (Exception ex)
			{
				await ReplyAsync(ex.Message);
			}
		}

		[Command("removelocation")]
		[Alias("rl")]
		[Summary("Removes a location from a LocationGroup")]
		public async Task RemoveLocation(string otherLocation, ulong id)
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
				IObjectOID effectiveOtherLocation;
				if (Enum.TryParse<DiscordLocationType>(otherLocation, true, out var locationDepth))
				{
					if (locationDepth is DiscordLocationType.Global or DiscordLocationType.Message or DiscordLocationType.Sentence)
					{
						await ReplyAsync($"{nameof(LocationGroup)}s cannot include {locationDepth}!");
						return;
					}
					effectiveOtherLocation = location.ForLocationType(locationDepth)!;
				}
				else
				{
					effectiveOtherLocation = objectOIDParser.Parse(otherLocation);
				}
				await locationGroupManager.RemoveLocation(authorOID, id, effectiveOtherLocation, options.DeveloperIDs.Contains(Context.User.Id));
				await ReplyAsync($"removed location with ID \"{effectiveOtherLocation}\" from group with ID \"{id}\"");
			}
			catch (Exception ex)
			{
				await ReplyAsync(ex.Message);
			}
		}

		[Command("removelocation")]
		[Alias("rl")]
		[Summary("Removes a location from a LocationGroup")]
		public async Task RemoveLocation(string otherLocation, [Remainder] string name)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			var group = await locationGroupManager.GetValidGroupFromNameAndPermission(authorOID, name, LocationGroupPermissionType.RemoveLocation);
			if (group is null)
			{
				await ReplyAsync($"could not find a group named \"{name}\" where author with ID \"{authorOID}\" has permission \"{LocationGroupPermissionType.RemoveLocation}\"");
				return;
			}
			try
			{
				IObjectOID effectiveOtherLocation;
				if (Enum.TryParse<DiscordLocationType>(otherLocation, true, out var locationDepth))
				{
					if (locationDepth is DiscordLocationType.Global or DiscordLocationType.Message or DiscordLocationType.Sentence)
					{
						await ReplyAsync($"{nameof(LocationGroup)}s cannot include {locationDepth}!");
						return;
					}
					effectiveOtherLocation = location.ForLocationType(locationDepth)!;
				}
				else
				{
					effectiveOtherLocation = objectOIDParser.Parse(otherLocation);
				}
				await locationGroupManager.RemoveLocation(authorOID, group.ID, effectiveOtherLocation);
				await ReplyAsync($"removed location with ID \"{effectiveOtherLocation}\" from group with ID \"{group.ID}\"");
			}
			catch (Exception ex)
			{
				await ReplyAsync(ex.Message);
			}
		}

		[Command("removelocation")]
		[Alias("rl")]
		[Summary("Removes a location from a LocationGroup")]
		public async Task RemoveLocation()
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			await ReplyAsync(embed: helpContentBuilder.BuildForSubject(HelpSubject.RemoveLocation).Build());
		}

		[Command("locationgrouprequestslist")]
		[Alias("lgrl", "lrl", "locationrequestslist", "locationgrouprequestlist", "locationrequestlist")]
		[Summary("Lists LocationGroupRequests for an author")]
		public async Task LocationGroupRequestsList([Remainder] string? _ = null)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			var requests = (await locationGroupRequestAccess.ReadLocationGroupRequestRangeByOwner(authorOID)).ToList();
			if (requests.Count is 0)
			{
				await ReplyAsync($"you have no {nameof(LocationGroup)}Requests.");
			}
			else
			{
				var listBuilder = ConsoleTableBuilder
					.From(requests)
					.WithFormat(ConsoleTableBuilderFormat.Minimal)
					.Export();
				await ReplyAsync(listBuilder.Insert(0, "```").Append("```").ToString().TrimEnd());
			}
		}

		[Command("locationgrouplist")]
		[Alias("lgl", "locationgroupslist")]
		[Summary("Lists LocationGroups for an author")]
		public async Task LocationGroupList([Remainder] string? _ = null)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			var groups = (await locationGroupPermissionAccess.ReadLocationGroupPermissionRangeByOwner(authorOID)).ToList();
			if (groups.Count is 0)
			{
				await ReplyAsync($"you are in no {nameof(LocationGroup)}s.");
			}
			else
			{
				var listBuilder = ConsoleTableBuilder
					.From(groups)
					.WithFormat(ConsoleTableBuilderFormat.Minimal)
					.Export();
				await ReplyAsync(listBuilder.Insert(0, "```").Append("```").ToString().TrimEnd());
			}
		}

		[Command("locationgroupinfo")]
		[Alias("lgi")]
		[Summary("Lists the name, permissions, and requests for a LocationGroup")]
		public async Task LocationGroupInfo(ulong id)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			var group = await locationGroupAccess.ReadLocationGroup(id);
			if (group is null)
			{
				await ReplyAsync($"no {nameof(LocationGroup)} with ID \"{id}\" was found");
				return;
			}
			if ((await locationGroupPermissionAccess.ReadLocationGroupPermissionsForOwner(id, authorOID)) is LocationGroupPermissionType.None)
			{
				await ReplyAsync("you do not have any permissions in this group!");
				return;
			}
			var permissions = (await locationGroupPermissionAccess.ReadLocationGroupPermissionRangeByID(id)).ToList();
			var requests = (await locationGroupRequestAccess.ReadLocationGroupRequestRangeByID(id)).ToList();
			var listBuilderPerms = ConsoleTableBuilder
				.From(permissions)
				.WithFormat(ConsoleTableBuilderFormat.Minimal)
				.WithTitle("Permissions")
				.Export();
			var listBuilderRequests = ConsoleTableBuilder
				.From(requests)
				.WithFormat(ConsoleTableBuilderFormat.Minimal)
				.WithTitle("Requests")
				.Export();
			await ReplyAsync($"Info for {nameof(LocationGroup)} with ID \"{group.ID}\" and Name \"{group.Name}\":" + Environment.NewLine + "```" + listBuilderPerms + Environment.NewLine + listBuilderRequests + "```");
		}

		[Command("locationgroupinfo")]
		[Alias("lgi")]
		[Summary("Lists the name, permissions, and requests for a LocationGroup")]
		public async Task LocationGroupInfo([Remainder] string? _ = null)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			await ReplyAsync(embed: helpContentBuilder.BuildForSubject(HelpSubject.LocationGroupInfo).Build());
		}

		private LocationGroupPermission? ParseLocationGroupPermission(string[] parameters, DiscordObjectOID location)
		{
			ulong? id = null;
			IObjectOID? otherLocationOID = null;
			LocationGroupPermissionType permissions = new();
			foreach (var parameter in parameters)
			{
				if (id is null)
				{
					if (ulong.TryParse(parameter, out var tempId))
					{
						id = tempId;
						continue;
					}
				}
				if (otherLocationOID is null)
				{
					try
					{
						otherLocationOID = objectOIDParser.Parse(parameter);
						continue;
					}
					catch
					{
						if (Enum.TryParse<DiscordLocationType>(parameter, true, out var locationDepth))
						{
							if (locationDepth is DiscordLocationType.Global or DiscordLocationType.Message or DiscordLocationType.Sentence)
							{
								return null;
							}
							otherLocationOID = location.ForLocationType(locationDepth)!;

							continue;
						}
					}
				}
				if (Enum.TryParse<LocationGroupPermissionType>(parameter, true, out var perm))
				{
					permissions |= perm;
				}
			}
			if (id is null || otherLocationOID is null || permissions == 0)
			{
				return null;
			}
			return new LocationGroupPermission(id.Value, otherLocationOID, permissions);
		}
	}
}
