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
				[
					AuthorGroupPermissionType.SentencesInGroup,
					AuthorGroupPermissionType.UseGroup,
					AuthorGroupPermissionType.AddAuthor,
					AuthorGroupPermissionType.RemoveAuthor,
					AuthorGroupPermissionType.RenameGroup,
					AuthorGroupPermissionType.DeleteGroup
				]
			));
		}

		public async Task RenameGroup(AuthorOID sender, Guid ID, string name)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));
			var senderAuthorGroupPermission = await authorGroupPermissionAccess.ReadAuthorGroupPermission(ID, sender)
			 ?? throw new ArgumentException($"Author \"{sender}\" is not registered to group with ID \"{ID}\".", nameof(sender));
			if (!senderAuthorGroupPermission.Permissions.Contains(AuthorGroupPermissionType.RenameGroup))
				throw new ArgumentException($"Author \"{sender}\" does not have permission to rename group with ID \"{ID}\".", nameof(sender));
			// All guards passed, allow rename.
			await authorGroupAccess.WriteAuthorGroup(new AuthorGroup(ID, name));
		}

		public async Task DeleteGroup(AuthorOID sender, Guid ID)
		{
			var senderAuthorGroupPermission = await authorGroupPermissionAccess.ReadAuthorGroupPermission(ID, sender)
			 ?? throw new ArgumentException($"Author \"{sender}\" is not registered to group with ID \"{ID}\".", nameof(sender));
			if (!senderAuthorGroupPermission.Permissions.Contains(AuthorGroupPermissionType.DeleteGroup))
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
			if (!senderAuthorGroupPermission.Permissions.Contains(AuthorGroupPermissionType.AddAuthor))
				throw new ArgumentException($"Author \"{sender}\" does not have permission to add authors to group with ID \"{authorGroupPermission.ID}\".", nameof(sender));
			foreach (var permission in authorGroupPermission.Permissions)
			{
				if (!senderAuthorGroupPermission.Permissions.Contains(permission))
					throw new ArgumentException($"Author \"{sender}\" does not have permission \"{permission}\", so they cannot give it to other authors that they add.", nameof(authorGroupPermission));
			}
			// All guards passed, allow write.
			await authorGroupPermissionAccess.WriteAuthorGroupPermission(authorGroupPermission);
		}

		public async Task RemoveAuthor(AuthorOID sender, Guid groupID, AuthorOID user)
		{
			if (sender == user)
				throw new ArgumentException($"You cannot remove yourself from a group. Please use the \"{nameof(LeaveGroup)}\" function instead.", nameof(user));
			var senderAuthorGroupPermission = await authorGroupPermissionAccess.ReadAuthorGroupPermission(groupID, sender)
			 ?? throw new ArgumentException($"Author \"{sender}\" is not registered to group with ID \"{groupID}\".", nameof(sender));
			if (!senderAuthorGroupPermission.Permissions.Contains(AuthorGroupPermissionType.RemoveAuthor))
				throw new ArgumentException($"Author \"{sender}\" does not have permission to remove authors from group with ID \"{groupID}\".", nameof(sender));
			// All guards passed, allow remove.
			await authorGroupPermissionAccess.DeleteAuthorFromAuthorGroup(groupID, user);
		}

		public async Task LeaveGroup(Guid groupID, AuthorOID sender)
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
