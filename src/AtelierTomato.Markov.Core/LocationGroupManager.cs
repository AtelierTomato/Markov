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
		private readonly ILocationSettingAccess locationSettingAccess;
		private readonly IAuthorRetortConfigAccess authorRetortConfigAccess;
		private readonly ILogger<LocationGroupManager> logger;
		public LocationGroupManager(ILocationAccess locationAccess, ILocationGroupAccess locationGroupAccess, ILocationGroupPermissionAccess locationGroupPermissionAccess, ILocationGroupRequestAccess locationGroupRequestAccess, ILocationSettingAccess locationSettingAccess, IAuthorRetortConfigAccess authorRetortConfigAccess, ILogger<LocationGroupManager> logger)
		{
			this.locationAccess = locationAccess;
			this.locationGroupAccess = locationGroupAccess;
			this.locationGroupPermissionAccess = locationGroupPermissionAccess;
			this.locationGroupRequestAccess = locationGroupRequestAccess;
			this.locationSettingAccess = locationSettingAccess;
			this.authorRetortConfigAccess = authorRetortConfigAccess;
			this.logger = logger;
		}

		public async Task<Guid> CreateGroup(AuthorOID sender, IObjectOID senderLocation, string name)
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
			return ID;
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

			// Check if Location is already in group
			_ = await locationGroupPermissionAccess.ReadLocationGroupPermission(locationGroupPermission.ID, locationGroupPermission.Location) ??
				throw new ArgumentException($"""Location "{locationGroupPermission.Location}" is not a member of group with ID "{locationGroupPermission.ID}".""");

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

		/// <summary>
		/// Gets the currently in use LocationGroup for the location, the LocationGroup the user would like to use, and the Filter that the user would like to use,
		/// finds the intersecting locations in both location groups, and the intersection locations in the filter, returns the result.
		/// </summary>
		/// <param name="author"></param>
		/// <param name="location"></param>
		/// <returns></returns>
		public async Task<IEnumerable<IObjectOID>> GetLocationsForFilter(AuthorOID author, IObjectOID location)
		{
			IEnumerable<IObjectOID> usableLocations = [location.Base()];
			var locationSettingHierarchy = await locationSettingAccess.ReadLocationSettingHierarchy(location);
			Guid? locationPreferredLocationGroupID = null;
			bool globalAllowed = false;

			// Determine which locations are usable from the location
			foreach (var locationSetting in locationSettingHierarchy)
			{
				if (locationSetting.GlobalAllowed is not null && (bool)locationSetting.GlobalAllowed)
				{
					globalAllowed = true;
					break;
				}
				if (locationSetting.LocationGroup is not null)
				{
					var permission = await locationGroupPermissionAccess.ReadLocationGroupPermission((Guid)locationSetting.LocationGroup, location);
					if (permission is not null && permission.Permissions.HasFlag(LocationGroupPermissionType.UseGroup))
					{
						locationPreferredLocationGroupID = permission.ID;
						break;
					}
				}
			}

			// If global isn't allowed, and the location has a preferred location group, get the usable locations in that location group, add them to usableLocations
			if (!globalAllowed && locationPreferredLocationGroupID is not null)
			{
				usableLocations = usableLocations.Concat(
					(await locationGroupPermissionAccess.ReadLocationGroupPermissionRangeByID((Guid)locationPreferredLocationGroupID))
						.Where(l => l.Permissions.HasFlag(LocationGroupPermissionType.SentencesInGroup))
						.Select(l => l.Location)
				).Distinct();
			}

			// If the author retort config is not null, determine what locationGroup and filter the author wishes to use, filter the usableLocations further by this.
			var authorRetortConfig = await authorRetortConfigAccess.ReadAuthorRetortConfig(author, location);
			if (authorRetortConfig is not null)
			{
				if (authorRetortConfig.LocationGroup is not null)
				{
					var authorPermission = await locationGroupPermissionAccess.ReadLocationGroupPermissionsForOwner((Guid)authorRetortConfig.LocationGroup, author);
					var locationPermission = await locationGroupPermissionAccess.ReadLocationGroupPermission((Guid)authorRetortConfig.LocationGroup, location);
					if (authorPermission.HasFlag(LocationGroupPermissionType.UseGroup) || locationPermission is not null && locationPermission.Permissions.HasFlag(LocationGroupPermissionType.UseGroup))
					{
						var authorUsableLocations = (await locationGroupPermissionAccess.ReadLocationGroupPermissionRangeByID((Guid)authorRetortConfig.LocationGroup))
							.Where(l => l.Permissions.HasFlag(LocationGroupPermissionType.SentencesInGroup))
							.Select(l => l.Location)
							.Distinct();
						if (globalAllowed)
						{
							usableLocations = authorUsableLocations;
						}
						else
						{
							usableLocations = authorUsableLocations.Where(a => usableLocations.Any(u => u.IsParentOrEqualTo(a))).ToList();
						}
					}
				}
			}
			return authorRetortConfig?.Filter.OIDs.Where(f => usableLocations.Any(u => u.IsParentOrEqualTo(f))).ToList() ?? usableLocations;
		}

		public async Task<LocationGroup?> GetValidGroupFromNameAndPermission(AuthorOID author, string groupName, LocationGroupPermissionType permission)
		{
			var locationGroupPermissions = (await locationGroupPermissionAccess.ReadLocationGroupRequestRangeByOwner(author)).Where(l => l.Permissions.HasFlag(permission));
			if (!locationGroupPermissions.Any())
			{
				return null;
			}
			var locationGroups = await locationGroupAccess.ReadLocationGroups(locationGroupPermissions.Select(l => l.ID));
			if (!locationGroups.Any())
			{
				_logNamelessLocationGroupWarning(logger, locationGroupPermissions.Select(l => l.ID).Distinct(), null);
				return null;
			}
			return locationGroups.Where(l => l.Name == groupName).FirstOrDefault();
		}

		private static readonly Action<ILogger, IEnumerable<Guid>, Exception?> _logNamelessLocationGroupWarning =
			LoggerMessage.Define<IEnumerable<Guid>>(
				LogLevel.Warning,
				new EventId(5, nameof(GetValidGroupFromNameAndPermission)),
				"""The LocationGroups with IDs "{IDs}" have no entry in the LocationGroup table and are thus nameless, this is unexpected.""");

	}
}
