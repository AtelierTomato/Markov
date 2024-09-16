using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Storage;
using Microsoft.Extensions.Logging;

namespace AtelierTomato.Markov.Core
{
	public class AuthorGroupManager
	{
		private readonly IAuthorGroupAccess authorGroupAccess;
		private readonly IAuthorGroupPermissionAccess authorGroupPermissionAccess;
		private readonly IAuthorGroupRequestAccess authorGroupRequestAccess;
		private readonly ILogger<AuthorGroupManager> logger;
		public AuthorGroupManager(IAuthorGroupAccess authorGroupAccess, IAuthorGroupPermissionAccess authorGroupPermissionAccess, IAuthorGroupRequestAccess authorGroupRequestAccess, ILogger<AuthorGroupManager> logger)
		{
			this.authorGroupAccess = authorGroupAccess;
			this.authorGroupPermissionAccess = authorGroupPermissionAccess;
			this.authorGroupRequestAccess = authorGroupRequestAccess;
			this.logger = logger;
		}

		public async Task CreateGroup(AuthorOID sender, string name)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			// All guards passed, allow create.
			var ID = Guid.NewGuid();
			await authorGroupAccess.WriteAuthorGroup(new(ID, name));
			await authorGroupPermissionAccess.WriteAuthorGroupPermission(new(
				ID,
				sender,
				AuthorGroupPermissionType.SentencesInGroup |
				AuthorGroupPermissionType.UseGroup |
				AuthorGroupPermissionType.AddAuthor |
				AuthorGroupPermissionType.RemoveAuthor |
				AuthorGroupPermissionType.RenameGroup |
				AuthorGroupPermissionType.DeleteGroup
			));
		}

