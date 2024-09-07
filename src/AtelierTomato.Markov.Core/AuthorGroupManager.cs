using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Storage;

namespace AtelierTomato.Markov.Core
{
	public class AuthorGroupManager
	{
		private readonly IAuthorGroupAccess authorGroupAccess;
		private readonly IAuthorGroupPermissionAccess authorGroupPermissionAccess;
		public AuthorGroupManager(IAuthorGroupAccess authorGroupAccess, IAuthorGroupPermissionAccess authorGroupPermissionAccess)
		{
			this.authorGroupAccess = authorGroupAccess;
			this.authorGroupPermissionAccess = authorGroupPermissionAccess;
		}

		public async Task CreateGroup(AuthorOID sender, string name)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));
			var ID = Guid.NewGuid();
			await authorGroupAccess.WriteAuthorGroup(new(ID, name));
			await authorGroupPermissionAccess.WriteAuthorGroupPermission(new(
				ID,
				sender,
				AuthorGroupPermissionType.Accepted |
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
			if (!senderAuthorGroupPermission.Permissions.HasFlag(AuthorGroupPermissionType.Accepted))
				throw new ArgumentException($"Author \"{sender}\" has not accepted the invitation into group with ID \"{ID}\".", nameof(sender));
			if (!senderAuthorGroupPermission.Permissions.HasFlag(AuthorGroupPermissionType.RenameGroup))
				throw new ArgumentException($"Author \"{sender}\" does not have permission to rename group with ID \"{ID}\".", nameof(sender));
			// All guards passed, allow rename.
			await authorGroupAccess.WriteAuthorGroup(new AuthorGroup(ID, name));
		}

		public async Task DeleteGroup(AuthorOID sender, Guid ID)
		{
			var senderAuthorGroupPermission = await authorGroupPermissionAccess.ReadAuthorGroupPermission(ID, sender)
			 ?? throw new ArgumentException($"Author \"{sender}\" is not registered to group with ID \"{ID}\".", nameof(sender));
			if (!senderAuthorGroupPermission.Permissions.HasFlag(AuthorGroupPermissionType.Accepted))
				throw new ArgumentException($"Author \"{sender}\" has not accepted the invitation into group with ID \"{ID}\".", nameof(sender));
			if (!senderAuthorGroupPermission.Permissions.HasFlag(AuthorGroupPermissionType.DeleteGroup))
				throw new ArgumentException($"Author \"{sender}\" does not have permission to delete group with ID \"{ID}\".", nameof(sender));
			// All guards passed, allow delete.
			await authorGroupAccess.DeleteAuthorGroup(ID);
		}

		public async Task AddOrUpdateAuthor(AuthorOID sender, AuthorGroupPermission authorGroupPermission)
		{
			if (sender == authorGroupPermission.Author)
				throw new ArgumentException($"You cannot update your own permissions in a group.", nameof(sender));
			var senderAuthorGroupPermission = await authorGroupPermissionAccess.ReadAuthorGroupPermission(authorGroupPermission.ID, sender)
			 ?? throw new ArgumentException($"Author \"{sender}\" is not registered to group with ID \"{authorGroupPermission.ID}\".", nameof(sender));
			if (!senderAuthorGroupPermission.Permissions.HasFlag(AuthorGroupPermissionType.Accepted))
				throw new ArgumentException($"Author \"{sender}\" has not accepted the invitation into group with ID \"{authorGroupPermission.ID}\".", nameof(sender));
			if (!senderAuthorGroupPermission.Permissions.HasFlag(AuthorGroupPermissionType.AddAuthor))
				throw new ArgumentException($"Author \"{sender}\" does not have permission to add authors to group with ID \"{authorGroupPermission.ID}\".", nameof(sender));
			if (authorGroupPermission.Permissions.HasFlag(AuthorGroupPermissionType.Accepted))
			{
				var authorGroupPermissionCurrent = await authorGroupPermissionAccess.ReadAuthorGroupPermission(authorGroupPermission.ID, authorGroupPermission.Author)
				 ?? throw new ArgumentException("An invite to a group cannot be sent already accpeted.", nameof(authorGroupPermission));
				if (!authorGroupPermissionCurrent.Permissions.HasFlag(AuthorGroupPermissionType.Accepted))
					throw new ArgumentException($"Author \"{authorGroupPermission.Author}\" has not accepted the invite to group with ID \"{authorGroupPermission.ID}\", you cannot accept it for them.", nameof(authorGroupPermission));
			}

			// Check if any permissions to assign are not held by the sender
			if ((authorGroupPermission.Permissions & ~senderAuthorGroupPermission.Permissions) != 0)
				throw new ArgumentException($"Author \"{sender}\" does not have some of the permissions they are trying to assign.", nameof(authorGroupPermission));

			// All guards passed, allow write.
			await authorGroupPermissionAccess.WriteAuthorGroupPermission(authorGroupPermission);
		}

		public async Task AcceptInvitation(AuthorOID sender, Guid ID)
		{
			var senderAuthorGroupPermission = await authorGroupPermissionAccess.ReadAuthorGroupPermission(ID, sender)
			 ?? throw new ArgumentException($"Author \"{sender}\" has not been sent an invitation to group with ID \"{ID}\".", nameof(ID));
			if (senderAuthorGroupPermission.Permissions.HasFlag(AuthorGroupPermissionType.Accepted))
				throw new ArgumentException($"Author \"{sender}\" has already accepted invitation into group with ID \"{ID}\".", nameof(ID));

			// All guards passed, allow accept.
			senderAuthorGroupPermission.Permissions |= AuthorGroupPermissionType.Accepted;
			await authorGroupPermissionAccess.WriteAuthorGroupPermission(senderAuthorGroupPermission);
		}

		public async Task RemoveAuthor(AuthorOID sender, Guid ID, AuthorOID user)
		{
			if (sender == user)
				throw new ArgumentException($"You cannot remove yourself from a group. Please use the \"{nameof(LeaveGroupOrDenyInvitation)}\" function instead.", nameof(user));
			var senderAuthorGroupPermission = await authorGroupPermissionAccess.ReadAuthorGroupPermission(ID, sender)
			 ?? throw new ArgumentException($"Author \"{sender}\" is not registered to group with ID \"{ID}\".", nameof(sender));
			if (!senderAuthorGroupPermission.Permissions.HasFlag(AuthorGroupPermissionType.Accepted))
				throw new ArgumentException($"Author \"{sender}\" has not accepted the invitation into group with ID \"{ID}\".", nameof(sender));
			if (!senderAuthorGroupPermission.Permissions.HasFlag(AuthorGroupPermissionType.RemoveAuthor))
				throw new ArgumentException($"Author \"{sender}\" does not have permission to remove authors from group with ID \"{ID}\".", nameof(sender));
			// All guards passed, allow remove.
			await authorGroupPermissionAccess.DeleteAuthorFromAuthorGroup(ID, user);
		}

		public async Task LeaveGroupOrDenyInvitation(Guid groupID, AuthorOID sender)
		{
			var authorGroupPermissions = await authorGroupPermissionAccess.ReadAuthorGroupPermissionRangeByID(groupID);
			if (!authorGroupPermissions.Select(p => p.Author).Contains(sender))
				throw new ArgumentException($"Author \"{sender}\" cannot leave group with ID \"{groupID}\" as they are not registered to it.", nameof(sender));
			if (authorGroupPermissions.Count() is 1)
				throw new ArgumentException($"Author \"{sender}\" cannot leave group with ID \"{groupID}\" as they are the only member of it. Please use \"{nameof(DeleteGroup)}\" function instead.", nameof(sender));
			// All guards passed, allow leave.
			await authorGroupPermissionAccess.DeleteAuthorFromAuthorGroup(groupID, sender);
		}
	}
}
