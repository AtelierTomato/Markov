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
				throw new ArgumentException($"""Author "{sender}" is not the same {nameof(Author)} as the {nameof(Location.Owner)} "{locationOwner}" of {nameof(Location)} "{senderLocation}".""", nameof(senderLocation));

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

		public async Task RenameGroup(AuthorOID sender, Guid ID, string name)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));
			var senderGroupPermission = await locationGroupPermissionAccess.ReadLocationGroupPermissionsForOwner(ID, sender);
			if (!senderGroupPermission.HasFlag(LocationGroupPermissionType.RenameGroup))
				throw new ArgumentException($"""Author "{sender}" does not have permission to rename group with ID "{ID}".""", nameof(sender));

			// All guards passed, allow rename.
			await locationGroupAccess.WriteLocationGroup(new LocationGroup(ID, name));
		}

		public async Task DeleteGroup(AuthorOID sender, Guid ID)
		{
			var senderGroupPermission = await locationGroupPermissionAccess.ReadLocationGroupPermissionsForOwner(ID, sender);
			if (!senderGroupPermission.HasFlag(LocationGroupPermissionType.DeleteGroup))
				throw new ArgumentException($"""Author "{sender}" does not have permission to delete group with ID "{ID}".""", nameof(sender));

			// All guards passed, allow delete.
			await locationGroupAccess.DeleteLocationGroup(ID);
		}

		public async Task SendOrUpdateLocationGroupRequest(AuthorOID sender, LocationGroupPermission locationGroupPermission)
		{
			var senderGroupPermission = await locationGroupPermissionAccess.ReadLocationGroupPermissionsForOwner(locationGroupPermission.ID, sender);
			if (!senderGroupPermission.HasFlag(LocationGroupPermissionType.AddLocation))
				throw new ArgumentException($"""Author "{sender}" does not have permission to add locations to group with ID "{locationGroupPermission.ID}".""", nameof(sender));
			if ((await locationGroupPermissionAccess.ReadLocationGroupPermission(locationGroupPermission.ID, locationGroupPermission.Location)) is not null)
				throw new ArgumentException($"""Location "{locationGroupPermission.Location}" is already registered to group with ID "{locationGroupPermission.ID}".""", nameof(locationGroupPermission));

			// Check if any permissions to assign are not held by the sender
			if ((locationGroupPermission.Permissions & ~senderGroupPermission) != 0)
				throw new ArgumentException($"""Author "{sender}" does not have some of the permissions they are trying to assign.""", nameof(locationGroupPermission));

			// All guards passed, allow request.
			await locationGroupRequestAccess.WriteLocationGroupRequest(locationGroupPermission);
		}

		public async Task AcceptInvitation(AuthorOID sender, IObjectOID locationID, Guid ID)
		{
			var locationOwner = await locationAccess.ReadLocationOwner(locationID)
			 ?? throw new InvalidOperationException($"Cannot process {nameof(LocationGroup)}Request accepting as the database returned no {nameof(Location.Owner)} for the {nameof(Location)}. Either run command Refresh{nameof(Location)} or contact the owner of the bot.");
			if (sender != locationOwner)
				throw new ArgumentException($"""Author "{sender}" is not the same {nameof(Author)} as the {nameof(Location.Owner)} "{locationOwner}" of {nameof(Location)} "{locationID}".""", nameof(locationID));
			var senderLocationGroupRequest = await locationGroupRequestAccess.ReadLocationGroupRequest(ID, locationID)
			 ?? throw new ArgumentException($"""Location "{locationID}" has not been sent an invitation to group with ID "{ID}".""", nameof(ID));

			// All guards passed, allow accept.
			await locationGroupPermissionAccess.WriteLocationGroupPermission(senderLocationGroupRequest);
			await locationGroupRequestAccess.DeleteLocationGroupRequest(ID, locationID);
		}

		public async Task DenyInvitation(AuthorOID sender, IObjectOID locationID, Guid ID)
		{
			var locationOwner = await locationAccess.ReadLocationOwner(locationID)
			 ?? throw new InvalidOperationException($"Cannot process {nameof(LocationGroup)}Request denying as the database returned no {nameof(Location.Owner)} for the {nameof(Location)}. Either run command Refresh{nameof(Location)} or contact the owner of the bot.");
			if (sender != locationOwner)
				throw new ArgumentException($"""Author "{sender}" is not the same {nameof(Author)} as the {nameof(Location.Owner)} "{locationOwner}" of {nameof(Location)} "{locationID}".""", nameof(locationID));
			_ = await locationGroupRequestAccess.ReadLocationGroupRequest(ID, locationID)
			 ?? throw new ArgumentException($"""Location "{locationID}" has not been sent an invitation to group with ID "{ID}".""", nameof(ID));

			// All guards passed, allow deny.
			await locationGroupRequestAccess.DeleteLocationGroupRequest(ID, locationID);
		}

		public async Task UpdateLocation(AuthorOID sender, LocationGroupPermission locationGroupPermission)
		{
			var senderGroupPermission = await locationGroupPermissionAccess.ReadLocationGroupPermissionsForOwner(locationGroupPermission.ID, sender);
			if (!senderGroupPermission.HasFlag(LocationGroupPermissionType.AddLocation))
				throw new ArgumentException($"""Author "{sender}" does not have permission to add locations to group with ID "{locationGroupPermission.ID}".""", nameof(sender));

			// Check if any permissions to assign are not held by the sender
			if ((locationGroupPermission.Permissions & ~senderGroupPermission) != 0)
				throw new ArgumentException($"""Author "{sender}" does not have some of the permissions they are trying to assign.""", nameof(locationGroupPermission));

			// All guards passed, allow write.
			await locationGroupPermissionAccess.WriteLocationGroupPermission(locationGroupPermission);
		}

		public async Task RemoveLocation(AuthorOID sender, Guid ID, IObjectOID location)
		{
			var senderGroupPermission = await locationGroupPermissionAccess.ReadLocationGroupPermissionsForOwner(ID, sender);
			if (!senderGroupPermission.HasFlag(LocationGroupPermissionType.RemoveLocation))
				throw new ArgumentException($"""Author "{sender}" does not have permission to remove locations from group with ID "{ID}".""", nameof(sender));

			var locationGroupPermissions = await locationGroupPermissionAccess.ReadLocationGroupPermissionRangeByID(ID);
			if (!locationGroupPermissions.Select(p => p.Location).Contains(location))
				throw new ArgumentException($"""Location "{location}" is not registered to group with ID "{ID}".""", nameof(location));
			var locationGroupPerissionsWithDeleteGroup = locationGroupPermissions.Where(p => p.Permissions.HasFlag(LocationGroupPermissionType.DeleteGroup));
			if (!locationGroupPerissionsWithDeleteGroup.Any())
			{
				_logOrphanedLocationGroupWarning(logger, ID, null);
				throw new InvalidOperationException($"""The {nameof(LocationGroup)} with ID "{ID}" has no members with permission {nameof(LocationGroupPermissionType.DeleteGroup)}. This is unexpected.""");
			}
			if (locationGroupPerissionsWithDeleteGroup.Count() is 1)
				throw new ArgumentException($"""Location "{location}" cannot be removed from group with ID "{ID}" as it is the only member of it that has the permission {nameof(LocationGroupPermissionType.DeleteGroup)}. Please use "{nameof(DeleteGroup)}" function instead.""", nameof(location));

			if ((locationGroupPermissions.Where(p => p.Location == location).First().Permissions & ~senderGroupPermission) != 0)
				throw new ArgumentException($"""Author "{sender}" does not have some of the permissions that the location they are trying to remove has.""", nameof(location));

			// All guards passed, allow remove.
			await locationGroupPermissionAccess.DeleteLocationFromLocationGroup(ID, location);
		}

		private static readonly Action<ILogger, Guid, Exception?> _logOrphanedLocationGroupWarning =
			LoggerMessage.Define<Guid>(
				LogLevel.Warning,
				new EventId(3, nameof(RemoveLocation)),
				"""The LocationGroup with ID "{ID}" has no members with permission DeleteGroup. This is unexpected.""");
	}
}
