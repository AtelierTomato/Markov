using AtelierTomato.Markov.Core;
using AtelierTomato.Markov.Core.Cooldown;
using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Service.Discord;
using AtelierTomato.Markov.Storage;
using ConsoleTableExt;
using Discord.Commands;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Bot.Discord.Core.CommandModules
{
	public class AuthorGroupModule : ModuleBase<SocketCommandContext>
	{
		private readonly IAuthorGroupAccess authorGroupAccess;
		private readonly IAuthorGroupPermissionAccess authorGroupPermissionAccess;
		private readonly IAuthorGroupRequestAccess authorGroupRequestAccess;
		private readonly DiscordBotOptions options;
		private readonly Cooldown cooldown;
		private readonly DiscordObjectOIDBuilder objectOIDBuilder;
		private readonly AuthorGroupManager authorGroupManager;
		public AuthorGroupModule(IAuthorGroupAccess authorGroupAccess, IAuthorGroupPermissionAccess authorGroupPermissionAccess, IAuthorGroupRequestAccess authorGroupRequestAccess, IOptions<DiscordBotOptions> options, Cooldown cooldown, DiscordObjectOIDBuilder objectOIDBuilder, AuthorGroupManager authorGroupManager)
		{
			this.authorGroupAccess = authorGroupAccess;
			this.authorGroupPermissionAccess = authorGroupPermissionAccess;
			this.authorGroupRequestAccess = authorGroupRequestAccess;
			this.options = options.Value;
			this.cooldown = cooldown;
			this.objectOIDBuilder = objectOIDBuilder;
			this.authorGroupManager = authorGroupManager;
		}

		[Command("createauthorgroup")]
		[Alias("cag")]
		[Summary("Creates an AuthorGroup and returns its name and ID")]
		public async Task CreateAuthorGroup([Remainder] string name)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			var id = await authorGroupManager.CreateGroup(authorOID, name);
			await ReplyAsync($"created new {nameof(AuthorGroup)} with ID \"{id}\" and name \"{name}\"!");
		}

		[Command("renameauthorgroup")]
		[Alias("rag")]
		[Summary("Renames an AuthorGroup")]
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
				await authorGroupManager.RenameGroup(authorOID, id, newName);
				await ReplyAsync($"renamed {nameof(AuthorGroup)} with ID \"{id}\" to \"{newName}\"");
			}
			catch (Exception ex)
			{
				await ReplyAsync(ex.Message);
			}
		}

		[Command("deleteauthorgroup")]
		[Alias("dag")]
		[Summary("Deletes an AuthorGroup")]
		public async Task DeleteAuthorGroup([Remainder] string name)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			var group = await authorGroupManager.GetValidGroupFromNameAndPermission(authorOID, name, AuthorGroupPermissionType.DeleteGroup);
			if (group is null)
			{
				await ReplyAsync($"could not find a group named \"{name}\" where author with ID \"{authorOID}\" has permission \"{AuthorGroupPermissionType.DeleteGroup}\"");
				return;
			}
			try
			{
				await authorGroupManager.DeleteGroup(authorOID, group.ID);
				await ReplyAsync($"deleted group with ID \"{group.ID}\"");
			}
			catch (Exception ex)
			{
				await ReplyAsync(ex.Message);
			}
		}

		[Command("deleteauthorgroup")]
		[Alias("dag")]
		[Summary("Deletes an AuthorGroup")]
		public async Task DeleteAuthorGroup(ulong id)
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
				await authorGroupManager.DeleteGroup(authorOID, id);
				await ReplyAsync($"deleted group with ID \"{id}\"");
			}
			catch (Exception ex)
			{
				await ReplyAsync(ex.Message);
			}
		}

		[Command("inviteauthor")]
		[Alias("ia")]
		[Summary("Invites an Author to an AuthorGroup")]
		public async Task InviteAuthor(params string[] parameters)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			var authorGroupPermission = ParseAuthorGroupPermission(parameters);
			if (authorGroupPermission is null)
			{
				await ReplyAsync(message: "you did not include all the necessary parameters, please provider a group ID, an author to invite, and one or more permissions");
				return;
			}
			try
			{
				await authorGroupManager.SendOrUpdateAuthorGroupRequest(authorOID, authorGroupPermission);
				await ReplyAsync($"invited author with id \"{authorGroupPermission.Author}\" to group with id \"{authorGroupPermission.ID}\" with permissions: {authorGroupPermission.Permissions}");
			}
			catch (Exception ex)
			{
				await ReplyAsync(ex.Message);
			}
		}

		[Command("acceptauthorinvitation")]
		[Alias("aai", "acceptauthorgroupinvitation", "aagi")]
		[Summary("Accepts an AuthorGroup invitation")]
		public async Task AcceptAuthorInvitation(ulong id)
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
				await authorGroupManager.AcceptInvitation(authorOID, id);
				await ReplyAsync($"accepted invitation to group with id \"{id}\"");
			}
			catch (Exception ex)
			{
				await ReplyAsync(ex.Message);
			}
		}

		[Command("acceptauthorinvitation")]
		[Alias("aai", "acceptauthorgroupinvitation", "aagi")]
		[Summary("Accepts an AuthorGroup invitation")]
		public async Task AcceptAuthorInvitation([Remainder] string name)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			var invitations = await authorGroupRequestAccess.ReadAuthorGroupRequestRangeByAuthor(authorOID);
			if (!invitations.Any())
			{
				await ReplyAsync("you don't have any invitations!");
				return;
			}
			var group = (await authorGroupAccess.ReadAuthorGroups(invitations.Select(i => i.ID))).Where(i => i.Name == name).FirstOrDefault();
			if (group is null)
			{
				await ReplyAsync($"no invitation to group with name \"{name}\" found");
				return;
			}
			try
			{
				await authorGroupManager.AcceptInvitation(authorOID, group.ID);
				await ReplyAsync($"accepted invitation to group with id \"{group.ID}\"");
			}
			catch (Exception ex)
			{
				await ReplyAsync(ex.Message);
			}
		}

		[Command("denyauthorinvitation")]
		[Alias("dai", "denyauthorgroupinvitation", "dagi")]
		[Summary("Denies an AuthorGroup invitation")]
		public async Task DenyAuthorInvitation(ulong id)
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
				await authorGroupManager.DenyInvitation(authorOID, id);
				await ReplyAsync($"denied invitation to group with id \"{id}\"");
			}
			catch (Exception ex)
			{
				await ReplyAsync(ex.Message);
			}
		}

		[Command("denyauthorinvitation")]
		[Alias("dai", "denyauthorgroupinvitation", "dagi")]
		[Summary("Denies an AuthorGroup invitation")]
		public async Task DenyAuthorInvitation([Remainder] string name)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			var invitations = await authorGroupRequestAccess.ReadAuthorGroupRequestRangeByAuthor(authorOID);
			if (!invitations.Any())
			{
				await ReplyAsync("you don't have any invitations!");
				return;
			}
			var group = (await authorGroupAccess.ReadAuthorGroups(invitations.Select(i => i.ID))).Where(i => i.Name == name).FirstOrDefault();
			if (group is null)
			{
				await ReplyAsync($"no invitation to group with name \"{name}\" found");
				return;
			}
			try
			{
				await authorGroupManager.DenyInvitation(authorOID, group.ID);
				await ReplyAsync($"denied invitation to group with id \"{group.ID}\"");
			}
			catch (Exception ex)
			{
				await ReplyAsync(ex.Message);
			}
		}

		[Command("updateauthor")]
		[Alias("ua")]
		[Summary("Updates an author in an AuthorGroup")]
		public async Task UpdateAuthor(params string[] parameters)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			var authorGroupPermission = ParseAuthorGroupPermission(parameters);
			if (authorGroupPermission is null)
			{
				await ReplyAsync(message: "you did not include all the necessary parameters, please provider a group ID, an author to invite, and one or more permission");
				return;
			}
			try
			{
				await authorGroupManager.UpdateAuthor(authorOID, authorGroupPermission);
				await ReplyAsync($"updated author with id \"{authorGroupPermission.Author}\" in group with id \"{authorGroupPermission.ID}\" to have permissions: {authorGroupPermission.Permissions}");
			}
			catch (Exception ex)
			{
				await ReplyAsync(ex.Message);
			}
		}

		[Command("removeauthor")]
		[Alias("ra")]
		[Summary("Removes an author from an AuthorGroup")]
		public async Task RemoveAuthor(string otherAuthorID, ulong id)
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
				AuthorOID effectiveOtherAuthorOID;
				if (ulong.TryParse(otherAuthorID, out ulong result))
				{
					effectiveOtherAuthorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, result.ToString());
				}
				else
				{
					effectiveOtherAuthorOID = AuthorOID.Parse(otherAuthorID);
				}
				await authorGroupManager.RemoveAuthor(authorOID, id, effectiveOtherAuthorOID);
				await ReplyAsync($"removed author with ID \"{effectiveOtherAuthorOID}\" from group with ID \"{id}\"");
			}
			catch (Exception ex)
			{
				await ReplyAsync(ex.Message);
			}
		}

		[Command("removeauthor")]
		[Alias("ra")]
		[Summary("Removes an author from an AuthorGroup")]
		public async Task RemoveAuthor(string otherAuthorID, [Remainder] string name)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			var group = await authorGroupManager.GetValidGroupFromNameAndPermission(authorOID, name, AuthorGroupPermissionType.RemoveAuthor);
			if (group is null)
			{
				await ReplyAsync($"could not find a group named \"{name}\" where author with ID \"{authorOID}\" has permission \"{AuthorGroupPermissionType.RemoveAuthor}\"");
				return;
			}
			try
			{
				AuthorOID effectiveOtherAuthorOID;
				if (ulong.TryParse(otherAuthorID, out ulong result))
				{
					effectiveOtherAuthorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, result.ToString());
				}
				else
				{
					effectiveOtherAuthorOID = AuthorOID.Parse(otherAuthorID);
				}
				await authorGroupManager.RemoveAuthor(authorOID, group.ID, effectiveOtherAuthorOID);
				await ReplyAsync($"removed author with ID \"{effectiveOtherAuthorOID}\" from group with ID \"{group.ID}\"");
			}
			catch (Exception ex)
			{
				await ReplyAsync(ex.Message);
			}
		}

		[Command("leaveauthorgroup")]
		[Alias("lag")]
		[Summary("Leaves an AuthorGroup")]
		public async Task LeaveAuthorGroup(ulong id)
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
				await authorGroupManager.LeaveGroup(authorOID, id);
				await ReplyAsync($"left group with ID \"{id}\"");
			}
			catch (Exception ex)
			{
				await ReplyAsync(ex.Message);
			}
		}

		[Command("leaveauthorgroup")]
		[Alias("lag")]
		[Summary("Leaves an AuthorGroup")]
		public async Task LeaveAuthorGroup([Remainder] string name)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			var permissions = await authorGroupPermissionAccess.ReadAuthorGroupPermissionRangeByAuthor(authorOID);
			if (!permissions.Any())
			{
				await ReplyAsync("you're not in any groups!");
				return;
			}
			var group = (await authorGroupAccess.ReadAuthorGroups(permissions.Select(p => p.ID))).Where(g => g.Name == name).FirstOrDefault();
			if (group is null)
			{
				await ReplyAsync($"you're not in any group with the name \"{name}\"");
				return;
			}
			try
			{
				await authorGroupManager.LeaveGroup(authorOID, group.ID);
				await ReplyAsync($"left group with ID \"{group.ID}\"");
			}
			catch (Exception ex)
			{
				await ReplyAsync(ex.Message);
			}
		}

		[Command("authorgrouprequestslist")]
		[Alias("agrl", "arl", "authorrequestslist", "authorgrouprequestlist", "listauthorrequest")]
		[Summary("Lists AuthorGroupRequests for an author")]
		public async Task AuthorGroupRequestsList()
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			var requests = (await authorGroupRequestAccess.ReadAuthorGroupRequestRangeByAuthor(authorOID)).ToList();
			if (requests.Count is 0)
			{
				await ReplyAsync($"you have no {nameof(AuthorGroup)}Requests.");
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

		[Command("authorgrouplist")]
		[Alias("agl", "authorgroupslist")]
		[Summary("Lists AuthorGroupRequests for an author")]
		public async Task AuthorGroupList()
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			var groups = (await authorGroupPermissionAccess.ReadAuthorGroupPermissionRangeByAuthor(authorOID)).ToList();
			if (groups.Count is 0)
			{
				await ReplyAsync($"you are in no {nameof(AuthorGroup)}s.");
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

		[Command("authorgroupinfo")]
		[Alias("agi")]
		[Summary("Lists the name, permissions, and requests for an AuthorGroup")]
		public async Task AuthorGroupInfo(ulong id)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			var group = await authorGroupAccess.ReadAuthorGroup(id);
			if (group is null)
			{
				await ReplyAsync($"no {nameof(AuthorGroup)} with ID \"{id}\" was found");
				return;
			}
			var permissions = (await authorGroupPermissionAccess.ReadAuthorGroupPermissionRangeByID(id)).ToList();
			if (!permissions.Any(p => p.Author == authorOID))
			{
				await ReplyAsync("you do not have any permissions in this group!");
				return;
			}
			var requests = (await authorGroupRequestAccess.ReadAuthorGroupRequestRangeByID(id)).ToList();
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
			await ReplyAsync($"Info for {nameof(AuthorGroup)} with ID \"{group.ID}\" and Name \"{group.Name}\":" + Environment.NewLine + "```" + listBuilderPerms + Environment.NewLine + listBuilderRequests + "```");
		}

		private AuthorGroupPermission? ParseAuthorGroupPermission(string[] parameters)
		{
			ulong? id = null;
			AuthorOID? otherAuthorOID = null;
			AuthorGroupPermissionType permissions = new();
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
				if (otherAuthorOID is null)
				{
					try
					{
						otherAuthorOID = AuthorOID.Parse(parameter);
						continue;
					}
					catch
					{
						if (ulong.TryParse(parameter, out ulong discordID))
						{
							otherAuthorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, discordID.ToString());
							continue;
						}
					}
				}
				if (Enum.TryParse<AuthorGroupPermissionType>(parameter, true, out var perm))
				{
					permissions |= perm;
				}
			}
			if (id is null || otherAuthorOID is null || permissions == 0)
			{
				return null;
			}
			return new AuthorGroupPermission(id.Value, otherAuthorOID, permissions);
		}
	}
}
