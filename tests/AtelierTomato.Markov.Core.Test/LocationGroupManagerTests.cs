using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Model.ObjectOID;
using AtelierTomato.Markov.Storage;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace AtelierTomato.Markov.Core.Test
{
	public class LocationGroupManagerTests
	{
		private readonly Guid guid = Guid.Parse("c0102252-65cc-4892-b9b6-70114aeabaa3");
		private readonly AuthorOID sender = AuthorOID.Parse("Discord:discord.com:142781100152848384");
		private readonly AuthorOID author = AuthorOID.Parse("Discord:discord.com:644249977840730118");
		private readonly DiscordObjectOID senderLocation = DiscordObjectOID.Parse("Discord:discord.com:1196939360080253061");
		private readonly DiscordObjectOID location = DiscordObjectOID.Parse("Discord:discord.com:1290098660385886248");

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData(" ")]
		public async Task CreateGroupNameEmptyFailTest(string? name)
		{
			var locationAccess = Mock.Of<ILocationAccess>();
			var locationGroupAccess = Mock.Of<ILocationGroupAccess>();
			var locationGroupPermissionAccess = Mock.Of<ILocationGroupPermissionAccess>();
			var locationGroupRequestAccess = Mock.Of<ILocationGroupRequestAccess>();
			var locationSettingAccess = Mock.Of<ILocationSettingAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<LocationGroupManager>>();
			var locationGroupManager = new LocationGroupManager(locationAccess, locationGroupAccess, locationGroupPermissionAccess, locationGroupRequestAccess, locationSettingAccess, authorRetortConfigAccess, logger);
#pragma warning disable CS8604 // Possible null reference argument.
			Func<Task> act = async () => await locationGroupManager.CreateGroup(sender, senderLocation, name);
#pragma warning restore CS8604 // Possible null reference argument.
			await act.Should().ThrowAsync<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'name')");
		}

		[Fact]
		public async Task CreateGroupNoOwnerFoundFailTest()
		{
			var locationAccess = Mock.Of<ILocationAccess>();
			Mock.Get(locationAccess)
				.Setup(l => l.ReadLocationOwner(senderLocation))
				.Returns(Task.FromResult<AuthorOID?>(null))
				.Verifiable();
			var locationGroupAccess = Mock.Of<ILocationGroupAccess>();
			var locationGroupPermissionAccess = Mock.Of<ILocationGroupPermissionAccess>();
			var locationGroupRequestAccess = Mock.Of<ILocationGroupRequestAccess>();
			var locationSettingAccess = Mock.Of<ILocationSettingAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<LocationGroupManager>>();
			var locationGroupManager = new LocationGroupManager(locationAccess, locationGroupAccess, locationGroupPermissionAccess, locationGroupRequestAccess, locationSettingAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await locationGroupManager.CreateGroup(sender, senderLocation, "MyNewGroup");
			await act.Should().ThrowAsync<InvalidOperationException>().WithMessage($"Cannot process {nameof(LocationGroup)} creation as the database returned no {nameof(Location.Owner)} for the {nameof(Location)}. Either run command Refresh{nameof(Location)} or contact the owner of the bot.");
			Mock.Get(locationAccess).Verify();
		}

		[Fact]
		public async Task CreateGroupDifferentOwnerFoundFailTest()
		{
			var locationAccess = Mock.Of<ILocationAccess>();
			Mock.Get(locationAccess)
				.Setup(l => l.ReadLocationOwner(senderLocation))
				.Returns(Task.FromResult<AuthorOID?>(author))
				.Verifiable();
			var locationGroupAccess = Mock.Of<ILocationGroupAccess>();
			var locationGroupPermissionAccess = Mock.Of<ILocationGroupPermissionAccess>();
			var locationGroupRequestAccess = Mock.Of<ILocationGroupRequestAccess>();
			var locationSettingAccess = Mock.Of<ILocationSettingAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<LocationGroupManager>>();
			var locationGroupManager = new LocationGroupManager(locationAccess, locationGroupAccess, locationGroupPermissionAccess, locationGroupRequestAccess, locationSettingAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await locationGroupManager.CreateGroup(sender, senderLocation, "MyNewGroup");
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""Author "{sender}" is not the same {nameof(Author)} as the {nameof(Location.Owner)} "{author}" of {nameof(Location)} "{senderLocation}". (Parameter '{nameof(senderLocation)}')""");
			Mock.Get(locationAccess).Verify();
		}

		[Fact]
		public async Task CreateGroupTest()
		{
			var locationAccess = Mock.Of<ILocationAccess>();
			Mock.Get(locationAccess)
				.Setup(l => l.ReadLocationOwner(senderLocation))
				.Returns(Task.FromResult<AuthorOID?>(sender))
				.Verifiable();
			var locationGroupAccess = Mock.Of<ILocationGroupAccess>();
			var locationGroupPermissionAccess = Mock.Of<ILocationGroupPermissionAccess>();
			var locationGroupRequestAccess = Mock.Of<ILocationGroupRequestAccess>();
			var locationSettingAccess = Mock.Of<ILocationSettingAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<LocationGroupManager>>();
			var locationGroupManager = new LocationGroupManager(locationAccess, locationGroupAccess, locationGroupPermissionAccess, locationGroupRequestAccess, locationSettingAccess, authorRetortConfigAccess, logger);
			await locationGroupManager.CreateGroup(sender, senderLocation, "MyNewGroup");
			Mock.Get(locationAccess).Verify();
			Mock.Get(locationGroupAccess).Verify(g => g.WriteLocationGroup(It.IsAny<LocationGroup>()), Times.Once());
			Mock.Get(locationGroupPermissionAccess).Verify(g => g.WriteLocationGroupPermission(It.IsAny<LocationGroupPermission>()), Times.Once());
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData(" ")]
		public async Task RenameGroupNameEmptyFailTest(string? name)
		{
			var locationAccess = Mock.Of<ILocationAccess>();
			var locationGroupAccess = Mock.Of<ILocationGroupAccess>();
			var locationGroupPermissionAccess = Mock.Of<ILocationGroupPermissionAccess>();
			var locationGroupRequestAccess = Mock.Of<ILocationGroupRequestAccess>();
			var locationSettingAccess = Mock.Of<ILocationSettingAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<LocationGroupManager>>();
			var locationGroupManager = new LocationGroupManager(locationAccess, locationGroupAccess, locationGroupPermissionAccess, locationGroupRequestAccess, locationSettingAccess, authorRetortConfigAccess, logger);
#pragma warning disable CS8604 // Possible null reference argument.
			Func<Task> act = async () => await locationGroupManager.RenameGroup(sender, guid, name);
#pragma warning restore CS8604 // Possible null reference argument.
			await act.Should().ThrowAsync<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'name')");
		}

		[Fact]
		public async Task RenameGroupNoPermissionFailTest()
		{
			var locationAccess = Mock.Of<ILocationAccess>();
			var locationGroupAccess = Mock.Of<ILocationGroupAccess>();
			var locationGroupPermissionAccess = Mock.Of<ILocationGroupPermissionAccess>();
			Mock.Get(locationGroupPermissionAccess)
				.Setup(l => l.ReadLocationGroupPermissionsForOwner(guid, sender))
				.Returns(Task.FromResult(LocationGroupPermissionType.UseGroup))
				.Verifiable();
			var locationGroupRequestAccess = Mock.Of<ILocationGroupRequestAccess>();
			var locationSettingAccess = Mock.Of<ILocationSettingAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<LocationGroupManager>>();
			var locationGroupManager = new LocationGroupManager(locationAccess, locationGroupAccess, locationGroupPermissionAccess, locationGroupRequestAccess, locationSettingAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await locationGroupManager.RenameGroup(sender, guid, "MyNewerGroup");
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""Author "{sender}" does not have permission to rename group with ID "{guid}". (Parameter '{nameof(sender)}')""");
			Mock.Get(locationGroupPermissionAccess).Verify();
		}

		[Fact]
		public async Task RenameGroupTest()
		{
			var locationAccess = Mock.Of<ILocationAccess>();
			var locationGroupAccess = Mock.Of<ILocationGroupAccess>();
			var locationGroupPermissionAccess = Mock.Of<ILocationGroupPermissionAccess>();
			Mock.Get(locationGroupPermissionAccess)
				.Setup(l => l.ReadLocationGroupPermissionsForOwner(guid, sender))
				.Returns(Task.FromResult(LocationGroupPermissionType.UseGroup | LocationGroupPermissionType.RenameGroup))
				.Verifiable();
			var locationGroupRequestAccess = Mock.Of<ILocationGroupRequestAccess>();
			var locationSettingAccess = Mock.Of<ILocationSettingAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<LocationGroupManager>>();
			var locationGroupManager = new LocationGroupManager(locationAccess, locationGroupAccess, locationGroupPermissionAccess, locationGroupRequestAccess, locationSettingAccess, authorRetortConfigAccess, logger);
			await locationGroupManager.RenameGroup(sender, guid, "MyNewerGroup");
			Mock.Get(locationGroupPermissionAccess).Verify();
			Mock.Get(locationGroupAccess).Verify(g => g.WriteLocationGroup(new LocationGroup(guid, "MyNewerGroup")), Times.Once());
		}

		[Fact]
		public async Task DeleteGroupNoPermissionFailTest()
		{
			var locationAccess = Mock.Of<ILocationAccess>();
			var locationGroupAccess = Mock.Of<ILocationGroupAccess>();
			var locationGroupPermissionAccess = Mock.Of<ILocationGroupPermissionAccess>();
			Mock.Get(locationGroupPermissionAccess)
				.Setup(l => l.ReadLocationGroupPermissionsForOwner(guid, sender))
				.Returns(Task.FromResult(LocationGroupPermissionType.UseGroup))
				.Verifiable();
			var locationGroupRequestAccess = Mock.Of<ILocationGroupRequestAccess>();
			var locationSettingAccess = Mock.Of<ILocationSettingAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<LocationGroupManager>>();
			var locationGroupManager = new LocationGroupManager(locationAccess, locationGroupAccess, locationGroupPermissionAccess, locationGroupRequestAccess, locationSettingAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await locationGroupManager.DeleteGroup(sender, guid);
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""Author "{sender}" does not have permission to delete group with ID "{guid}". (Parameter '{nameof(sender)}')""");
			Mock.Get(locationGroupPermissionAccess).Verify();
		}

		[Fact]
		public async Task DeleteGroupTest()
		{
			var locationAccess = Mock.Of<ILocationAccess>();
			var locationGroupAccess = Mock.Of<ILocationGroupAccess>();
			var locationGroupPermissionAccess = Mock.Of<ILocationGroupPermissionAccess>();
			Mock.Get(locationGroupPermissionAccess)
				.Setup(l => l.ReadLocationGroupPermissionsForOwner(guid, sender))
				.Returns(Task.FromResult(LocationGroupPermissionType.UseGroup | LocationGroupPermissionType.DeleteGroup))
				.Verifiable();
			var locationGroupRequestAccess = Mock.Of<ILocationGroupRequestAccess>();
			var locationSettingAccess = Mock.Of<ILocationSettingAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<LocationGroupManager>>();
			var locationGroupManager = new LocationGroupManager(locationAccess, locationGroupAccess, locationGroupPermissionAccess, locationGroupRequestAccess, locationSettingAccess, authorRetortConfigAccess, logger);
			await locationGroupManager.DeleteGroup(sender, guid);
			Mock.Get(locationGroupPermissionAccess).Verify();
			Mock.Get(locationGroupAccess).Verify(g => g.DeleteLocationGroup(guid), Times.Once());
		}

		[Fact]
		public async Task SendOrUpdateLocationGroupRequestNoPermissionFailTest()
		{
			var locationAccess = Mock.Of<ILocationAccess>();
			var locationGroupAccess = Mock.Of<ILocationGroupAccess>();
			var locationGroupPermissionAccess = Mock.Of<ILocationGroupPermissionAccess>();
			Mock.Get(locationGroupPermissionAccess)
				.Setup(l => l.ReadLocationGroupPermissionsForOwner(guid, sender))
				.Returns(Task.FromResult(LocationGroupPermissionType.UseGroup))
				.Verifiable();
			var locationGroupRequestAccess = Mock.Of<ILocationGroupRequestAccess>();
			var locationSettingAccess = Mock.Of<ILocationSettingAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<LocationGroupManager>>();
			var locationGroupManager = new LocationGroupManager(locationAccess, locationGroupAccess, locationGroupPermissionAccess, locationGroupRequestAccess, locationSettingAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await locationGroupManager.SendOrUpdateLocationGroupRequest(sender, new LocationGroupPermission(guid, senderLocation, LocationGroupPermissionType.UseGroup));
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""Author "{sender}" does not have permission to add locations to group with ID "{guid}". (Parameter '{nameof(sender)}')""");
			Mock.Get(locationGroupPermissionAccess).Verify();
		}

		[Fact]
		public async Task SendOrUpdateLocationGroupRequestAlreadyMemberFailTest()
		{
			var locationGroupPermission = new LocationGroupPermission(guid, senderLocation, LocationGroupPermissionType.UseGroup);
			var locationAccess = Mock.Of<ILocationAccess>();
			var locationGroupAccess = Mock.Of<ILocationGroupAccess>();
			var locationGroupPermissionAccess = Mock.Of<ILocationGroupPermissionAccess>();
			Mock.Get(locationGroupPermissionAccess)
				.Setup(l => l.ReadLocationGroupPermissionsForOwner(guid, sender))
				.Returns(Task.FromResult(LocationGroupPermissionType.UseGroup | LocationGroupPermissionType.AddLocation))
				.Verifiable();
			Mock.Get(locationGroupPermissionAccess)
				.Setup(l => l.ReadLocationGroupPermission(locationGroupPermission.ID, locationGroupPermission.Location))
				.Returns(Task.FromResult<LocationGroupPermission?>(locationGroupPermission))
				.Verifiable();
			var locationGroupRequestAccess = Mock.Of<ILocationGroupRequestAccess>();
			var locationSettingAccess = Mock.Of<ILocationSettingAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<LocationGroupManager>>();
			var locationGroupManager = new LocationGroupManager(locationAccess, locationGroupAccess, locationGroupPermissionAccess, locationGroupRequestAccess, locationSettingAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await locationGroupManager.SendOrUpdateLocationGroupRequest(sender, locationGroupPermission);
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""Location "{locationGroupPermission.Location}" is already registered to group with ID "{locationGroupPermission.ID}". (Parameter '{nameof(locationGroupPermission)}')""");
			Mock.Get(locationGroupPermissionAccess).Verify();
		}

		[Fact]
		public async Task SendOrUpdateLocationGroupRequestLackingPermissionsFailTest()
		{
			var locationGroupPermission = new LocationGroupPermission(guid, senderLocation, LocationGroupPermissionType.UseGroup | LocationGroupPermissionType.SentencesInGroup);
			var locationAccess = Mock.Of<ILocationAccess>();
			var locationGroupAccess = Mock.Of<ILocationGroupAccess>();
			var locationGroupPermissionAccess = Mock.Of<ILocationGroupPermissionAccess>();
			Mock.Get(locationGroupPermissionAccess)
				.Setup(l => l.ReadLocationGroupPermissionsForOwner(guid, sender))
				.Returns(Task.FromResult(LocationGroupPermissionType.UseGroup | LocationGroupPermissionType.AddLocation))
				.Verifiable();
			Mock.Get(locationGroupPermissionAccess)
				.Setup(l => l.ReadLocationGroupPermission(locationGroupPermission.ID, locationGroupPermission.Location))
				.Returns(Task.FromResult<LocationGroupPermission?>(null))
				.Verifiable();
			var locationGroupRequestAccess = Mock.Of<ILocationGroupRequestAccess>();
			var locationSettingAccess = Mock.Of<ILocationSettingAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<LocationGroupManager>>();
			var locationGroupManager = new LocationGroupManager(locationAccess, locationGroupAccess, locationGroupPermissionAccess, locationGroupRequestAccess, locationSettingAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await locationGroupManager.SendOrUpdateLocationGroupRequest(sender, locationGroupPermission);
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""Author "{sender}" does not have some of the permissions they are trying to assign. (Parameter '{nameof(locationGroupPermission)}')""");
			Mock.Get(locationGroupPermissionAccess).Verify();
		}

		[Fact]
		public async Task SendOrUpdateLocationGroupRequestTest()
		{
			var locationGroupPermission = new LocationGroupPermission(guid, senderLocation, LocationGroupPermissionType.UseGroup);
			var locationAccess = Mock.Of<ILocationAccess>();
			var locationGroupAccess = Mock.Of<ILocationGroupAccess>();
			var locationGroupPermissionAccess = Mock.Of<ILocationGroupPermissionAccess>();
			Mock.Get(locationGroupPermissionAccess)
				.Setup(l => l.ReadLocationGroupPermissionsForOwner(guid, sender))
				.Returns(Task.FromResult(LocationGroupPermissionType.UseGroup | LocationGroupPermissionType.AddLocation))
				.Verifiable();
			Mock.Get(locationGroupPermissionAccess)
				.Setup(l => l.ReadLocationGroupPermission(locationGroupPermission.ID, locationGroupPermission.Location))
				.Returns(Task.FromResult<LocationGroupPermission?>(null))
				.Verifiable();
			var locationGroupRequestAccess = Mock.Of<ILocationGroupRequestAccess>();
			var locationSettingAccess = Mock.Of<ILocationSettingAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<LocationGroupManager>>();
			var locationGroupManager = new LocationGroupManager(locationAccess, locationGroupAccess, locationGroupPermissionAccess, locationGroupRequestAccess, locationSettingAccess, authorRetortConfigAccess, logger);
			await locationGroupManager.SendOrUpdateLocationGroupRequest(sender, locationGroupPermission);
			Mock.Get(locationGroupPermissionAccess).Verify();
			Mock.Get(locationGroupRequestAccess).Verify(g => g.WriteLocationGroupRequest(locationGroupPermission), Times.Once());
		}

		[Fact]
		public async Task AcceptInvitationNoOwnerFoundFailTest()
		{
			var locationAccess = Mock.Of<ILocationAccess>();
			Mock.Get(locationAccess)
				.Setup(l => l.ReadLocationOwner(senderLocation))
				.Returns(Task.FromResult<AuthorOID?>(null))
				.Verifiable();
			var locationGroupAccess = Mock.Of<ILocationGroupAccess>();
			var locationGroupPermissionAccess = Mock.Of<ILocationGroupPermissionAccess>();
			var locationGroupRequestAccess = Mock.Of<ILocationGroupRequestAccess>();
			var locationSettingAccess = Mock.Of<ILocationSettingAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<LocationGroupManager>>();
			var locationGroupManager = new LocationGroupManager(locationAccess, locationGroupAccess, locationGroupPermissionAccess, locationGroupRequestAccess, locationSettingAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await locationGroupManager.AcceptInvitation(sender, senderLocation, guid);
			await act.Should().ThrowAsync<InvalidOperationException>().WithMessage($"Cannot process {nameof(LocationGroup)}Request accepting as the database returned no {nameof(Location.Owner)} for the {nameof(Location)}. Either run command Refresh{nameof(Location)} or contact the owner of the bot.");
			Mock.Get(locationAccess).Verify();
		}

		[Fact]
		public async Task AcceptInvitationDifferentOwnerFoundFailTest()
		{
			var locationAccess = Mock.Of<ILocationAccess>();
			Mock.Get(locationAccess)
				.Setup(l => l.ReadLocationOwner(senderLocation))
				.Returns(Task.FromResult<AuthorOID?>(author))
				.Verifiable();
			var locationGroupAccess = Mock.Of<ILocationGroupAccess>();
			var locationGroupPermissionAccess = Mock.Of<ILocationGroupPermissionAccess>();
			var locationGroupRequestAccess = Mock.Of<ILocationGroupRequestAccess>();
			var locationSettingAccess = Mock.Of<ILocationSettingAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<LocationGroupManager>>();
			var locationGroupManager = new LocationGroupManager(locationAccess, locationGroupAccess, locationGroupPermissionAccess, locationGroupRequestAccess, locationSettingAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await locationGroupManager.AcceptInvitation(sender, senderLocation, guid);
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""Author "{sender}" is not the same {nameof(Author)} as the {nameof(Location.Owner)} "{author}" of {nameof(Location)} "{senderLocation}". (Parameter 'locationID')""");
			Mock.Get(locationAccess).Verify();
		}

		[Fact]
		public async Task AcceptInvitationNoInvitationFoundFailTest()
		{
			var locationAccess = Mock.Of<ILocationAccess>();
			Mock.Get(locationAccess)
				.Setup(l => l.ReadLocationOwner(senderLocation))
				.Returns(Task.FromResult<AuthorOID?>(sender))
				.Verifiable();
			var locationGroupAccess = Mock.Of<ILocationGroupAccess>();
			var locationGroupPermissionAccess = Mock.Of<ILocationGroupPermissionAccess>();
			var locationGroupRequestAccess = Mock.Of<ILocationGroupRequestAccess>();
			Mock.Get(locationGroupRequestAccess)
				.Setup(l => l.ReadLocationGroupRequest(guid, senderLocation))
				.Returns(Task.FromResult<LocationGroupPermission?>(null))
				.Verifiable();
			var locationSettingAccess = Mock.Of<ILocationSettingAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<LocationGroupManager>>();
			var locationGroupManager = new LocationGroupManager(locationAccess, locationGroupAccess, locationGroupPermissionAccess, locationGroupRequestAccess, locationSettingAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await locationGroupManager.AcceptInvitation(sender, senderLocation, guid);
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""Location "{senderLocation}" has not been sent an invitation to group with ID "{guid}". (Parameter 'ID')""");
			Mock.Get(locationAccess).Verify();
			Mock.Get(locationGroupRequestAccess).Verify();
		}

		[Fact]
		public async Task AcceptInvitationTest()
		{
			var locationGroupPermission = new LocationGroupPermission(guid, senderLocation, LocationGroupPermissionType.UseGroup);
			var locationAccess = Mock.Of<ILocationAccess>();
			Mock.Get(locationAccess)
				.Setup(l => l.ReadLocationOwner(senderLocation))
				.Returns(Task.FromResult<AuthorOID?>(sender))
				.Verifiable();
			var locationGroupAccess = Mock.Of<ILocationGroupAccess>();
			var locationGroupPermissionAccess = Mock.Of<ILocationGroupPermissionAccess>();
			var locationGroupRequestAccess = Mock.Of<ILocationGroupRequestAccess>();
			Mock.Get(locationGroupRequestAccess)
				.Setup(l => l.ReadLocationGroupRequest(guid, senderLocation))
				.Returns(Task.FromResult<LocationGroupPermission?>(locationGroupPermission))
				.Verifiable();
			var locationSettingAccess = Mock.Of<ILocationSettingAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<LocationGroupManager>>();
			var locationGroupManager = new LocationGroupManager(locationAccess, locationGroupAccess, locationGroupPermissionAccess, locationGroupRequestAccess, locationSettingAccess, authorRetortConfigAccess, logger);
			await locationGroupManager.AcceptInvitation(sender, senderLocation, guid);
			Mock.Get(locationAccess).Verify();
			Mock.Get(locationGroupRequestAccess).Verify();
			Mock.Get(locationGroupPermissionAccess).Verify(g => g.WriteLocationGroupPermission(locationGroupPermission), Times.Once());
			Mock.Get(locationGroupRequestAccess).Verify(g => g.DeleteLocationGroupRequest(guid, senderLocation), Times.Once());
		}

		[Fact]
		public async Task DenyInvitationNoOwnerFoundFailTest()
		{
			var locationAccess = Mock.Of<ILocationAccess>();
			Mock.Get(locationAccess)
				.Setup(l => l.ReadLocationOwner(senderLocation))
				.Returns(Task.FromResult<AuthorOID?>(null))
				.Verifiable();
			var locationGroupAccess = Mock.Of<ILocationGroupAccess>();
			var locationGroupPermissionAccess = Mock.Of<ILocationGroupPermissionAccess>();
			var locationGroupRequestAccess = Mock.Of<ILocationGroupRequestAccess>();
			var locationSettingAccess = Mock.Of<ILocationSettingAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<LocationGroupManager>>();
			var locationGroupManager = new LocationGroupManager(locationAccess, locationGroupAccess, locationGroupPermissionAccess, locationGroupRequestAccess, locationSettingAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await locationGroupManager.DenyInvitation(sender, senderLocation, guid);
			await act.Should().ThrowAsync<InvalidOperationException>().WithMessage($"Cannot process {nameof(LocationGroup)}Request denying as the database returned no {nameof(Location.Owner)} for the {nameof(Location)}. Either run command Refresh{nameof(Location)} or contact the owner of the bot.");
			Mock.Get(locationAccess).Verify();
		}

		[Fact]
		public async Task DenyInvitationDifferentOwnerFoundFailTest()
		{
			var locationAccess = Mock.Of<ILocationAccess>();
			Mock.Get(locationAccess)
				.Setup(l => l.ReadLocationOwner(senderLocation))
				.Returns(Task.FromResult<AuthorOID?>(author))
				.Verifiable();
			var locationGroupAccess = Mock.Of<ILocationGroupAccess>();
			var locationGroupPermissionAccess = Mock.Of<ILocationGroupPermissionAccess>();
			var locationGroupRequestAccess = Mock.Of<ILocationGroupRequestAccess>();
			var locationSettingAccess = Mock.Of<ILocationSettingAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<LocationGroupManager>>();
			var locationGroupManager = new LocationGroupManager(locationAccess, locationGroupAccess, locationGroupPermissionAccess, locationGroupRequestAccess, locationSettingAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await locationGroupManager.DenyInvitation(sender, senderLocation, guid);
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""Author "{sender}" is not the same {nameof(Author)} as the {nameof(Location.Owner)} "{author}" of {nameof(Location)} "{senderLocation}". (Parameter 'locationID')""");
			Mock.Get(locationAccess).Verify();
		}

		[Fact]
		public async Task DenyInvitationNoInvitationFoundFailTest()
		{
			var locationAccess = Mock.Of<ILocationAccess>();
			Mock.Get(locationAccess)
				.Setup(l => l.ReadLocationOwner(senderLocation))
				.Returns(Task.FromResult<AuthorOID?>(sender))
				.Verifiable();
			var locationGroupAccess = Mock.Of<ILocationGroupAccess>();
			var locationGroupPermissionAccess = Mock.Of<ILocationGroupPermissionAccess>();
			var locationGroupRequestAccess = Mock.Of<ILocationGroupRequestAccess>();
			Mock.Get(locationGroupRequestAccess)
				.Setup(l => l.ReadLocationGroupRequest(guid, senderLocation))
				.Returns(Task.FromResult<LocationGroupPermission?>(null))
				.Verifiable();
			var locationSettingAccess = Mock.Of<ILocationSettingAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<LocationGroupManager>>();
			var locationGroupManager = new LocationGroupManager(locationAccess, locationGroupAccess, locationGroupPermissionAccess, locationGroupRequestAccess, locationSettingAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await locationGroupManager.DenyInvitation(sender, senderLocation, guid);
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""Location "{senderLocation}" has not been sent an invitation to group with ID "{guid}". (Parameter 'ID')""");
			Mock.Get(locationAccess).Verify();
			Mock.Get(locationGroupRequestAccess).Verify();
		}

		[Fact]
		public async Task DenyInvitationTest()
		{
			var locationGroupPermission = new LocationGroupPermission(guid, senderLocation, LocationGroupPermissionType.UseGroup);
			var locationAccess = Mock.Of<ILocationAccess>();
			Mock.Get(locationAccess)
				.Setup(l => l.ReadLocationOwner(senderLocation))
				.Returns(Task.FromResult<AuthorOID?>(sender))
				.Verifiable();
			var locationGroupAccess = Mock.Of<ILocationGroupAccess>();
			var locationGroupPermissionAccess = Mock.Of<ILocationGroupPermissionAccess>();
			var locationGroupRequestAccess = Mock.Of<ILocationGroupRequestAccess>();
			Mock.Get(locationGroupRequestAccess)
				.Setup(l => l.ReadLocationGroupRequest(guid, senderLocation))
				.Returns(Task.FromResult<LocationGroupPermission?>(locationGroupPermission))
				.Verifiable();
			var locationSettingAccess = Mock.Of<ILocationSettingAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<LocationGroupManager>>();
			var locationGroupManager = new LocationGroupManager(locationAccess, locationGroupAccess, locationGroupPermissionAccess, locationGroupRequestAccess, locationSettingAccess, authorRetortConfigAccess, logger);
			await locationGroupManager.DenyInvitation(sender, senderLocation, guid);
			Mock.Get(locationAccess).Verify();
			Mock.Get(locationGroupRequestAccess).Verify();
			Mock.Get(locationGroupRequestAccess).Verify(g => g.DeleteLocationGroupRequest(guid, senderLocation), Times.Once());
		}

		[Fact]
		public async Task UpdateLocationNoPermissionFailTest()
		{
			var locationAccess = Mock.Of<ILocationAccess>();
			var locationGroupAccess = Mock.Of<ILocationGroupAccess>();
			var locationGroupPermissionAccess = Mock.Of<ILocationGroupPermissionAccess>();
			Mock.Get(locationGroupPermissionAccess)
				.Setup(l => l.ReadLocationGroupPermissionsForOwner(guid, sender))
				.Returns(Task.FromResult(LocationGroupPermissionType.UseGroup))
				.Verifiable();
			var locationGroupRequestAccess = Mock.Of<ILocationGroupRequestAccess>();
			var locationSettingAccess = Mock.Of<ILocationSettingAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<LocationGroupManager>>();
			var locationGroupManager = new LocationGroupManager(locationAccess, locationGroupAccess, locationGroupPermissionAccess, locationGroupRequestAccess, locationSettingAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await locationGroupManager.UpdateLocation(sender, new LocationGroupPermission(guid, senderLocation, LocationGroupPermissionType.UseGroup));
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""Author "{sender}" does not have permission to add locations to group with ID "{guid}". (Parameter '{nameof(sender)}')""");
			Mock.Get(locationGroupPermissionAccess).Verify();
		}

		[Fact]
		public async Task UpdateLocationLackingPermissionsFailTest()
		{
			var locationGroupPermission = new LocationGroupPermission(guid, senderLocation, LocationGroupPermissionType.UseGroup | LocationGroupPermissionType.SentencesInGroup);
			var locationAccess = Mock.Of<ILocationAccess>();
			var locationGroupAccess = Mock.Of<ILocationGroupAccess>();
			var locationGroupPermissionAccess = Mock.Of<ILocationGroupPermissionAccess>();
			Mock.Get(locationGroupPermissionAccess)
				.Setup(l => l.ReadLocationGroupPermissionsForOwner(guid, sender))
				.Returns(Task.FromResult(LocationGroupPermissionType.UseGroup | LocationGroupPermissionType.AddLocation))
				.Verifiable();
			var locationGroupRequestAccess = Mock.Of<ILocationGroupRequestAccess>();
			var locationSettingAccess = Mock.Of<ILocationSettingAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<LocationGroupManager>>();
			var locationGroupManager = new LocationGroupManager(locationAccess, locationGroupAccess, locationGroupPermissionAccess, locationGroupRequestAccess, locationSettingAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await locationGroupManager.UpdateLocation(sender, locationGroupPermission);
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""Author "{sender}" does not have some of the permissions they are trying to assign. (Parameter '{nameof(locationGroupPermission)}')""");
			Mock.Get(locationGroupPermissionAccess).Verify();
		}

		[Fact]
		public async Task UpdateLocationTest()
		{
			var locationGroupPermission = new LocationGroupPermission(guid, senderLocation, LocationGroupPermissionType.UseGroup);
			var locationAccess = Mock.Of<ILocationAccess>();
			var locationGroupAccess = Mock.Of<ILocationGroupAccess>();
			var locationGroupPermissionAccess = Mock.Of<ILocationGroupPermissionAccess>();
			Mock.Get(locationGroupPermissionAccess)
				.Setup(l => l.ReadLocationGroupPermissionsForOwner(guid, sender))
				.Returns(Task.FromResult(LocationGroupPermissionType.UseGroup | LocationGroupPermissionType.AddLocation))
				.Verifiable();
			var locationGroupRequestAccess = Mock.Of<ILocationGroupRequestAccess>();
			var locationSettingAccess = Mock.Of<ILocationSettingAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<LocationGroupManager>>();
			var locationGroupManager = new LocationGroupManager(locationAccess, locationGroupAccess, locationGroupPermissionAccess, locationGroupRequestAccess, locationSettingAccess, authorRetortConfigAccess, logger);
			await locationGroupManager.UpdateLocation(sender, locationGroupPermission);
			Mock.Get(locationGroupPermissionAccess).Verify();
			Mock.Get(locationGroupPermissionAccess).Verify(g => g.WriteLocationGroupPermission(locationGroupPermission), Times.Once());
		}

		[Fact]
		public async Task RemoveLocationNoPermissionFailTest()
		{
			var locationAccess = Mock.Of<ILocationAccess>();
			var locationGroupAccess = Mock.Of<ILocationGroupAccess>();
			var locationGroupPermissionAccess = Mock.Of<ILocationGroupPermissionAccess>();
			Mock.Get(locationGroupPermissionAccess)
				.Setup(l => l.ReadLocationGroupPermissionsForOwner(guid, sender))
				.Returns(Task.FromResult(LocationGroupPermissionType.UseGroup))
				.Verifiable();
			var locationGroupRequestAccess = Mock.Of<ILocationGroupRequestAccess>();
			var locationSettingAccess = Mock.Of<ILocationSettingAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<LocationGroupManager>>();
			var locationGroupManager = new LocationGroupManager(locationAccess, locationGroupAccess, locationGroupPermissionAccess, locationGroupRequestAccess, locationSettingAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await locationGroupManager.RemoveLocation(sender, guid, senderLocation);
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""Author "{sender}" does not have permission to remove locations from group with ID "{guid}". (Parameter '{nameof(sender)}')""");
			Mock.Get(locationGroupPermissionAccess).Verify();
		}

		[Fact]
		public async Task RemoveLocationNotRegisteredFailTest()
		{
			var locationAccess = Mock.Of<ILocationAccess>();
			var locationGroupAccess = Mock.Of<ILocationGroupAccess>();
			var locationGroupPermissionAccess = Mock.Of<ILocationGroupPermissionAccess>();
			Mock.Get(locationGroupPermissionAccess)
				.Setup(l => l.ReadLocationGroupPermissionsForOwner(guid, sender))
				.Returns(Task.FromResult(LocationGroupPermissionType.UseGroup | LocationGroupPermissionType.RemoveLocation))
				.Verifiable();
			Mock.Get(locationGroupPermissionAccess)
				.Setup(l => l.ReadLocationGroupPermissionRangeByID(guid))
				.Returns(Task.FromResult<IEnumerable<LocationGroupPermission>>([]))
				.Verifiable();
			var locationGroupRequestAccess = Mock.Of<ILocationGroupRequestAccess>();
			var locationSettingAccess = Mock.Of<ILocationSettingAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<LocationGroupManager>>();
			var locationGroupManager = new LocationGroupManager(locationAccess, locationGroupAccess, locationGroupPermissionAccess, locationGroupRequestAccess, locationSettingAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await locationGroupManager.RemoveLocation(sender, guid, senderLocation);
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""Location "{senderLocation}" is not registered to group with ID "{guid}". (Parameter 'location')""");
			Mock.Get(locationGroupPermissionAccess).Verify();
		}

		[Fact]
		public async Task RemoveLocationGroupOrphanedFailTest()
		{
			var locationGroupPermission = new LocationGroupPermission(guid, senderLocation, LocationGroupPermissionType.UseGroup | LocationGroupPermissionType.RemoveLocation);
			var locationAccess = Mock.Of<ILocationAccess>();
			var locationGroupAccess = Mock.Of<ILocationGroupAccess>();
			var locationGroupPermissionAccess = Mock.Of<ILocationGroupPermissionAccess>();
			Mock.Get(locationGroupPermissionAccess)
				.Setup(l => l.ReadLocationGroupPermissionsForOwner(guid, sender))
				.Returns(Task.FromResult(locationGroupPermission.Permissions))
				.Verifiable();
			Mock.Get(locationGroupPermissionAccess)
				.Setup(l => l.ReadLocationGroupPermissionRangeByID(guid))
				.Returns(Task.FromResult<IEnumerable<LocationGroupPermission>>([locationGroupPermission]))
				.Verifiable();
			var locationGroupRequestAccess = Mock.Of<ILocationGroupRequestAccess>();
			var locationSettingAccess = Mock.Of<ILocationSettingAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<LocationGroupManager>>();
			var locationGroupManager = new LocationGroupManager(locationAccess, locationGroupAccess, locationGroupPermissionAccess, locationGroupRequestAccess, locationSettingAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await locationGroupManager.RemoveLocation(sender, guid, senderLocation);
			await act.Should().ThrowAsync<InvalidOperationException>().WithMessage($"""The {nameof(LocationGroup)} with ID "{guid}" has no members with permission {nameof(LocationGroupPermissionType.DeleteGroup)}. This is unexpected.""");
			Mock.Get(locationGroupPermissionAccess).Verify();
		}

		[Fact]
		public async Task RemoveLocationLastLocationWithDeleteFailTest()
		{
			var locationGroupPermission = new LocationGroupPermission(guid, senderLocation, LocationGroupPermissionType.UseGroup | LocationGroupPermissionType.RemoveLocation | LocationGroupPermissionType.DeleteGroup);
			var locationAccess = Mock.Of<ILocationAccess>();
			var locationGroupAccess = Mock.Of<ILocationGroupAccess>();
			var locationGroupPermissionAccess = Mock.Of<ILocationGroupPermissionAccess>();
			Mock.Get(locationGroupPermissionAccess)
				.Setup(l => l.ReadLocationGroupPermissionsForOwner(guid, sender))
				.Returns(Task.FromResult(locationGroupPermission.Permissions))
				.Verifiable();
			Mock.Get(locationGroupPermissionAccess)
				.Setup(l => l.ReadLocationGroupPermissionRangeByID(guid))
				.Returns(Task.FromResult<IEnumerable<LocationGroupPermission>>([locationGroupPermission]))
				.Verifiable();
			var locationGroupRequestAccess = Mock.Of<ILocationGroupRequestAccess>();
			var locationSettingAccess = Mock.Of<ILocationSettingAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<LocationGroupManager>>();
			var locationGroupManager = new LocationGroupManager(locationAccess, locationGroupAccess, locationGroupPermissionAccess, locationGroupRequestAccess, locationSettingAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await locationGroupManager.RemoveLocation(sender, guid, senderLocation);
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""Location "{senderLocation}" cannot be removed from group with ID "{guid}" as it is the only member of it that has the permission {nameof(LocationGroupPermissionType.DeleteGroup)}. Please use "{nameof(LocationGroupManager.DeleteGroup)}" function instead. (Parameter 'location')""");
			Mock.Get(locationGroupPermissionAccess).Verify();
		}

		[Fact]
		public async Task RemoveLocationLackingPermissionsFailTest()
		{
			var senderLocationGroupPermission = new LocationGroupPermission(guid, senderLocation, LocationGroupPermissionType.UseGroup | LocationGroupPermissionType.RemoveLocation | LocationGroupPermissionType.DeleteGroup);
			var locationGroupPermission = new LocationGroupPermission(guid, location, LocationGroupPermissionType.UseGroup | LocationGroupPermissionType.RemoveLocation | LocationGroupPermissionType.DeleteGroup | LocationGroupPermissionType.SentencesInGroup);
			var locationAccess = Mock.Of<ILocationAccess>();
			var locationGroupAccess = Mock.Of<ILocationGroupAccess>();
			var locationGroupPermissionAccess = Mock.Of<ILocationGroupPermissionAccess>();
			Mock.Get(locationGroupPermissionAccess)
				.Setup(l => l.ReadLocationGroupPermissionsForOwner(guid, sender))
				.Returns(Task.FromResult(senderLocationGroupPermission.Permissions))
				.Verifiable();
			Mock.Get(locationGroupPermissionAccess)
				.Setup(l => l.ReadLocationGroupPermissionRangeByID(guid))
				.Returns(Task.FromResult<IEnumerable<LocationGroupPermission>>([senderLocationGroupPermission, locationGroupPermission]))
				.Verifiable();
			var locationGroupRequestAccess = Mock.Of<ILocationGroupRequestAccess>();
			var locationSettingAccess = Mock.Of<ILocationSettingAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<LocationGroupManager>>();
			var locationGroupManager = new LocationGroupManager(locationAccess, locationGroupAccess, locationGroupPermissionAccess, locationGroupRequestAccess, locationSettingAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await locationGroupManager.RemoveLocation(sender, guid, location);
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""Author "{sender}" does not have some of the permissions that the location they are trying to remove has. (Parameter 'location')""");
			Mock.Get(locationGroupPermissionAccess).Verify();
		}

		[Fact]
		public async Task RemoveLocationTest()
		{
			var senderLocationGroupPermission = new LocationGroupPermission(guid, senderLocation, LocationGroupPermissionType.UseGroup | LocationGroupPermissionType.RemoveLocation | LocationGroupPermissionType.DeleteGroup | LocationGroupPermissionType.SentencesInGroup);
			var locationGroupPermission = new LocationGroupPermission(guid, location, LocationGroupPermissionType.UseGroup | LocationGroupPermissionType.RemoveLocation | LocationGroupPermissionType.DeleteGroup | LocationGroupPermissionType.SentencesInGroup);
			var locationAccess = Mock.Of<ILocationAccess>();
			var locationGroupAccess = Mock.Of<ILocationGroupAccess>();
			var locationGroupPermissionAccess = Mock.Of<ILocationGroupPermissionAccess>();
			Mock.Get(locationGroupPermissionAccess)
				.Setup(l => l.ReadLocationGroupPermissionsForOwner(guid, sender))
				.Returns(Task.FromResult(senderLocationGroupPermission.Permissions))
				.Verifiable();
			Mock.Get(locationGroupPermissionAccess)
				.Setup(l => l.ReadLocationGroupPermissionRangeByID(guid))
				.Returns(Task.FromResult<IEnumerable<LocationGroupPermission>>([senderLocationGroupPermission, locationGroupPermission]))
				.Verifiable();
			var locationGroupRequestAccess = Mock.Of<ILocationGroupRequestAccess>();
			var locationSettingAccess = Mock.Of<ILocationSettingAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<LocationGroupManager>>();
			var locationGroupManager = new LocationGroupManager(locationAccess, locationGroupAccess, locationGroupPermissionAccess, locationGroupRequestAccess, locationSettingAccess, authorRetortConfigAccess, logger);
			await locationGroupManager.RemoveLocation(sender, guid, location);
			Mock.Get(locationGroupPermissionAccess).Verify();
			Mock.Get(locationGroupPermissionAccess).Verify(g => g.DeleteLocationFromLocationGroup(guid, location));
		}

		[Fact]
		public async Task GetLocationsForFilterGlobalDisabledTest()
		{
			var locationGroupPermission = new LocationGroupPermission(guid, location, LocationGroupPermissionType.UseGroup | LocationGroupPermissionType.RemoveLocation | LocationGroupPermissionType.DeleteGroup | LocationGroupPermissionType.SentencesInGroup);
			var locationGroupPermission2 = new LocationGroupPermission(guid, senderLocation, LocationGroupPermissionType.UseGroup | LocationGroupPermissionType.RemoveLocation | LocationGroupPermissionType.DeleteGroup | LocationGroupPermissionType.SentencesInGroup);
			var locationGroupPermission3 = new LocationGroupPermission(guid, DiscordObjectOID.Parse("Discord:discord.com:1312182108013465620"), LocationGroupPermissionType.UseGroup | LocationGroupPermissionType.RemoveLocation | LocationGroupPermissionType.DeleteGroup | LocationGroupPermissionType.SentencesInGroup);
			var locationGroupPermission4 = new LocationGroupPermission(guid, DiscordObjectOID.Parse("Discord:discord.com:302152934249070593"), LocationGroupPermissionType.UseGroup);
			var authorLocationGroup = new Guid("1a0be0c3-32d0-4d0f-93a0-e86620fa848b");
			var channel = DiscordObjectOID.Parse("Discord:discord.com:1290098660385886248:1318052420265316483:1318063513880494132");
			var authorPermissions = LocationGroupPermissionType.AddLocation | LocationGroupPermissionType.UseGroup | LocationGroupPermissionType.SentencesInGroup | LocationGroupPermissionType.DeleteGroup | LocationGroupPermissionType.RemoveLocation | LocationGroupPermissionType.RenameGroup;
			IEnumerable<LocationGroupPermission> authorLocationGroupPermissions = [
				new LocationGroupPermission(authorLocationGroup, location, authorPermissions), // in location group
				new LocationGroupPermission(authorLocationGroup, DiscordObjectOID.Parse("Discord:discord.com:295808243509362700"), authorPermissions), // not in location group
				new LocationGroupPermission(authorLocationGroup, senderLocation, authorPermissions), // in location group
			];
			var filter = new SentenceFilter(
				[
					channel, // transitively in location group and author location group
					DiscordObjectOID.Parse("Discord:discord.com:1312182108013465620:0:1321631281582182400"), // transitively in location group, not in author location group
					DiscordObjectOID.Parse("Discord:discord.com:1315082650603622440"), // not in location group nor author location group
					senderLocation, // in location group and author location group
					DiscordObjectOID.Parse("Discord:discord.com:1290098660385886248:1317994175152525420") // transitively in location group and author location group
				],
				[author]
			);
			var authorRetortConfig = new AuthorRetortConfig(author, channel, DisplayOptionType.Mimic, filter, null, authorLocationGroup);
			var locationAccess = Mock.Of<ILocationAccess>();
			var locationGroupAccess = Mock.Of<ILocationGroupAccess>();
			var locationGroupPermissionAccess = Mock.Of<ILocationGroupPermissionAccess>();
			Mock.Get(locationGroupPermissionAccess)
				.Setup(l => l.ReadLocationGroupPermission(guid, channel))
				.Returns(Task.FromResult<LocationGroupPermission?>(locationGroupPermission))
				.Verifiable();
			Mock.Get(locationGroupPermissionAccess)
				.Setup(l => l.ReadLocationGroupPermissionRangeByID(guid))
				.Returns(Task.FromResult<IEnumerable<LocationGroupPermission>>([locationGroupPermission, locationGroupPermission2, locationGroupPermission3, locationGroupPermission4]));
			Mock.Get(locationGroupPermissionAccess)
				.Setup(l => l.ReadLocationGroupPermissionsForOwner(authorLocationGroup, author))
				.Returns(Task.FromResult(authorPermissions))
				.Verifiable();
			Mock.Get(locationGroupPermissionAccess)
				.Setup(l => l.ReadLocationGroupPermission(authorLocationGroup, channel))
				.Returns(Task.FromResult<LocationGroupPermission?>(null))
				.Verifiable();
			Mock.Get(locationGroupPermissionAccess)
				.Setup(l => l.ReadLocationGroupPermissionRangeByID(authorLocationGroup))
				.Returns(Task.FromResult(authorLocationGroupPermissions))
				.Verifiable();
			var locationGroupRequestAccess = Mock.Of<ILocationGroupRequestAccess>();
			var locationSettingAccess = Mock.Of<ILocationSettingAccess>();
			Mock.Get(locationSettingAccess)
					.Setup(l => l.ReadLocationSettingHierarchy(channel))
					.Returns(Task.FromResult<IEnumerable<LocationSetting>>(
						[
							new(channel, [], [], [], false, null),
							new(location, [], [], [], false, guid),
							new(DiscordObjectOID.Parse("Discord:discord.com"), [], [], [], false, new()) // Can be whatever, because the important thing is we *shouldn't* get here
						]
					))
					.Verifiable();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			Mock.Get(authorRetortConfigAccess)
					.Setup(l => l.ReadAuthorRetortConfig(author, channel))
					.Returns(Task.FromResult<AuthorRetortConfig?>(authorRetortConfig))
					.Verifiable();
			var logger = Mock.Of<ILogger<LocationGroupManager>>();
			var locationGroupManager = new LocationGroupManager(locationAccess, locationGroupAccess, locationGroupPermissionAccess, locationGroupRequestAccess, locationSettingAccess, authorRetortConfigAccess, logger);
			var result = await locationGroupManager.GetLocationsForFilter(author, channel);
			IEnumerable<IObjectOID> expectedData = [
					channel,
					senderLocation,
					DiscordObjectOID.Parse("Discord:discord.com:1290098660385886248:1317994175152525420")
				];
			result.Should().BeEquivalentTo(expectedData);
			Mock.Get(locationGroupPermissionAccess).Verify();
			Mock.Get(locationSettingAccess).Verify();
			Mock.Get(authorRetortConfigAccess).Verify();
		}

		[Fact]
		public async Task GetLocationsForFilterGlobalEnabledTest()
		{
			var authorLocationGroup = new Guid("1a0be0c3-32d0-4d0f-93a0-e86620fa848b");
			var channel = DiscordObjectOID.Parse("Discord:discord.com:1290098660385886248:1318052420265316483:1318063513880494132");
			var authorPermissions = LocationGroupPermissionType.AddLocation | LocationGroupPermissionType.UseGroup | LocationGroupPermissionType.SentencesInGroup | LocationGroupPermissionType.DeleteGroup | LocationGroupPermissionType.RemoveLocation | LocationGroupPermissionType.RenameGroup;
			IEnumerable<LocationGroupPermission> authorLocationGroupPermissions = [
				new LocationGroupPermission(authorLocationGroup, location, authorPermissions),
				new LocationGroupPermission(authorLocationGroup, DiscordObjectOID.Parse("Discord:discord.com:295808243509362700"), authorPermissions),
				new LocationGroupPermission(authorLocationGroup, senderLocation, authorPermissions),
			];
			var filter = new SentenceFilter(
				[
					channel, // transitively in author location group
					DiscordObjectOID.Parse("Discord:discord.com:1312182108013465620:0:1321631281582182400"), // not in author location group
					DiscordObjectOID.Parse("Discord:discord.com:1315082650603622440"), // not in author location group
					senderLocation, // in author location group
					DiscordObjectOID.Parse("Discord:discord.com:1290098660385886248:1317994175152525420") // transitively in author location group
				],
				[author]
			);
			var authorRetortConfig = new AuthorRetortConfig(author, channel, DisplayOptionType.Mimic, filter, null, authorLocationGroup);
			var locationAccess = Mock.Of<ILocationAccess>();
			var locationGroupAccess = Mock.Of<ILocationGroupAccess>();
			var locationGroupPermissionAccess = Mock.Of<ILocationGroupPermissionAccess>();
			Mock.Get(locationGroupPermissionAccess)
				.Setup(l => l.ReadLocationGroupPermissionsForOwner(authorLocationGroup, author))
				.Returns(Task.FromResult(authorPermissions))
				.Verifiable();
			Mock.Get(locationGroupPermissionAccess)
				.Setup(l => l.ReadLocationGroupPermission(authorLocationGroup, channel))
				.Returns(Task.FromResult<LocationGroupPermission?>(null))
				.Verifiable();
			Mock.Get(locationGroupPermissionAccess)
				.Setup(l => l.ReadLocationGroupPermissionRangeByID(authorLocationGroup))
				.Returns(Task.FromResult(authorLocationGroupPermissions))
				.Verifiable();
			var locationGroupRequestAccess = Mock.Of<ILocationGroupRequestAccess>();
			var locationSettingAccess = Mock.Of<ILocationSettingAccess>();
			Mock.Get(locationSettingAccess)
					.Setup(l => l.ReadLocationSettingHierarchy(channel))
					.Returns(Task.FromResult<IEnumerable<LocationSetting>>(
						[
							new(channel, [], [], [], true, null),
							new(location, [], [], [], false, guid), // In this test, this will also be ignored
							new(DiscordObjectOID.Parse("Discord:discord.com"), [], [], [], false, new()) // Can be whatever, because the important thing is we *shouldn't* get here
						]
					))
					.Verifiable();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			Mock.Get(authorRetortConfigAccess)
					.Setup(l => l.ReadAuthorRetortConfig(author, channel))
					.Returns(Task.FromResult<AuthorRetortConfig?>(authorRetortConfig))
					.Verifiable();
			var logger = Mock.Of<ILogger<LocationGroupManager>>();
			var locationGroupManager = new LocationGroupManager(locationAccess, locationGroupAccess, locationGroupPermissionAccess, locationGroupRequestAccess, locationSettingAccess, authorRetortConfigAccess, logger);
			var result = await locationGroupManager.GetLocationsForFilter(author, channel);
			IEnumerable<IObjectOID> expectedData = [
					channel,
					senderLocation,
					DiscordObjectOID.Parse("Discord:discord.com:1290098660385886248:1317994175152525420")
				];
			result.Should().BeEquivalentTo(expectedData);
			Mock.Get(locationGroupPermissionAccess).Verify();
			Mock.Get(locationSettingAccess).Verify();
			Mock.Get(authorRetortConfigAccess).Verify();
		}

		[Fact]
		public async Task GetLocationsForFilterNoGroupsTest()
		{
			var channel = DiscordObjectOID.Parse("Discord:discord.com:1290098660385886248:1318052420265316483:1318063513880494132");
			var filter = new SentenceFilter(
				[
					channel,
					DiscordObjectOID.Parse("Discord:discord.com:1312182108013465620:0:1321631281582182400"),
					DiscordObjectOID.Parse("Discord:discord.com:1315082650603622440"),
					senderLocation,
					DiscordObjectOID.Parse("Discord:discord.com:1290098660385886248:1317994175152525420")
				],
				[author]
			);
			var authorRetortConfig = new AuthorRetortConfig(author, channel, DisplayOptionType.Mimic, filter);
			var locationAccess = Mock.Of<ILocationAccess>();
			var locationGroupAccess = Mock.Of<ILocationGroupAccess>();
			var locationGroupPermissionAccess = Mock.Of<ILocationGroupPermissionAccess>();
			var locationGroupRequestAccess = Mock.Of<ILocationGroupRequestAccess>();
			var locationSettingAccess = Mock.Of<ILocationSettingAccess>();
			Mock.Get(locationSettingAccess)
					.Setup(l => l.ReadLocationSettingHierarchy(channel))
					.Returns(Task.FromResult<IEnumerable<LocationSetting>>(
						[
							new(channel, [], [], [], false, null),
							new(location, [], [], [], false, null),
						]
					))
					.Verifiable();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			Mock.Get(authorRetortConfigAccess)
					.Setup(l => l.ReadAuthorRetortConfig(author, channel))
					.Returns(Task.FromResult<AuthorRetortConfig?>(authorRetortConfig))
					.Verifiable();
			var logger = Mock.Of<ILogger<LocationGroupManager>>();
			var locationGroupManager = new LocationGroupManager(locationAccess, locationGroupAccess, locationGroupPermissionAccess, locationGroupRequestAccess, locationSettingAccess, authorRetortConfigAccess, logger);
			var result = await locationGroupManager.GetLocationsForFilter(author, channel);
			IEnumerable<IObjectOID> expectedData = [
					channel,
					DiscordObjectOID.Parse("Discord:discord.com:1290098660385886248:1317994175152525420")
				];
			result.Should().BeEquivalentTo(expectedData);
			Mock.Get(locationSettingAccess).Verify();
			Mock.Get(authorRetortConfigAccess).Verify();
		}
	}
}
