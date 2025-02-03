using AtelierTomato.Markov.Core;
using AtelierTomato.Markov.Core.Cooldown;
using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Service.Discord;
using AtelierTomato.Markov.Storage;
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
		public async Task DeleteAuthorGroup(Guid id)
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

		//[Command("inviteauthor")]
		//[Alias("ia")]
		//[Summary("Invites an author to an AuthorGroup")]
		//public async Task InviteAuthor(Guid id, IUser user, [Remainder] string[] permissionsParam)
		//{
		//	var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
		//	var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
		//	if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
		//	{
		//		await ReplyAsync(message: "slow down!!");
		//		return;
		//	}
		//	try
		//	{
		//		var permissions = permissionsParam
		//			.Select(Enum.Parse<AuthorGroupPermissionType>)
		//			.Aggregate((current, perm) => current | perm);
		//		var otherAuthorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, user.Id.ToString());
		//		var authorGroupPermission = new AuthorGroupPermission(id, otherAuthorOID, permissions);
		//		await authorGroupManager.SendOrUpdateAuthorGroupRequest(authorOID, authorGroupPermission);
		//		await ReplyAsync($"invited author with ID \"{user.Id}\" to group with ID \"{id}\"");
		//	}
		//	catch (Exception ex)
		//	{
		//		await ReplyAsync(ex.Message);
		//	}
		//}

		//[Command("inviteauthor")]
		//[Alias("ia")]
		//[Summary("Invites an author to an AuthorGroup")]
		//public async Task InviteAuthor(string otherAuthorID, [Remainder] string[] permissionsParam)
		//{
		//	var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
		//	var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
		//	if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
		//	{
		//		await ReplyAsync(message: "slow down!!");
		//		return;
		//	}
		//	try
		//	{
		//		AuthorOID effectiveOtherAuthorOID;
		//		if (ulong.TryParse(otherAuthorID, out ulong result))
		//		{
		//			effectiveOtherAuthorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, result.ToString());
		//		}
		//		else
		//		{
		//			effectiveOtherAuthorOID = AuthorOID.Parse(otherAuthorID);
		//		}
		//		var permissions = permissionsParam
		//			.Select(Enum.Parse<AuthorGroupPermissionType>)
		//			.Aggregate((current, perm) => current | perm);
		//		var authorGroupPermission = new AuthorGroupPermission(id, effectiveOtherAuthorOID, permissions);
		//		await authorGroupManager.SendOrUpdateAuthorGroupRequest(authorOID, authorGroupPermission);
		//		await ReplyAsync($"invited author with ID \"{effectiveOtherAuthorOID}\" to group with ID \"{id}\"");
		//	}
		//	catch (Exception ex)
		//	{
		//		await ReplyAsync(ex.Message);
		//	}
		//}

		//[Command("inviteauthor")]
		//[Alias("ia")]
		//[Summary("Invites an author to an AuthorGroup")]
		//public async Task InviteAuthor(IUser user, [Remainder] string[] param)
		//{
		//	var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
		//	var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
		//	if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
		//	{
		//		await ReplyAsync(message: "slow down!!");
		//		return;
		//	}
		//	try
		//	{
		//		(string groupName, AuthorGroupPermissionType permissions) = ParseGroupAndPermissions(param);
		//		var group = await authorGroupManager.GetValidGroupFromNameAndPermission(authorOID, groupName, AuthorGroupPermissionType.AddAuthor);
		//		if (group is null)
		//		{
		//			await ReplyAsync($"could not find a group named \"{groupName}\" where author with ID \"{authorOID}\" has permission \"{AuthorGroupPermissionType.AddAuthor}\"");
		//			return;
		//		}
		//		var otherAuthorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, user.Id.ToString());
		//		var authorGroupPermission = new AuthorGroupPermission(group.ID, otherAuthorOID, permissions);
		//		await authorGroupManager.SendOrUpdateAuthorGroupRequest(authorOID, authorGroupPermission);
		//		await ReplyAsync($"invited author with ID \"{user.Id}\" to group with ID \"{group.ID}\"");
		//	}
		//	catch (Exception ex)
		//	{
		//		await ReplyAsync(ex.Message);
		//	}
		//}

		//[Command("inviteauthor")]
		//[Alias("ia")]
		//[Summary("Invites an author to an AuthorGroup")]
		//public async Task InviteAuthor(string otherAuthorID, [Remainder] string[] param)
		//{
		//	var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
		//	var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
		//	if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
		//	{
		//		await ReplyAsync(message: "slow down!!");
		//		return;
		//	}
		//	try
		//	{
		//		AuthorOID effectiveOtherAuthorOID;
		//		if (ulong.TryParse(otherAuthorID, out ulong result))
		//		{
		//			effectiveOtherAuthorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, result.ToString());
		//		}
		//		else
		//		{
		//			effectiveOtherAuthorOID = AuthorOID.Parse(otherAuthorID);
		//		}
		//		(string groupName, AuthorGroupPermissionType permissions) = ParseGroupAndPermissions(param);
		//		var group = await authorGroupManager.GetValidGroupFromNameAndPermission(authorOID, groupName, AuthorGroupPermissionType.AddAuthor);
		//		if (group is null)
		//		{
		//			await ReplyAsync($"could not find a group named \"{groupName}\" where author with ID \"{authorOID}\" has permission \"{AuthorGroupPermissionType.AddAuthor}\"");
		//			return;
		//		}
		//		var authorGroupPermission = new AuthorGroupPermission(group.ID, effectiveOtherAuthorOID, permissions);
		//		await authorGroupManager.SendOrUpdateAuthorGroupRequest(authorOID, authorGroupPermission);
		//		await ReplyAsync($"invited author with ID \"{effectiveOtherAuthorOID}\" to group with ID \"{group.ID}\"");
		//	}
		//	catch (Exception ex)
		//	{
		//		await ReplyAsync(ex.Message);
		//	}
		//}

		//public static (string GroupName, AuthorGroupPermissionType Permissions) ParseGroupAndPermissions(string[] param)
		//{
		//	var nameParts = new List<string>();
		//	var permissionsList = new List<AuthorGroupPermissionType>();
		//	bool foundPermission = false;

		//	foreach (var part in param)
		//	{
		//		if (!foundPermission)
		//		{
		//			try
		//			{
		//				var permission = Enum.Parse<AuthorGroupPermissionType>(part, true);
		//				foundPermission = true;
		//				permissionsList.Add(permission); // First permission found
		//			}
		//			catch (ArgumentException)
		//			{
		//				nameParts.Add(part); // Still part of the name
		//			}
		//		}
		//		else
		//		{
		//			permissionsList.Add(Enum.Parse<AuthorGroupPermissionType>(part, true)); // Add remaining permissions
		//		}
		//	}

		//	if (permissionsList.Count == 0)
		//	{
		//		throw new ArgumentException("No valid permissions were found in the input.");
		//	}

		//	string groupName = string.Join(" ", nameParts);
		//	var permissions = permissionsList.Aggregate((current, perm) => current | perm);

		//	return (groupName, permissions);
		//}


		//[Command("acceptauthorinvitation")]
		//[Alias("aai", "acceptauthorgroupinvitation", "aagi")]
		//[Summary("Accepts an AuthorGroup invitation")]
		//public async Task AcceptAuthorInvitation(Guid id)
		//{
		//	var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
		//	var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
		//	if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
		//	{
		//		await ReplyAsync(message: "slow down!!");
		//		return;
		//	}
		//	try
		//	{
		//		await authorGroupManager.AcceptInvitation(authorOID, id);
		//		await ReplyAsync($"accepted invitation to group with id \"{id}\"");
		//	}
		//	catch (Exception ex)
		//	{
		//		await ReplyAsync(ex.Message);
		//	}
		//}

		//[Command("acceptauthorinvitation")]
		//[Alias("aai", "acceptauthorgroupinvitation", "aagi")]
		//[Summary("Accepts an AuthorGroup invitation")]
		//public async Task AcceptAuthorInvitation([Remainder] string name)
		//{
		//	var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
		//	var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
		//	if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
		//	{
		//		await ReplyAsync(message: "slow down!!");
		//		return;
		//	}
		//	var invitations = await authorGroupRequestAccess.ReadAuthorGroupRequestRangeByAuthor(authorOID);
		//	if (!invitations.Any())
		//	{
		//		await ReplyAsync("you don't have any invitations!");
		//		return;
		//	}
		//	var group = (await authorGroupAccess.ReadAuthorGroups(invitations.Select(i => i.ID))).Where(i => i.Name == name).FirstOrDefault();
		//	if (group is null)
		//	{
		//		await ReplyAsync($"no invitation to group with name \"{name}\" found");
		//		return;
		//	}
		//	try
		//	{
		//		await authorGroupManager.AcceptInvitation(authorOID, group.ID);
		//		await ReplyAsync($"accepted invitation to group with id \"{group.ID}\"");
		//	}
		//	catch (Exception ex)
		//	{
		//		await ReplyAsync(ex.Message);
		//	}
		//}

		//[Command("denyauthorinvitation")]
		//[Alias("dai", "denyauthorgroupinvitation", "dagi")]
		//[Summary("Denies an AuthorGroup invitation")]
		//public async Task DenyAuthorInvitation(Guid id)
		//{
		//	var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
		//	var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
		//	if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
		//	{
		//		await ReplyAsync(message: "slow down!!");
		//		return;
		//	}
		//	try
		//	{
		//		await authorGroupManager.DenyInvitation(authorOID, id);
		//		await ReplyAsync($"denied invitation to group with id \"{id}\"");
		//	}
		//	catch (Exception ex)
		//	{
		//		await ReplyAsync(ex.Message);
		//	}
		//}

		//[Command("denyauthorinvitation")]
		//[Alias("dai", "denyauthorgroupinvitation", "dagi")]
		//[Summary("Denies an AuthorGroup invitation")]
		//public async Task DenyAuthorInvitation([Remainder] string name)
		//{
		//	var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
		//	var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
		//	if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
		//	{
		//		await ReplyAsync(message: "slow down!!");
		//		return;
		//	}
		//	var invitations = await authorGroupRequestAccess.ReadAuthorGroupRequestRangeByAuthor(authorOID);
		//	if (!invitations.Any())
		//	{
		//		await ReplyAsync("you don't have any invitations!");
		//		return;
		//	}
		//	var group = (await authorGroupAccess.ReadAuthorGroups(invitations.Select(i => i.ID))).Where(i => i.Name == name).FirstOrDefault();
		//	if (group is null)
		//	{
		//		await ReplyAsync($"no invitation to group with name \"{name}\" found");
		//		return;
		//	}
		//	try
		//	{
		//		await authorGroupManager.DenyInvitation(authorOID, group.ID);
		//		await ReplyAsync($"denied invitation to group with id \"{group.ID}\"");
		//	}
		//	catch (Exception ex)
		//	{
		//		await ReplyAsync(ex.Message);
		//	}
		//}

		//[Command("updateauthor")]
		//[Alias("ua")]
		//[Summary("Updates an author in an AuthorGroup")]
		//public async Task UpdateAuthor(IUser user, Guid id, [Remainder] string[] permissionsParam)
		//{
		//	var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
		//	var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
		//	if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
		//	{
		//		await ReplyAsync(message: "slow down!!");
		//		return;
		//	}
		//	try
		//	{
		//		var permissions = permissionsParam
		//			.Select(Enum.Parse<AuthorGroupPermissionType>)
		//			.Aggregate((current, perm) => current | perm);
		//		var otherAuthorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, user.Id.ToString());
		//		var authorGroupPermission = new AuthorGroupPermission(id, otherAuthorOID, permissions);
		//		await authorGroupManager.UpdateAuthor(authorOID, authorGroupPermission);
		//		await ReplyAsync($"updated author with ID \"{user.Id}\" in group with ID \"{id}\"");
		//	}
		//	catch (Exception ex)
		//	{
		//		await ReplyAsync(ex.Message);
		//	}
		//}

		//[Command("updateauthor")]
		//[Alias("ua")]
		//[Summary("Updates an author in an AuthorGroup")]
		//public async Task UpdateAuthor(string otherAuthorID, Guid id, [Remainder] string[] permissionsParam)
		//{
		//	var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
		//	var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
		//	if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
		//	{
		//		await ReplyAsync(message: "slow down!!");
		//		return;
		//	}
		//	try
		//	{
		//		AuthorOID effectiveOtherAuthorOID;
		//		if (ulong.TryParse(otherAuthorID, out ulong result))
		//		{
		//			effectiveOtherAuthorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, result.ToString());
		//		}
		//		else
		//		{
		//			effectiveOtherAuthorOID = AuthorOID.Parse(otherAuthorID);
		//		}
		//		var permissions = permissionsParam
		//			.Select(Enum.Parse<AuthorGroupPermissionType>)
		//			.Aggregate((current, perm) => current | perm);
		//		var authorGroupPermission = new AuthorGroupPermission(id, effectiveOtherAuthorOID, permissions);
		//		await authorGroupManager.UpdateAuthor(authorOID, authorGroupPermission);
		//		await ReplyAsync($"updated author with ID \"{effectiveOtherAuthorOID}\" in group with ID \"{id}\"");
		//	}
		//	catch (Exception ex)
		//	{
		//		await ReplyAsync(ex.Message);
		//	}
		//}

		//[Command("updateauthor")]
		//[Alias("ua")]
		//[Summary("Updates an author in an AuthorGroup")]
		//public async Task UpdateAuthor(IUser user, [Remainder] string[] param)
		//{
		//	var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
		//	var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
		//	if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
		//	{
		//		await ReplyAsync(message: "slow down!!");
		//		return;
		//	}
		//	try
		//	{
		//		(string groupName, AuthorGroupPermissionType permissions) = ParseGroupAndPermissions(param);
		//		var group = await authorGroupManager.GetValidGroupFromNameAndPermission(authorOID, groupName, AuthorGroupPermissionType.AddAuthor);
		//		if (group is null)
		//		{
		//			await ReplyAsync($"could not find a group named \"{groupName}\" where author with ID \"{authorOID}\" has permission \"{AuthorGroupPermissionType.AddAuthor}\"");
		//			return;
		//		}
		//		var otherAuthorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, user.Id.ToString());
		//		var authorGroupPermission = new AuthorGroupPermission(group.ID, otherAuthorOID, permissions);
		//		await authorGroupManager.UpdateAuthor(authorOID, authorGroupPermission);
		//		await ReplyAsync($"updated author with ID \"{user.Id}\" in group with ID \"{group.ID}\"");
		//	}
		//	catch (Exception ex)
		//	{
		//		await ReplyAsync(ex.Message);
		//	}
		//}

		//[Command("updateauthor")]
		//[Alias("ua")]
		//[Summary("Updates an author in an AuthorGroup")]
		//public async Task UpdateAuthor(string otherAuthorID, [Remainder] string[] param)
		//{
		//	var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
		//	var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
		//	if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
		//	{
		//		await ReplyAsync(message: "slow down!!");
		//		return;
		//	}
		//	try
		//	{
		//		AuthorOID effectiveOtherAuthorOID;
		//		if (ulong.TryParse(otherAuthorID, out ulong result))
		//		{
		//			effectiveOtherAuthorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, result.ToString());
		//		}
		//		else
		//		{
		//			effectiveOtherAuthorOID = AuthorOID.Parse(otherAuthorID);
		//		}
		//		(string groupName, AuthorGroupPermissionType permissions) = ParseGroupAndPermissions(param);
		//		var group = await authorGroupManager.GetValidGroupFromNameAndPermission(authorOID, groupName, AuthorGroupPermissionType.AddAuthor);
		//		if (group is null)
		//		{
		//			await ReplyAsync($"could not find a group named \"{groupName}\" where author with ID \"{authorOID}\" has permission \"{AuthorGroupPermissionType.AddAuthor}\"");
		//			return;
		//		}
		//		var authorGroupPermission = new AuthorGroupPermission(group.ID, effectiveOtherAuthorOID, permissions);
		//		await authorGroupManager.UpdateAuthor(authorOID, authorGroupPermission);
		//		await ReplyAsync($"updated author with ID \"{effectiveOtherAuthorOID}\" in group with ID \"{group.ID}\"");
		//	}
		//	catch (Exception ex)
		//	{
		//		await ReplyAsync(ex.Message);
		//	}
		//}

		//[Command("removeauthor")]
		//[Alias("ra")]
		//[Summary("Removes an author from an AuthorGroup")]
		//public async Task RemoveAuthor(IUser user, Guid id)
		//{
		//	var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
		//	var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
		//	if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
		//	{
		//		await ReplyAsync(message: "slow down!!");
		//		return;
		//	}
		//	try
		//	{
		//		var otherAuthorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, user.Id.ToString());
		//		await authorGroupManager.RemoveAuthor(authorOID, id, otherAuthorOID);
		//		await ReplyAsync($"removed author with ID \"{otherAuthorOID}\" from group with ID \"{id}\"");
		//	}
		//	catch (Exception ex)
		//	{
		//		await ReplyAsync(ex.Message);
		//	}
		//}

		//[Command("removeauthor")]
		//[Alias("ra")]
		//[Summary("Removes an author from an AuthorGroup")]
		//public async Task RemoveAuthor(string otherAuthorID, Guid id)
		//{
		//	var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
		//	var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
		//	if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
		//	{
		//		await ReplyAsync(message: "slow down!!");
		//		return;
		//	}
		//	try
		//	{
		//		AuthorOID effectiveOtherAuthorOID;
		//		if (ulong.TryParse(otherAuthorID, out ulong result))
		//		{
		//			effectiveOtherAuthorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, result.ToString());
		//		}
		//		else
		//		{
		//			effectiveOtherAuthorOID = AuthorOID.Parse(otherAuthorID);
		//		}
		//		await authorGroupManager.RemoveAuthor(authorOID, id, effectiveOtherAuthorOID);
		//		await ReplyAsync($"removed author with ID \"{effectiveOtherAuthorOID}\" from group with ID \"{id}\"");
		//	}
		//	catch (Exception ex)
		//	{
		//		await ReplyAsync(ex.Message);
		//	}
		//}

		//[Command("removeauthor")]
		//[Alias("ra")]
		//[Summary("Removes an author from an AuthorGroup")]
		//public async Task RemoveAuthor(IUser user, [Remainder] string name)
		//{
		//	var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
		//	var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
		//	if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
		//	{
		//		await ReplyAsync(message: "slow down!!");
		//		return;
		//	}
		//	var group = await authorGroupManager.GetValidGroupFromNameAndPermission(authorOID, name, AuthorGroupPermissionType.RemoveAuthor);
		//	if (group is null)
		//	{
		//		await ReplyAsync($"could not find a group named \"{name}\" where author with ID \"{authorOID}\" has permission \"{AuthorGroupPermissionType.RemoveAuthor}\"");
		//		return;
		//	}
		//	try
		//	{
		//		var otherAuthorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, user.Id.ToString());
		//		await authorGroupManager.RemoveAuthor(authorOID, group.ID, otherAuthorOID);
		//		await ReplyAsync($"removed author with ID \"{otherAuthorOID}\" from group with ID \"{group.ID}\"");
		//	}
		//	catch (Exception ex)
		//	{
		//		await ReplyAsync(ex.Message);
		//	}
		//}

		//[Command("removeauthor")]
		//[Alias("ra")]
		//[Summary("Removes an author from an AuthorGroup")]
		//public async Task RemoveAuthor(string otherAuthorID, [Remainder] string name)
		//{
		//	var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
		//	var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
		//	if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
		//	{
		//		await ReplyAsync(message: "slow down!!");
		//		return;
		//	}
		//	var group = await authorGroupManager.GetValidGroupFromNameAndPermission(authorOID, name, AuthorGroupPermissionType.RemoveAuthor);
		//	if (group is null)
		//	{
		//		await ReplyAsync($"could not find a group named \"{name}\" where author with ID \"{authorOID}\" has permission \"{AuthorGroupPermissionType.RemoveAuthor}\"");
		//		return;
		//	}
		//	try
		//	{
		//		AuthorOID effectiveOtherAuthorOID;
		//		if (ulong.TryParse(otherAuthorID, out ulong result))
		//		{
		//			effectiveOtherAuthorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, result.ToString());
		//		}
		//		else
		//		{
		//			effectiveOtherAuthorOID = AuthorOID.Parse(otherAuthorID);
		//		}
		//		await authorGroupManager.RemoveAuthor(authorOID, group.ID, effectiveOtherAuthorOID);
		//		await ReplyAsync($"removed author with ID \"{effectiveOtherAuthorOID}\" from group with ID \"{group.ID}\"");
		//	}
		//	catch (Exception ex)
		//	{
		//		await ReplyAsync(ex.Message);
		//	}
		//}

		//[Command("leaveauthorgroup")]
		//[Alias("lag")]
		//[Summary("Leaves an AuthorGroup")]
		//public async Task LeaveAuthorGroup(Guid id)
		//{
		//	var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
		//	var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
		//	if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
		//	{
		//		await ReplyAsync(message: "slow down!!");
		//		return;
		//	}
		//	try
		//	{
		//		await authorGroupManager.LeaveGroup(authorOID, id);
		//		await ReplyAsync($"left group with ID \"{id}\"");
		//	}
		//	catch (Exception ex)
		//	{
		//		await ReplyAsync(ex.Message);
		//	}
		//}

		//[Command("leaveauthorgroup")]
		//[Alias("lag")]
		//[Summary("Leaves an AuthorGroup")]
		//public async Task LeaveAuthorGroup([Remainder] string name)
		//{
		//	var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
		//	var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
		//	if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
		//	{
		//		await ReplyAsync(message: "slow down!!");
		//		return;
		//	}
		//	var permissions = await authorGroupPermissionAccess.ReadAuthorGroupPermissionRangeByAuthor(authorOID);
		//	if (!permissions.Any())
		//	{
		//		await ReplyAsync("you're not in any groups!");
		//		return;
		//	}
		//	var group = (await authorGroupAccess.ReadAuthorGroups(permissions.Select(p => p.ID))).Where(g => g.Name == name).FirstOrDefault();
		//	if (group is null)
		//	{
		//		await ReplyAsync($"you're not in any group with the name \"{name}\"");
		//		return;
		//	}
		//	try
		//	{
		//		await authorGroupManager.LeaveGroup(authorOID, group.ID);
		//		await ReplyAsync($"left group with ID \"{group.ID}\"");
		//	}
		//	catch (Exception ex)
		//	{
		//		await ReplyAsync(ex.Message);
		//	}
		//}
	}
}
