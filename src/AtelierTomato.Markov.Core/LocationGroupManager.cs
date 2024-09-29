using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Storage;
using Microsoft.Extensions.Logging;

namespace AtelierTomato.Markov.Core
{
	public class LocationGroupManager
	{
		private readonly ILocationAccess locationAccess;
		private readonly ILocationGroupAccess locationGroupAccess;
		private readonly ILocationGroupPermissionAccess locationGroupPermissionAccess;
		private readonly ILocationGroupRequestAccess locationGroupRequestAccess;
		private readonly ILogger<LocationGroupManager> logger;
		public LocationGroupManager(ILocationAccess locationAccess, ILocationGroupAccess locationGroupAccess, ILocationGroupPermissionAccess locationGroupPermissionAccess, ILocationGroupRequestAccess locationGroupRequestAccess, ILogger<LocationGroupManager> logger)
		{
			this.locationAccess = locationAccess;
			this.locationGroupAccess = locationGroupAccess;
			this.locationGroupPermissionAccess = locationGroupPermissionAccess;
			this.locationGroupRequestAccess = locationGroupRequestAccess;
			this.logger = logger;
		}

		public async Task CreateGroup(AuthorOID sender, IObjectOID senderLocation, string name)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));
			var locationOwner = await locationAccess.ReadLocationOwner(senderLocation)
			 ?? throw new InvalidOperationException($"Cannot process {nameof(LocationGroup)} creation as the database returned no {nameof(Location.Owner)} for the {nameof(Location)}. Either run command Refresh{nameof(Location)} or contact the owner of the bot.");
			if (sender != locationOwner)
				throw new ArgumentException($"""Sender "{sender}" is not the same {nameof(Author)} as the {nameof(Location.Owner)} "{locationOwner}" of {nameof(Location)} "{senderLocation}".""", nameof(senderLocation));

			// All guards passed, allow create.
			var ID = Guid.NewGuid();
			await locationGroupAccess.WriteLocationGroup(new(ID, name));
			await locationGroupPermissionAccess.WriteLocationGroupPermission(new(
				ID,
				senderLocation,
				LocationGroupPermissionType.SentencesInGroup |
				LocationGroupPermissionType.UseGroup |
				LocationGroupPermissionType.AddLocation |
				LocationGroupPermissionType.RemoveLocation |
				LocationGroupPermissionType.RenameGroup |
				LocationGroupPermissionType.DeleteGroup
			));
		}

		public async Task RenameGroup(AuthorOID sender, IObjectOID senderLocation, Guid ID, string name)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));
			var locationOwner = await locationAccess.ReadLocationOwner(senderLocation)
			 ?? throw new InvalidOperationException($"Cannot process {nameof(LocationGroup)} rename as the database returned no {nameof(Location.Owner)} for the {nameof(Location)}. Either run command Refresh{nameof(Location)} or contact the owner of the bot.");
			if (sender != locationOwner)
				throw new ArgumentException($"""Sender "{sender}" is not the same {nameof(Author)} as the {nameof(Location.Owner)} "{locationOwner}" of {nameof(Location)} "{senderLocation}".""", nameof(senderLocation));
			var senderLocationGroupPermission = await locationGroupPermissionAccess.ReadLocationGroupPermission(ID, senderLocation)
			 ?? throw new ArgumentException($"""Location "{senderLocation}" is not registered to group with ID "{ID}".""", nameof(senderLocation));
			if (!senderLocationGroupPermission.Permissions.HasFlag(LocationGroupPermissionType.RenameGroup))
				throw new ArgumentException($"""Location "{senderLocation}" does not have permission to rename group with ID "{ID}".""", nameof(senderLocation));

			// All guards passed, allow rename.
			await locationGroupAccess.WriteLocationGroup(new LocationGroup(ID, name));
		}

		public async Task DeleteGroup(AuthorOID sender, IObjectOID senderLocation, Guid ID)
		{
			var locationOwner = await locationAccess.ReadLocationOwner(senderLocation)
			 ?? throw new InvalidOperationException($"Cannot process {nameof(LocationGroup)} deletion as the database returned no {nameof(Location.Owner)} for the {nameof(Location)}. Either run command Refresh{nameof(Location)} or contact the owner of the bot.");
			if (sender != locationOwner)
				throw new ArgumentException($"""Sender "{sender}" is not the same {nameof(Author)} as the {nameof(Location.Owner)} "{locationOwner}" of {nameof(Location)} "{senderLocation}".""", nameof(senderLocation));
			var senderLocationGroupPermission = await locationGroupPermissionAccess.ReadLocationGroupPermission(ID, senderLocation)
			 ?? throw new ArgumentException($"""Location "{senderLocation}" is not registered to group with ID "{ID}".""", nameof(senderLocation));
			if (!senderLocationGroupPermission.Permissions.HasFlag(LocationGroupPermissionType.DeleteGroup))
				throw new ArgumentException($"""Location "{senderLocation}" does not have permission to delete group with ID "{ID}".""", nameof(senderLocation));

			// All guards passed, allow delete.
			await locationGroupAccess.DeleteLocationGroup(ID);
		}

		public async Task SendOrUpdateLocationGroupRequest(AuthorOID sender, IObjectOID senderLocation, LocationGroupPermission locationGroupPermission)
		{
			if (senderLocation == locationGroupPermission.Location)
				throw new ArgumentException($"You cannot invite your own location to a group.", nameof(senderLocation));
			var locationOwner = await locationAccess.ReadLocationOwner(senderLocation)
			 ?? throw new InvalidOperationException($"Cannot process {nameof(LocationGroup)}Request as the database returned no {nameof(Location.Owner)} for the {nameof(Location)}. Either run command Refresh{nameof(Location)} or contact the owner of the bot.");
			if (sender != locationOwner)
				throw new ArgumentException($"""Sender "{sender}" is not the same {nameof(Author)} as the {nameof(Location.Owner)} "{locationOwner}" of {nameof(Location)} "{senderLocation}".""", nameof(senderLocation));
			var senderLocationGroupPermission = await locationGroupPermissionAccess.ReadLocationGroupPermission(locationGroupPermission.ID, senderLocation)
			 ?? throw new ArgumentException($"""Location "{senderLocation}" is not registered to group with ID "{locationGroupPermission.ID}".""", nameof(senderLocation));
			if (!senderLocationGroupPermission.Permissions.HasFlag(LocationGroupPermissionType.AddLocation))
				throw new ArgumentException($"""Location "{senderLocation}" does not have permission to add locations to group with ID "{locationGroupPermission.ID}".""", nameof(senderLocation));
			if ((await locationGroupPermissionAccess.ReadLocationGroupPermission(locationGroupPermission.ID, locationGroupPermission.Location)) is not null)
				throw new ArgumentException($"""Location "{locationGroupPermission.Location}" is already registered to group with ID "{locationGroupPermission.ID}".""", nameof(locationGroupPermission));

			// Check if any permissions to assign are not held by the sender
			if ((locationGroupPermission.Permissions & ~senderLocationGroupPermission.Permissions) != 0)
				throw new ArgumentException($"""Location "{senderLocation}" does not have some of the permissions it is trying to assign.""", nameof(locationGroupPermission));

			// All guards passed, allow request.
			await locationGroupRequestAccess.WriteLocationGroupRequest(locationGroupPermission);
		}

		public async Task AcceptInvitation(AuthorOID sender, IObjectOID senderLocation, Guid ID)
		{
			var locationOwner = await locationAccess.ReadLocationOwner(senderLocation)
			 ?? throw new InvalidOperationException($"Cannot process {nameof(LocationGroup)}Request accepting as the database returned no {nameof(Location.Owner)} for the {nameof(Location)}. Either run command Refresh{nameof(Location)} or contact the owner of the bot.");
			if (sender != locationOwner)
				throw new ArgumentException($"""Sender "{sender}" is not the same {nameof(Author)} as the {nameof(Location.Owner)} "{locationOwner}" of {nameof(Location)} "{senderLocation}".""", nameof(senderLocation));
			var senderLocationGroupRequest = await locationGroupRequestAccess.ReadLocationGroupRequest(ID, senderLocation)
			 ?? throw new ArgumentException($"""Location "{senderLocation}" has not been sent an invitation to group with ID "{ID}".""", nameof(ID));

			// All guards passed, allow accept.
			await locationGroupPermissionAccess.WriteLocationGroupPermission(senderLocationGroupRequest);
			await locationGroupRequestAccess.DeleteLocationGroupRequest(ID, senderLocation);
		}

		public async Task DenyInvitation(AuthorOID sender, IObjectOID senderLocation, Guid ID)
		{
			var locationOwner = await locationAccess.ReadLocationOwner(senderLocation)
			 ?? throw new InvalidOperationException($"Cannot process {nameof(LocationGroup)}Request denying as the database returned no {nameof(Location.Owner)} for the {nameof(Location)}. Either run command Refresh{nameof(Location)} or contact the owner of the bot.");
			if (sender != locationOwner)
				throw new ArgumentException($"""Sender "{sender}" is not the same {nameof(Author)} as the {nameof(Location.Owner)} "{locationOwner}" of {nameof(Location)} "{senderLocation}".""", nameof(senderLocation));
			_ = await locationGroupRequestAccess.ReadLocationGroupRequest(ID, senderLocation)
			 ?? throw new ArgumentException($"""Location "{senderLocation}" has not been sent an invitation to group with ID "{ID}".""", nameof(ID));

			// All guards passed, allow deny.
			await locationGroupRequestAccess.DeleteLocationGroupRequest(ID, senderLocation);
		}

		public async Task UpdateLocation(AuthorOID sender, IObjectOID senderLocation, LocationGroupPermission locationGroupPermission)
		{
			if (senderLocation == locationGroupPermission.Location)
				throw new ArgumentException($"You cannot update your own location's permissions in a group.", nameof(senderLocation));
			var locationOwner = await locationAccess.ReadLocationOwner(senderLocation)
			 ?? throw new InvalidOperationException($"Cannot process {nameof(LocationGroupPermission)} update as the database returned no {nameof(Location.Owner)} for the {nameof(Location)}. Either run command Refresh{nameof(Location)} or contact the owner of the bot.");
			if (sender != locationOwner)
				throw new ArgumentException($"""Sender "{sender}" is not the same {nameof(Author)} as the {nameof(Location.Owner)} "{locationOwner}" of {nameof(Location)} "{senderLocation}".""", nameof(senderLocation));
			var senderLocationGroupPermission = await locationGroupPermissionAccess.ReadLocationGroupPermission(locationGroupPermission.ID, senderLocation)
			 ?? throw new ArgumentException($"""Location "{senderLocation}" is not registered to group with ID "{locationGroupPermission.ID}".""", nameof(senderLocation));
			if (!senderLocationGroupPermission.Permissions.HasFlag(LocationGroupPermissionType.AddLocation))
				throw new ArgumentException($"""Location "{senderLocation}" does not have permission to add locations to group with ID "{locationGroupPermission.ID}".""", nameof(senderLocation));

			// Check if any permissions to assign are not held by the sender
			if ((locationGroupPermission.Permissions & ~senderLocationGroupPermission.Permissions) != 0)
				throw new ArgumentException($"""Location "{senderLocation}" does not have some of the permissions it is trying to assign.""", nameof(locationGroupPermission));

			// All guards passed, allow write.
			await locationGroupPermissionAccess.WriteLocationGroupPermission(locationGroupPermission);
		}

		public async Task RemoveLocation(AuthorOID sender, IObjectOID senderLocation, Guid ID, IObjectOID location)
		{
			if (senderLocation == location)
				throw new ArgumentException($"""You cannot remove your own location from a group. Please use the "{nameof(LeaveGroup)}" function instead.""", nameof(location));
			var locationOwner = await locationAccess.ReadLocationOwner(senderLocation)
			 ?? throw new InvalidOperationException($"Cannot process {nameof(Location)} removal as the database returned no {nameof(Location.Owner)} for the {nameof(Location)}. Either run command Refresh{nameof(Location)} or contact the owner of the bot.");
			if (sender != locationOwner)
				throw new ArgumentException($"""Sender "{sender}" is not the same {nameof(Author)} as the {nameof(Location.Owner)} "{locationOwner}" of {nameof(Location)} "{location}".""", nameof(location));
			var senderLocationGroupPermission = await locationGroupPermissionAccess.ReadLocationGroupPermission(ID, senderLocation)
			 ?? throw new ArgumentException($"""Location "{senderLocation}" is not registered to group with ID "{ID}".""", nameof(senderLocation));
			if (!senderLocationGroupPermission.Permissions.HasFlag(LocationGroupPermissionType.RemoveLocation))
				throw new ArgumentException($"""Location "{senderLocation}" does not have permission to remove locations from group with ID "{ID}".""", nameof(senderLocation));
			var locationGroupPermission = await locationGroupPermissionAccess.ReadLocationGroupPermission(ID, location)
			 ?? throw new ArgumentException($"""Location "{location}" is not registered to group with ID "{ID}".""", nameof(location));
			if ((locationGroupPermission.Permissions & ~senderLocationGroupPermission.Permissions) != 0)
				throw new ArgumentException($"""Location "{senderLocation}" does not have some of the permissions that the location it is trying to remove has.""", nameof(location));

			// All guards passed, allow remove.
			await locationGroupPermissionAccess.DeleteLocationFromLocationGroup(ID, location);
		}

		public async Task LeaveGroup(AuthorOID sender, IObjectOID senderLocation, Guid ID)
		{
			var locationOwner = await locationAccess.ReadLocationOwner(senderLocation)
			 ?? throw new InvalidOperationException($"Cannot process {nameof(LocationGroup)} deletion as the database returned no {nameof(Location.Owner)} for the {nameof(Location)}. Either run command Refresh{nameof(Location)} or contact the owner of the bot.");
			if (sender != locationOwner)
				throw new ArgumentException($"""Sender "{sender}" is not the same {nameof(Author)} as the {nameof(Location.Owner)} "{locationOwner}" of {nameof(Location)} "{senderLocation}".""", nameof(senderLocation));
			var locationGroupPermissions = await locationGroupPermissionAccess.ReadLocationGroupPermissionRangeByID(ID);
			if (!locationGroupPermissions.Select(p => p.Location).Contains(senderLocation))
				throw new ArgumentException($"""Location "{senderLocation}" cannot leave group with ID "{ID}" as they are not registered to it.""", nameof(senderLocation));
			var locationGroupPerissionsWithDeleteGroup = locationGroupPermissions.Where(p => p.Permissions.HasFlag(LocationGroupPermissionType.DeleteGroup));
			if (!locationGroupPerissionsWithDeleteGroup.Any())
			{
				_logOrphanedGroupWarning(logger, ID, null);
				throw new InvalidOperationException($"""The {nameof(LocationGroup)} with ID "{ID}" has no members with permission {nameof(LocationGroupPermissionType.DeleteGroup)}. This is unexpected.""");
			}
			if (locationGroupPerissionsWithDeleteGroup.Count() is 1)
				throw new ArgumentException($"""Location "{senderLocation}" cannot leave group with ID "{ID}" as it is the only member of it that has the permission {nameof(LocationGroupPermissionType.DeleteGroup)}. Please use "{nameof(DeleteGroup)}" function instead.""", nameof(sender));

			// All guards passed, allow leave.
			await locationGroupPermissionAccess.DeleteLocationFromLocationGroup(ID, senderLocation);
		}

		private static readonly Action<ILogger, Guid, Exception?> _logOrphanedGroupWarning =
			LoggerMessage.Define<Guid>(
				LogLevel.Warning,
				new EventId(2, nameof(LeaveGroup)),
				"""The LocationGroup with ID "{ID}" has no members with permission DeleteGroup. This is unexpected.""");
	}
}