		public async Task RenameGroup(AuthorOID sender, Guid ID, string name)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));
			var senderAuthorGroupPermission = await authorGroupPermissionAccess.ReadAuthorGroupPermission(ID, sender)
			 ?? throw new ArgumentException($"Author \"{sender}\" is not registered to group with ID \"{ID}\".", nameof(sender));
			if (!senderAuthorGroupPermission.Permissions.HasFlag(AuthorGroupPermissionType.RenameGroup))
				throw new ArgumentException($"Author \"{sender}\" does not have permission to rename group with ID \"{ID}\".", nameof(sender));

			// All guards passed, allow rename.
			await authorGroupAccess.WriteAuthorGroup(new AuthorGroup(ID, name));
		}

		public async Task DeleteGroup(AuthorOID sender, Guid ID)
		{
			var senderAuthorGroupPermission = await authorGroupPermissionAccess.ReadAuthorGroupPermission(ID, sender)
			 ?? throw new ArgumentException($"Author \"{sender}\" is not registered to group with ID \"{ID}\".", nameof(sender));
			if (!senderAuthorGroupPermission.Permissions.HasFlag(AuthorGroupPermissionType.DeleteGroup))
				throw new ArgumentException($"Author \"{sender}\" does not have permission to delete group with ID \"{ID}\".", nameof(sender));

			// All guards passed, allow delete.
			await authorGroupAccess.DeleteAuthorGroup(ID);
		}

		public async Task SendOrUpdateAuthorGroupRequest(AuthorOID sender, AuthorGroupPermission authorGroupPermission)
		{
			if (sender == authorGroupPermission.Author)
				throw new ArgumentException($"You cannot invite yourself to a group.", nameof(sender));
			var senderAuthorGroupPermission = await authorGroupPermissionAccess.ReadAuthorGroupPermission(authorGroupPermission.ID, sender)
			 ?? throw new ArgumentException($"Author \"{sender}\" is not registered to group with ID \"{authorGroupPermission.ID}\".", nameof(sender));
			if (!senderAuthorGroupPermission.Permissions.HasFlag(AuthorGroupPermissionType.AddAuthor))
				throw new ArgumentException($"Author \"{sender}\" does not have permission to add authors to group with ID \"{authorGroupPermission.ID}\".", nameof(sender));
			if ((await authorGroupPermissionAccess.ReadAuthorGroupPermission(authorGroupPermission.ID, authorGroupPermission.Author)) is not null)
				throw new ArgumentException($"Author \"{authorGroupPermission.Author}\" is already registered to group with ID \"{authorGroupPermission.ID}\".", nameof(authorGroupPermission));

			// Check if any permissions to assign are not held by the sender
			if ((authorGroupPermission.Permissions & ~senderAuthorGroupPermission.Permissions) != 0)
				throw new ArgumentException($"Author \"{sender}\" does not have some of the permissions they are trying to assign.", nameof(authorGroupPermission));

			// All guards passed, allow request.
			await authorGroupRequestAccess.WriteAuthorGroupRequest(authorGroupPermission);
		}

		public async Task AcceptInvitation(AuthorOID sender, Guid ID)
		{
			var senderAuthorGroupRequest = await authorGroupRequestAccess.ReadAuthorGroupRequest(ID, sender)
			 ?? throw new ArgumentException($"Author \"{sender}\" has not been sent an invitation to group with ID \"{ID}\".", nameof(ID));

			// All guards passed, allow accept.
			await authorGroupPermissionAccess.WriteAuthorGroupPermission(senderAuthorGroupRequest);
			await authorGroupRequestAccess.DeleteAuthorGroupRequest(ID, sender);
		}

		public async Task DenyInvitation(AuthorOID sender, Guid ID)
		{
			_ = await authorGroupRequestAccess.ReadAuthorGroupRequest(ID, sender)
			 ?? throw new ArgumentException($"Author \"{sender}\" has not been sent an invitation to group with ID \"{ID}\".", nameof(ID));

			// All guards passed, allow deny.
			await authorGroupRequestAccess.DeleteAuthorGroupRequest(ID, sender);
		}

		public async Task UpdateAuthor(AuthorOID sender, AuthorGroupPermission authorGroupPermission)
		{
			if (sender == authorGroupPermission.Author)
				throw new ArgumentException($"You cannot update your own permissions in a group.", nameof(sender));
			var senderAuthorGroupPermission = await authorGroupPermissionAccess.ReadAuthorGroupPermission(authorGroupPermission.ID, sender)
			 ?? throw new ArgumentException($"Author \"{sender}\" is not registered to group with ID \"{authorGroupPermission.ID}\".", nameof(sender));
			if (!senderAuthorGroupPermission.Permissions.HasFlag(AuthorGroupPermissionType.AddAuthor))
				throw new ArgumentException($"Author \"{sender}\" does not have permission to add authors to group with ID \"{authorGroupPermission.ID}\".", nameof(sender));

			// Check if any permissions to assign are not held by the sender
			if ((authorGroupPermission.Permissions & ~senderAuthorGroupPermission.Permissions) != 0)
				throw new ArgumentException($"Author \"{sender}\" does not have some of the permissions they are trying to assign.", nameof(authorGroupPermission));

			// All guards passed, allow write.
			await authorGroupPermissionAccess.WriteAuthorGroupPermission(authorGroupPermission);
		}

		public async Task RemoveAuthor(AuthorOID sender, Guid ID, AuthorOID user)
		{
			if (sender == user)
				throw new ArgumentException($"You cannot remove yourself from a group. Please use the \"{nameof(LeaveGroup)}\" function instead.", nameof(user));
			var senderAuthorGroupPermission = await authorGroupPermissionAccess.ReadAuthorGroupPermission(ID, sender)
			 ?? throw new ArgumentException($"Author \"{sender}\" is not registered to group with ID \"{ID}\".", nameof(sender));
			if (!senderAuthorGroupPermission.Permissions.HasFlag(AuthorGroupPermissionType.RemoveAuthor))
				throw new ArgumentException($"Author \"{sender}\" does not have permission to remove authors from group with ID \"{ID}\".", nameof(sender));
			var authorGroupPermission = await authorGroupPermissionAccess.ReadAuthorGroupPermission(ID, user)
			 ?? throw new ArgumentException($"Author \"{user}\" is not registered to group with ID \"{ID}\".", nameof(user));
			if ((authorGroupPermission.Permissions & ~senderAuthorGroupPermission.Permissions) != 0)
				throw new ArgumentException($"Author \"{sender}\" does not have some of the permissions that the author they are trying to remove has.", nameof(sender));

			// All guards passed, allow remove.
			await authorGroupPermissionAccess.DeleteAuthorFromAuthorGroup(ID, user);
		}

		public async Task LeaveGroup(AuthorOID sender, Guid ID)
		{
			var authorGroupPermissions = await authorGroupPermissionAccess.ReadAuthorGroupPermissionRangeByID(ID);
			if (!authorGroupPermissions.Select(p => p.Author).Contains(sender))
				throw new ArgumentException($"Author \"{sender}\" cannot leave group with ID \"{ID}\" as they are not registered to it.", nameof(sender));
			var authorGroupPerissionsWithDeleteGroup = authorGroupPermissions.Where(p => p.Permissions.HasFlag(AuthorGroupPermissionType.DeleteGroup));
			if (!authorGroupPerissionsWithDeleteGroup.Any())
			{
				_logOrphanedGroupWarning(logger, ID, null);
				throw new InvalidOperationException($"The {nameof(AuthorGroup)} with ID \"{ID}\" has no members with permission {nameof(AuthorGroupPermissionType.DeleteGroup)}. This is unexpected.");
			}
			if (authorGroupPerissionsWithDeleteGroup.Count() is 1)
				throw new ArgumentException($"Author \"{sender}\" cannot leave group with ID \"{ID}\" as they are the only member of it that has the permission {nameof(AuthorGroupPermissionType.DeleteGroup)}. Please use \"{nameof(DeleteGroup)}\" function instead.", nameof(sender));

			// All guards passed, allow leave.
			await authorGroupPermissionAccess.DeleteAuthorFromAuthorGroup(ID, sender);
		}

		private static readonly Action<ILogger, Guid, Exception?> _logOrphanedGroupWarning =
			LoggerMessage.Define<Guid>(
				LogLevel.Warning,
				new EventId(2, nameof(LeaveGroup)),
				"The AuthorGroup with ID \"{ID}\" has no members with permission DeleteGroup. This is unexpected.");
	}
}
