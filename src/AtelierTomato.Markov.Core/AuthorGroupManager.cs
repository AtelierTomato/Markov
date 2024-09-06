using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Storage;

namespace AtelierTomato.Markov.Core
{
	public class AuthorGroupManager
	{
		private readonly IAuthorGroupPermissionAccess authorGroupAccess;
		public AuthorGroupManager(IAuthorGroupPermissionAccess authorGroupPermissionAccesss)
		{
			this.authorGroupAccess = authorGroupPermissionAccesss;
		}

		public async Task WritePermissionToGroup(AuthorOID sender, AuthorGroupPermission authorGroupPermission)
		{
			var senderAuthorGroupPermission = await authorGroupAccess.ReadAuthorGroupPermission(authorGroupPermission.ID, sender)
				?? throw new ArgumentException($"Author \"{sender}\" is not registered to group with ID \"{authorGroupPermission.ID}\".", nameof(sender));
			if (!senderAuthorGroupPermission.Permissions.Contains(AuthorGroupPermissionType.AddAuthor))
				throw new ArgumentException($"Author \"{sender}\" does not have permission to add authors to group with ID \"{authorGroupPermission.ID}\".", nameof(sender));
			foreach (var permission in authorGroupPermission.Permissions)
			{
				if (!senderAuthorGroupPermission.Permissions.Contains(permission))
					throw new ArgumentException($"Author \"{sender}\" does not have permission \"{permission}\", so they cannot give it to other authors that they add.", nameof(authorGroupPermission));
			}
			// All guards passed, allow write.
			await authorGroupAccess.WriteAuthorGroupPermission(authorGroupPermission);
		}

		public async Task RemoveAuthorFromGroup(AuthorOID sender, string groupID, AuthorOID user)
		{
			if (sender == user)
				throw new ArgumentException($"You cannot remove yourself from a group, if you would like to leave a group, please use the leave function instead.", nameof(user));
			var senderAuthorGroupPermission = await authorGroupAccess.ReadAuthorGroupPermission(groupID, sender)
				?? throw new ArgumentException($"Author \"{sender}\" is not registered to group with ID \"{groupID}\".", nameof(sender));
			if (!senderAuthorGroupPermission.Permissions.Contains(AuthorGroupPermissionType.RemoveAuthor))
				throw new ArgumentException($"Author \"{sender}\" does not have permission to remove authors from group with ID \"{groupID}\".", nameof(sender));
			// All guards passed, allow remove.
			await authorGroupAccess.DeleteAuthorFromAuthorGroup(groupID, user);
		}

		public async Task LeaveGroup(string groupID, AuthorOID sender) => await authorGroupAccess.DeleteAuthorFromAuthorGroup(groupID, sender);
	}
}
