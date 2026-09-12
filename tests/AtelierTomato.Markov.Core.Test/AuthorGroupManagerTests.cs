using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Storage;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace AtelierTomato.Markov.Core.Test
{
	public class AuthorGroupManagerTests
	{
		private readonly ulong id = 103;
		private readonly AuthorOID sender = AuthorOID.Parse("Discord:discord.com:142781100152848384");
		private readonly AuthorOID author = AuthorOID.Parse("Discord:discord.com:644249977840730118");

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData(" ")]
		public async Task CreateGroupNameEmptyFailTest(string? name)
		{
			var authorGroupAccess = Mock.Of<IAuthorGroupAccess>();
			var authorGroupPermissionAccess = Mock.Of<IAuthorGroupPermissionAccess>();
			var authorGroupRequestAccess = Mock.Of<IAuthorGroupRequestAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<AuthorGroupManager>>();
			var authorGroupManager = new AuthorGroupManager(authorGroupAccess, authorGroupPermissionAccess, authorGroupRequestAccess, authorRetortConfigAccess, logger);
#pragma warning disable CS8604 // Possible null reference argument.
			Func<Task> act = async () => await authorGroupManager.CreateGroup(sender, name);
#pragma warning restore CS8604 // Possible null reference argument.
			await act.Should().ThrowAsync<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'name')");
		}

		[Fact]
		public async Task CreateGroupTest()
		{
			var authorGroupAccess = Mock.Of<IAuthorGroupAccess>();
			var authorGroupPermissionAccess = Mock.Of<IAuthorGroupPermissionAccess>();
			var authorGroupRequestAccess = Mock.Of<IAuthorGroupRequestAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<AuthorGroupManager>>();
			var authorGroupManager = new AuthorGroupManager(authorGroupAccess, authorGroupPermissionAccess, authorGroupRequestAccess, authorRetortConfigAccess, logger);
			await authorGroupManager.CreateGroup(sender, "MyNewGroup");
			Mock.Get(authorGroupAccess).Verify(g => g.WriteNewAuthorGroup("MyNewGroup"), Times.Once);
			Mock.Get(authorGroupPermissionAccess).Verify(g => g.WriteAuthorGroupPermission(It.IsAny<AuthorGroupPermission>()), Times.Once);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData(" ")]
		public async Task RenameGroupNameEmptyFailTest(string? name)
		{
			var authorGroupAccess = Mock.Of<IAuthorGroupAccess>();
			var authorGroupPermissionAccess = Mock.Of<IAuthorGroupPermissionAccess>();
			var authorGroupRequestAccess = Mock.Of<IAuthorGroupRequestAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<AuthorGroupManager>>();
			var authorGroupManager = new AuthorGroupManager(authorGroupAccess, authorGroupPermissionAccess, authorGroupRequestAccess, authorRetortConfigAccess, logger);
#pragma warning disable CS8604 // Possible null reference argument.
			Func<Task> act = async () => await authorGroupManager.RenameGroup(sender, id, name);
#pragma warning restore CS8604 // Possible null reference argument.
			await act.Should().ThrowAsync<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'name')");
		}

		[Fact]
		public async Task RenameGroupNotRegisteredFailTest()
		{
			var authorGroupAccess = Mock.Of<IAuthorGroupAccess>();
			var authorGroupPermissionAccess = Mock.Of<IAuthorGroupPermissionAccess>();
			Mock.Get(authorGroupPermissionAccess)
				.Setup(a => a.ReadAuthorGroupPermission(id, sender))
				.Returns(Task.FromResult<AuthorGroupPermission?>(null))
				.Verifiable();
			var authorGroupRequestAccess = Mock.Of<IAuthorGroupRequestAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<AuthorGroupManager>>();
			var authorGroupManager = new AuthorGroupManager(authorGroupAccess, authorGroupPermissionAccess, authorGroupRequestAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await authorGroupManager.RenameGroup(sender, id, "MyNewerGroup");
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""Author "{sender}" is not registered to group with ID "{id}". (Parameter 'sender')""");
			Mock.Get(authorGroupPermissionAccess).Verify();
		}

		[Fact]
		public async Task RenameGroupNoPermissionFailTest()
		{
			var authorGroupAccess = Mock.Of<IAuthorGroupAccess>();
			var authorGroupPermissionAccess = Mock.Of<IAuthorGroupPermissionAccess>();
			Mock.Get(authorGroupPermissionAccess)
				.Setup(a => a.ReadAuthorGroupPermission(id, sender))
				.Returns(Task.FromResult<AuthorGroupPermission?>(new AuthorGroupPermission(id, sender, AuthorGroupPermissionType.SentencesInGroup | AuthorGroupPermissionType.UseGroup)))
				.Verifiable();
			var authorGroupRequestAccess = Mock.Of<IAuthorGroupRequestAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<AuthorGroupManager>>();
			var authorGroupManager = new AuthorGroupManager(authorGroupAccess, authorGroupPermissionAccess, authorGroupRequestAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await authorGroupManager.RenameGroup(sender, id, "MyNewerGroup");
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""Author "{sender}" does not have permission to rename group with ID "{id}". (Parameter 'sender')""");
			Mock.Get(authorGroupPermissionAccess).Verify();
		}

		[Fact]
		public async Task RenameGroupTest()
		{
			var authorGroupAccess = Mock.Of<IAuthorGroupAccess>();
			var authorGroupPermissionAccess = Mock.Of<IAuthorGroupPermissionAccess>();
			Mock.Get(authorGroupPermissionAccess)
				.Setup(a => a.ReadAuthorGroupPermission(id, sender))
				.Returns(Task.FromResult<AuthorGroupPermission?>(new AuthorGroupPermission(id, sender, AuthorGroupPermissionType.SentencesInGroup | AuthorGroupPermissionType.UseGroup | AuthorGroupPermissionType.RenameGroup)))
				.Verifiable();
			var authorGroupRequestAccess = Mock.Of<IAuthorGroupRequestAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<AuthorGroupManager>>();
			var authorGroupManager = new AuthorGroupManager(authorGroupAccess, authorGroupPermissionAccess, authorGroupRequestAccess, authorRetortConfigAccess, logger);
			await authorGroupManager.RenameGroup(sender, id, "MyNewerGroup");
			Mock.Get(authorGroupAccess).Verify(g => g.WriteAuthorGroup(new(id, "MyNewerGroup")), Times.Once);
			Mock.Get(authorGroupPermissionAccess).Verify();
		}

		[Fact]
		public async Task DeleteGroupNotRegisteredFailTest()
		{
			var authorGroupAccess = Mock.Of<IAuthorGroupAccess>();
			var authorGroupPermissionAccess = Mock.Of<IAuthorGroupPermissionAccess>();
			Mock.Get(authorGroupPermissionAccess)
				.Setup(a => a.ReadAuthorGroupPermission(id, sender))
				.Returns(Task.FromResult<AuthorGroupPermission?>(null))
				.Verifiable();
			var authorGroupRequestAccess = Mock.Of<IAuthorGroupRequestAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<AuthorGroupManager>>();
			var authorGroupManager = new AuthorGroupManager(authorGroupAccess, authorGroupPermissionAccess, authorGroupRequestAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await authorGroupManager.DeleteGroup(sender, id);
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""Author "{sender}" is not registered to group with ID "{id}". (Parameter 'sender')""");
			Mock.Get(authorGroupPermissionAccess).Verify();
		}

		[Fact]
		public async Task DeleteGroupNoPermissionFailTest()
		{
			var authorGroupAccess = Mock.Of<IAuthorGroupAccess>();
			var authorGroupPermissionAccess = Mock.Of<IAuthorGroupPermissionAccess>();
			Mock.Get(authorGroupPermissionAccess)
				.Setup(a => a.ReadAuthorGroupPermission(id, sender))
				.Returns(Task.FromResult<AuthorGroupPermission?>(new AuthorGroupPermission(id, sender, AuthorGroupPermissionType.SentencesInGroup | AuthorGroupPermissionType.UseGroup)))
				.Verifiable();
			var authorGroupRequestAccess = Mock.Of<IAuthorGroupRequestAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<AuthorGroupManager>>();
			var authorGroupManager = new AuthorGroupManager(authorGroupAccess, authorGroupPermissionAccess, authorGroupRequestAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await authorGroupManager.DeleteGroup(sender, id);
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""Author "{sender}" does not have permission to delete group with ID "{id}". (Parameter 'sender')""");
			Mock.Get(authorGroupPermissionAccess).Verify();
		}

		[Fact]
		public async Task DeleteGroupTest()
		{
			var authorGroupAccess = Mock.Of<IAuthorGroupAccess>();
			var authorGroupPermissionAccess = Mock.Of<IAuthorGroupPermissionAccess>();
			Mock.Get(authorGroupPermissionAccess)
				.Setup(a => a.ReadAuthorGroupPermission(id, sender))
				.Returns(Task.FromResult<AuthorGroupPermission?>(new AuthorGroupPermission(id, sender, AuthorGroupPermissionType.SentencesInGroup | AuthorGroupPermissionType.UseGroup | AuthorGroupPermissionType.DeleteGroup)))
				.Verifiable();
			var authorGroupRequestAccess = Mock.Of<IAuthorGroupRequestAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<AuthorGroupManager>>();
			var authorGroupManager = new AuthorGroupManager(authorGroupAccess, authorGroupPermissionAccess, authorGroupRequestAccess, authorRetortConfigAccess, logger);
			await authorGroupManager.DeleteGroup(sender, id);
			Mock.Get(authorGroupAccess).Verify(g => g.DeleteAuthorGroup(id), Times.Once);
			Mock.Get(authorGroupPermissionAccess).Verify();
		}

		[Fact]
		public async Task SendOrUpdateAuthorGroupRequestInviteSelfFailTest()
		{
			var authorGroupAccess = Mock.Of<IAuthorGroupAccess>();
			var authorGroupPermissionAccess = Mock.Of<IAuthorGroupPermissionAccess>();
			var authorGroupRequestAccess = Mock.Of<IAuthorGroupRequestAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<AuthorGroupManager>>();
			var authorGroupManager = new AuthorGroupManager(authorGroupAccess, authorGroupPermissionAccess, authorGroupRequestAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await authorGroupManager.SendOrUpdateAuthorGroupRequest(sender, new(id, sender, new()));
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"You cannot invite yourself to a group. (Parameter 'sender')");
		}

		[Fact]
		public async Task SendOrUpdateAuthorGroupRequestNotRegisteredFailTest()
		{
			var authorGroupAccess = Mock.Of<IAuthorGroupAccess>();
			var authorGroupPermissionAccess = Mock.Of<IAuthorGroupPermissionAccess>();
			Mock.Get(authorGroupPermissionAccess)
				.Setup(a => a.ReadAuthorGroupPermission(id, sender))
				.Returns(Task.FromResult<AuthorGroupPermission?>(null))
				.Verifiable();
			var authorGroupRequestAccess = Mock.Of<IAuthorGroupRequestAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<AuthorGroupManager>>();
			var authorGroupManager = new AuthorGroupManager(authorGroupAccess, authorGroupPermissionAccess, authorGroupRequestAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await authorGroupManager.SendOrUpdateAuthorGroupRequest(sender, new(id, author, new()));
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""Author "{sender}" is not registered to group with ID "{id}". (Parameter 'sender')""");
			Mock.Get(authorGroupPermissionAccess).Verify();
		}

		[Fact]
		public async Task SendOrUpdateAuthorGroupRequestNoPermissionFailTest()
		{
			var authorGroupAccess = Mock.Of<IAuthorGroupAccess>();
			var authorGroupPermissionAccess = Mock.Of<IAuthorGroupPermissionAccess>();
			Mock.Get(authorGroupPermissionAccess)
				.Setup(a => a.ReadAuthorGroupPermission(id, sender))
				.Returns(Task.FromResult<AuthorGroupPermission?>(new AuthorGroupPermission(id, sender, AuthorGroupPermissionType.SentencesInGroup | AuthorGroupPermissionType.UseGroup)))
				.Verifiable();
			var authorGroupRequestAccess = Mock.Of<IAuthorGroupRequestAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<AuthorGroupManager>>();
			var authorGroupManager = new AuthorGroupManager(authorGroupAccess, authorGroupPermissionAccess, authorGroupRequestAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await authorGroupManager.SendOrUpdateAuthorGroupRequest(sender, new(id, author, new()));
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""Author "{sender}" does not have permission to add authors to group with ID "{id}". (Parameter 'sender')""");
			Mock.Get(authorGroupPermissionAccess).Verify();
		}

		[Fact]
		public async Task SendOrUpdateAuthorGroupRequestAlreadyRegisteredFailTest()
		{
			var authorGroupAccess = Mock.Of<IAuthorGroupAccess>();
			var authorGroupPermissionAccess = Mock.Of<IAuthorGroupPermissionAccess>();
			Mock.Get(authorGroupPermissionAccess)
				.Setup(a => a.ReadAuthorGroupPermission(id, sender))
				.Returns(Task.FromResult<AuthorGroupPermission?>(new AuthorGroupPermission(id, sender, AuthorGroupPermissionType.SentencesInGroup | AuthorGroupPermissionType.UseGroup | AuthorGroupPermissionType.AddAuthor)))
				.Verifiable();
			Mock.Get(authorGroupPermissionAccess)
				.Setup(a => a.ReadAuthorGroupPermission(id, author))
				.Returns(Task.FromResult<AuthorGroupPermission?>(new AuthorGroupPermission(id, author, AuthorGroupPermissionType.SentencesInGroup | AuthorGroupPermissionType.UseGroup)))
				.Verifiable();
			var authorGroupRequestAccess = Mock.Of<IAuthorGroupRequestAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<AuthorGroupManager>>();
			var authorGroupManager = new AuthorGroupManager(authorGroupAccess, authorGroupPermissionAccess, authorGroupRequestAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await authorGroupManager.SendOrUpdateAuthorGroupRequest(sender, new(id, author, new()));
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""Author "{author}" is already registered to group with ID "{id}". (Parameter 'authorGroupPermission')""");
			Mock.Get(authorGroupPermissionAccess).Verify();
		}

		[Theory]
		[InlineData(AuthorGroupPermissionType.AddAuthor, AuthorGroupPermissionType.SentencesInGroup)]
		[InlineData(AuthorGroupPermissionType.AddAuthor, AuthorGroupPermissionType.RemoveAuthor)]
		[InlineData(AuthorGroupPermissionType.AddAuthor, AuthorGroupPermissionType.DeleteGroup)]
		[InlineData(AuthorGroupPermissionType.AddAuthor, AuthorGroupPermissionType.UseGroup)]
		[InlineData(AuthorGroupPermissionType.AddAuthor, AuthorGroupPermissionType.RenameGroup)]
		public async Task SendOrUpdateAuthorGroupRequestAddLackingPermissionsFailTest(AuthorGroupPermissionType senderPerms, AuthorGroupPermissionType authorPerms)
		{
			var authorGroupAccess = Mock.Of<IAuthorGroupAccess>();
			var authorGroupPermissionAccess = Mock.Of<IAuthorGroupPermissionAccess>();
			Mock.Get(authorGroupPermissionAccess)
				.Setup(a => a.ReadAuthorGroupPermission(id, sender))
				.Returns(Task.FromResult<AuthorGroupPermission?>(new AuthorGroupPermission(id, sender, senderPerms)))
				.Verifiable();
			var authorGroupRequestAccess = Mock.Of<IAuthorGroupRequestAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<AuthorGroupManager>>();
			var authorGroupManager = new AuthorGroupManager(authorGroupAccess, authorGroupPermissionAccess, authorGroupRequestAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await authorGroupManager.SendOrUpdateAuthorGroupRequest(sender, new(id, author, authorPerms));
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""Author "{sender}" does not have some of the permissions they are trying to assign. (Parameter 'authorGroupPermission')""");
			Mock.Get(authorGroupPermissionAccess).Verify();
		}

		[Fact]
		public async Task SendOrUpdateAuthorGroupRequestTest()
		{
			var authorGroupAccess = Mock.Of<IAuthorGroupAccess>();
			var authorGroupPermissionAccess = Mock.Of<IAuthorGroupPermissionAccess>();
			Mock.Get(authorGroupPermissionAccess)
				.Setup(a => a.ReadAuthorGroupPermission(id, sender))
				.Returns(Task.FromResult<AuthorGroupPermission?>(new AuthorGroupPermission(id, sender, AuthorGroupPermissionType.SentencesInGroup | AuthorGroupPermissionType.UseGroup | AuthorGroupPermissionType.AddAuthor)))
				.Verifiable();
			var authorGroupRequestAccess = Mock.Of<IAuthorGroupRequestAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<AuthorGroupManager>>();
			var authorGroupManager = new AuthorGroupManager(authorGroupAccess, authorGroupPermissionAccess, authorGroupRequestAccess, authorRetortConfigAccess, logger);
			AuthorGroupPermission authorGroupPermission = new(id, author, new());
			await authorGroupManager.SendOrUpdateAuthorGroupRequest(sender, authorGroupPermission);
			Mock.Get(authorGroupRequestAccess).Verify(r => r.WriteAuthorGroupRequest(authorGroupPermission), Times.Once);
			Mock.Get(authorGroupPermissionAccess).Verify();
		}

		[Fact]
		public async Task AcceptInvitationNoInviteFailTest()
		{
			var authorGroupAccess = Mock.Of<IAuthorGroupAccess>();
			var authorGroupPermissionAccess = Mock.Of<IAuthorGroupPermissionAccess>();
			var authorGroupRequestAccess = Mock.Of<IAuthorGroupRequestAccess>();
			Mock.Get(authorGroupRequestAccess)
				.Setup(a => a.ReadAuthorGroupRequest(id, sender))
				.Returns(Task.FromResult<AuthorGroupPermission?>(null))
				.Verifiable();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<AuthorGroupManager>>();
			var authorGroupManager = new AuthorGroupManager(authorGroupAccess, authorGroupPermissionAccess, authorGroupRequestAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await authorGroupManager.AcceptInvitation(sender, id);
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""Author "{sender}" has not been sent an invitation to group with ID "{id}". (Parameter 'ID')""");
			Mock.Get(authorGroupRequestAccess).Verify();
		}

		[Fact]
		public async Task AcceptInvitationTest()
		{
			var authorGroupAccess = Mock.Of<IAuthorGroupAccess>();
			var authorGroupPermissionAccess = Mock.Of<IAuthorGroupPermissionAccess>();
			var authorGroupRequestAccess = Mock.Of<IAuthorGroupRequestAccess>();
			AuthorGroupPermission? authorGroupPermission = new AuthorGroupPermission(id, sender, AuthorGroupPermissionType.SentencesInGroup | AuthorGroupPermissionType.UseGroup | AuthorGroupPermissionType.AddAuthor);
			Mock.Get(authorGroupRequestAccess)
				.Setup(a => a.ReadAuthorGroupRequest(id, sender))
				.Returns(Task.FromResult<AuthorGroupPermission?>(authorGroupPermission))
				.Verifiable();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<AuthorGroupManager>>();
			var authorGroupManager = new AuthorGroupManager(authorGroupAccess, authorGroupPermissionAccess, authorGroupRequestAccess, authorRetortConfigAccess, logger);
			await authorGroupManager.AcceptInvitation(sender, id);
			Mock.Get(authorGroupRequestAccess).Verify(r => r.DeleteAuthorGroupRequest(id, sender), Times.Once);
			Mock.Get(authorGroupPermissionAccess).Verify(p => p.WriteAuthorGroupPermission(authorGroupPermission), Times.Once);
		}

		[Fact]
		public async Task DenyInvitationNoInviteFailTest()
		{
			var authorGroupAccess = Mock.Of<IAuthorGroupAccess>();
			var authorGroupPermissionAccess = Mock.Of<IAuthorGroupPermissionAccess>();
			var authorGroupRequestAccess = Mock.Of<IAuthorGroupRequestAccess>();
			Mock.Get(authorGroupRequestAccess)
				.Setup(a => a.ReadAuthorGroupRequest(id, sender))
				.Returns(Task.FromResult<AuthorGroupPermission?>(null))
				.Verifiable();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<AuthorGroupManager>>();
			var authorGroupManager = new AuthorGroupManager(authorGroupAccess, authorGroupPermissionAccess, authorGroupRequestAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await authorGroupManager.DenyInvitation(sender, id);
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""Author "{sender}" has not been sent an invitation to group with ID "{id}". (Parameter 'ID')""");
			Mock.Get(authorGroupRequestAccess).Verify();
		}

		[Fact]
		public async Task DenyInvitationTest()
		{
			var authorGroupAccess = Mock.Of<IAuthorGroupAccess>();
			var authorGroupPermissionAccess = Mock.Of<IAuthorGroupPermissionAccess>();
			var authorGroupRequestAccess = Mock.Of<IAuthorGroupRequestAccess>();
			AuthorGroupPermission? authorGroupPermission = new AuthorGroupPermission(id, sender, AuthorGroupPermissionType.SentencesInGroup | AuthorGroupPermissionType.UseGroup | AuthorGroupPermissionType.AddAuthor);
			Mock.Get(authorGroupRequestAccess)
				.Setup(a => a.ReadAuthorGroupRequest(id, sender))
				.Returns(Task.FromResult<AuthorGroupPermission?>(authorGroupPermission))
				.Verifiable();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<AuthorGroupManager>>();
			var authorGroupManager = new AuthorGroupManager(authorGroupAccess, authorGroupPermissionAccess, authorGroupRequestAccess, authorRetortConfigAccess, logger);
			await authorGroupManager.DenyInvitation(sender, id);
			Mock.Get(authorGroupRequestAccess).Verify(r => r.DeleteAuthorGroupRequest(id, sender), Times.Once);
		}

		[Fact]
		public async Task UpdateAuthorUpdateSelfFailTest()
		{
			var authorGroupAccess = Mock.Of<IAuthorGroupAccess>();
			var authorGroupPermissionAccess = Mock.Of<IAuthorGroupPermissionAccess>();
			var authorGroupRequestAccess = Mock.Of<IAuthorGroupRequestAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<AuthorGroupManager>>();
			var authorGroupManager = new AuthorGroupManager(authorGroupAccess, authorGroupPermissionAccess, authorGroupRequestAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await authorGroupManager.UpdateAuthor(sender, new(id, sender, new()));
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"You cannot update your own permissions in a group. (Parameter 'sender')");
		}
		[Fact]
		public async Task UpdateAuthorNotRegisteredFailTest()
		{
			var authorGroupAccess = Mock.Of<IAuthorGroupAccess>();
			var authorGroupPermissionAccess = Mock.Of<IAuthorGroupPermissionAccess>();
			Mock.Get(authorGroupPermissionAccess)
				.Setup(a => a.ReadAuthorGroupPermission(id, sender))
				.Returns(Task.FromResult<AuthorGroupPermission?>(null))
				.Verifiable();
			var authorGroupRequestAccess = Mock.Of<IAuthorGroupRequestAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<AuthorGroupManager>>();
			var authorGroupManager = new AuthorGroupManager(authorGroupAccess, authorGroupPermissionAccess, authorGroupRequestAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await authorGroupManager.UpdateAuthor(sender, new(id, author, new()));
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""Author "{sender}" is not registered to group with ID "{id}". (Parameter 'sender')""");
			Mock.Get(authorGroupPermissionAccess).Verify();
		}

		[Fact]
		public async Task UpdateAuthorNoPermissionFailTest()
		{
			var authorGroupAccess = Mock.Of<IAuthorGroupAccess>();
			var authorGroupPermissionAccess = Mock.Of<IAuthorGroupPermissionAccess>();
			Mock.Get(authorGroupPermissionAccess)
				.Setup(a => a.ReadAuthorGroupPermission(id, sender))
				.Returns(Task.FromResult<AuthorGroupPermission?>(new AuthorGroupPermission(id, sender, AuthorGroupPermissionType.SentencesInGroup | AuthorGroupPermissionType.UseGroup)))
				.Verifiable();
			var authorGroupRequestAccess = Mock.Of<IAuthorGroupRequestAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<AuthorGroupManager>>();
			var authorGroupManager = new AuthorGroupManager(authorGroupAccess, authorGroupPermissionAccess, authorGroupRequestAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await authorGroupManager.UpdateAuthor(sender, new(id, author, new()));
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""Author "{sender}" does not have permission to add authors to group with ID "{id}". (Parameter 'sender')""");
			Mock.Get(authorGroupPermissionAccess).Verify();
		}

		[Theory]
		[InlineData(AuthorGroupPermissionType.AddAuthor, AuthorGroupPermissionType.SentencesInGroup)]
		[InlineData(AuthorGroupPermissionType.AddAuthor, AuthorGroupPermissionType.RemoveAuthor)]
		[InlineData(AuthorGroupPermissionType.AddAuthor, AuthorGroupPermissionType.DeleteGroup)]
		[InlineData(AuthorGroupPermissionType.AddAuthor, AuthorGroupPermissionType.UseGroup)]
		[InlineData(AuthorGroupPermissionType.AddAuthor, AuthorGroupPermissionType.RenameGroup)]
		public async Task UpdateAuthorLackingPermissionsFailTest(AuthorGroupPermissionType senderPerms, AuthorGroupPermissionType authorPerms)
		{
			var authorGroupAccess = Mock.Of<IAuthorGroupAccess>();
			var authorGroupPermissionAccess = Mock.Of<IAuthorGroupPermissionAccess>();
			Mock.Get(authorGroupPermissionAccess)
				.Setup(a => a.ReadAuthorGroupPermission(id, sender))
				.Returns(Task.FromResult<AuthorGroupPermission?>(new AuthorGroupPermission(id, sender, senderPerms)))
				.Verifiable();
			var authorGroupRequestAccess = Mock.Of<IAuthorGroupRequestAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<AuthorGroupManager>>();
			var authorGroupManager = new AuthorGroupManager(authorGroupAccess, authorGroupPermissionAccess, authorGroupRequestAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await authorGroupManager.UpdateAuthor(sender, new(id, author, authorPerms));
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""Author "{sender}" does not have some of the permissions they are trying to assign. (Parameter 'authorGroupPermission')""");
			Mock.Get(authorGroupPermissionAccess).Verify();
		}

		[Fact]
		public async Task UpdateAuthorTest()
		{
			var authorGroupAccess = Mock.Of<IAuthorGroupAccess>();
			var authorGroupPermissionAccess = Mock.Of<IAuthorGroupPermissionAccess>();
			Mock.Get(authorGroupPermissionAccess)
				.Setup(a => a.ReadAuthorGroupPermission(id, sender))
				.Returns(Task.FromResult<AuthorGroupPermission?>(new AuthorGroupPermission(id, sender, AuthorGroupPermissionType.SentencesInGroup | AuthorGroupPermissionType.UseGroup | AuthorGroupPermissionType.AddAuthor)))
				.Verifiable();
			Mock.Get(authorGroupPermissionAccess)
				.Setup(a => a.ReadAuthorGroupPermission(id, author))
				.Returns(Task.FromResult<AuthorGroupPermission?>(new AuthorGroupPermission(id, sender, AuthorGroupPermissionType.SentencesInGroup | AuthorGroupPermissionType.UseGroup)))
				.Verifiable();
			var authorGroupRequestAccess = Mock.Of<IAuthorGroupRequestAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<AuthorGroupManager>>();
			var authorGroupManager = new AuthorGroupManager(authorGroupAccess, authorGroupPermissionAccess, authorGroupRequestAccess, authorRetortConfigAccess, logger);
			AuthorGroupPermission authorGroupPermission = new(id, author, new());
			await authorGroupManager.UpdateAuthor(sender, authorGroupPermission);
			Mock.Get(authorGroupPermissionAccess).Verify();
			Mock.Get(authorGroupPermissionAccess).Verify(p => p.WriteAuthorGroupPermission(authorGroupPermission), Times.Once);
		}

		[Fact]
		public async Task RemoveAuthorRemoveSelfFailTest()
		{
			var authorGroupAccess = Mock.Of<IAuthorGroupAccess>();
			var authorGroupPermissionAccess = Mock.Of<IAuthorGroupPermissionAccess>();
			var authorGroupRequestAccess = Mock.Of<IAuthorGroupRequestAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<AuthorGroupManager>>();
			var authorGroupManager = new AuthorGroupManager(authorGroupAccess, authorGroupPermissionAccess, authorGroupRequestAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await authorGroupManager.RemoveAuthor(sender, id, sender);
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""You cannot remove yourself from a group. Please use the "{nameof(AuthorGroupManager.LeaveGroup)}" function instead. (Parameter 'user')""");
		}

		[Fact]
		public async Task RemoveAuthorNotRegisteredFailTest()
		{
			var authorGroupAccess = Mock.Of<IAuthorGroupAccess>();
			var authorGroupPermissionAccess = Mock.Of<IAuthorGroupPermissionAccess>();
			Mock.Get(authorGroupPermissionAccess)
				.Setup(a => a.ReadAuthorGroupPermission(id, sender))
				.Returns(Task.FromResult<AuthorGroupPermission?>(null))
				.Verifiable();
			var authorGroupRequestAccess = Mock.Of<IAuthorGroupRequestAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<AuthorGroupManager>>();
			var authorGroupManager = new AuthorGroupManager(authorGroupAccess, authorGroupPermissionAccess, authorGroupRequestAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await authorGroupManager.RemoveAuthor(sender, id, author);
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""Author "{sender}" is not registered to group with ID "{id}". (Parameter 'sender')""");
			Mock.Get(authorGroupPermissionAccess).Verify();
		}

		[Fact]
		public async Task RemoveAuthorNoPermissionFailTest()
		{
			var authorGroupAccess = Mock.Of<IAuthorGroupAccess>();
			var authorGroupPermissionAccess = Mock.Of<IAuthorGroupPermissionAccess>();
			Mock.Get(authorGroupPermissionAccess)
				.Setup(a => a.ReadAuthorGroupPermission(id, sender))
				.Returns(Task.FromResult<AuthorGroupPermission?>(new AuthorGroupPermission(id, sender, AuthorGroupPermissionType.SentencesInGroup | AuthorGroupPermissionType.UseGroup)))
				.Verifiable();
			var authorGroupRequestAccess = Mock.Of<IAuthorGroupRequestAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<AuthorGroupManager>>();
			var authorGroupManager = new AuthorGroupManager(authorGroupAccess, authorGroupPermissionAccess, authorGroupRequestAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await authorGroupManager.RemoveAuthor(sender, id, author);
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""Author "{sender}" does not have permission to remove authors from group with ID "{id}". (Parameter 'sender')""");
			Mock.Get(authorGroupPermissionAccess).Verify();
		}

		[Fact]
		public async Task RemoveAuthorAuthorNotRegisteredTest()
		{
			var authorGroupAccess = Mock.Of<IAuthorGroupAccess>();
			var authorGroupPermissionAccess = Mock.Of<IAuthorGroupPermissionAccess>();
			Mock.Get(authorGroupPermissionAccess)
				.Setup(a => a.ReadAuthorGroupPermission(id, sender))
				.Returns(Task.FromResult<AuthorGroupPermission?>(new AuthorGroupPermission(id, sender, AuthorGroupPermissionType.SentencesInGroup | AuthorGroupPermissionType.UseGroup | AuthorGroupPermissionType.RemoveAuthor)))
				.Verifiable();
			Mock.Get(authorGroupPermissionAccess)
				.Setup(a => a.ReadAuthorGroupPermission(id, author))
				.Returns(Task.FromResult<AuthorGroupPermission?>(null))
				.Verifiable();
			var authorGroupRequestAccess = Mock.Of<IAuthorGroupRequestAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<AuthorGroupManager>>();
			var authorGroupManager = new AuthorGroupManager(authorGroupAccess, authorGroupPermissionAccess, authorGroupRequestAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await authorGroupManager.RemoveAuthor(sender, id, author);
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""Author "{author}" is not registered to group with ID "{id}". (Parameter 'user')""");
			Mock.Get(authorGroupPermissionAccess).Verify();
		}

		[Theory]
		[InlineData(AuthorGroupPermissionType.RemoveAuthor, AuthorGroupPermissionType.SentencesInGroup)]
		[InlineData(AuthorGroupPermissionType.RemoveAuthor, AuthorGroupPermissionType.AddAuthor)]
		[InlineData(AuthorGroupPermissionType.RemoveAuthor, AuthorGroupPermissionType.DeleteGroup)]
		[InlineData(AuthorGroupPermissionType.RemoveAuthor, AuthorGroupPermissionType.UseGroup)]
		[InlineData(AuthorGroupPermissionType.RemoveAuthor, AuthorGroupPermissionType.RenameGroup)]
		public async Task RemoveAuthorLackingPermissionsFailTest(AuthorGroupPermissionType senderPerms, AuthorGroupPermissionType authorPerms)
		{
			var authorGroupAccess = Mock.Of<IAuthorGroupAccess>();
			var authorGroupPermissionAccess = Mock.Of<IAuthorGroupPermissionAccess>();
			Mock.Get(authorGroupPermissionAccess)
				.Setup(a => a.ReadAuthorGroupPermission(id, sender))
				.Returns(Task.FromResult<AuthorGroupPermission?>(new AuthorGroupPermission(id, sender, senderPerms)))
				.Verifiable();
			Mock.Get(authorGroupPermissionAccess)
				.Setup(a => a.ReadAuthorGroupPermission(id, author))
				.Returns(Task.FromResult<AuthorGroupPermission?>(new AuthorGroupPermission(id, author, authorPerms)))
				.Verifiable();
			var authorGroupRequestAccess = Mock.Of<IAuthorGroupRequestAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<AuthorGroupManager>>();
			var authorGroupManager = new AuthorGroupManager(authorGroupAccess, authorGroupPermissionAccess, authorGroupRequestAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await authorGroupManager.RemoveAuthor(sender, id, author);
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""Author "{sender}" does not have some of the permissions that the author they are trying to remove has. (Parameter 'sender')""");
			Mock.Get(authorGroupPermissionAccess).Verify();
		}

		[Fact]
		public async Task RemoveAuthorTest()
		{
			var authorGroupAccess = Mock.Of<IAuthorGroupAccess>();
			var authorGroupPermissionAccess = Mock.Of<IAuthorGroupPermissionAccess>();
			Mock.Get(authorGroupPermissionAccess)
				.Setup(a => a.ReadAuthorGroupPermission(id, sender))
				.Returns(Task.FromResult<AuthorGroupPermission?>(new AuthorGroupPermission(id, sender, AuthorGroupPermissionType.SentencesInGroup | AuthorGroupPermissionType.UseGroup | AuthorGroupPermissionType.RemoveAuthor)))
				.Verifiable();
			Mock.Get(authorGroupPermissionAccess)
				.Setup(a => a.ReadAuthorGroupPermission(id, author))
				.Returns(Task.FromResult<AuthorGroupPermission?>(new AuthorGroupPermission(id, author, AuthorGroupPermissionType.SentencesInGroup | AuthorGroupPermissionType.UseGroup)))
				.Verifiable();
			var authorGroupRequestAccess = Mock.Of<IAuthorGroupRequestAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<AuthorGroupManager>>();
			var authorGroupManager = new AuthorGroupManager(authorGroupAccess, authorGroupPermissionAccess, authorGroupRequestAccess, authorRetortConfigAccess, logger);
			await authorGroupManager.RemoveAuthor(sender, id, author);
			Mock.Get(authorGroupPermissionAccess).Verify();
			Mock.Get(authorGroupPermissionAccess).Verify(p => p.DeleteAuthorFromAuthorGroup(id, author), Times.Once);
		}

		[Fact]
		public async Task LeaveGroupNotRegisteredFailTest()
		{
			var authorGroupAccess = Mock.Of<IAuthorGroupAccess>();
			var authorGroupPermissionAccess = Mock.Of<IAuthorGroupPermissionAccess>();
			Mock.Get(authorGroupPermissionAccess)
				.Setup(a => a.ReadAuthorGroupPermissionRangeByID(id))
				.Returns(Task.FromResult<IEnumerable<AuthorGroupPermission>>([]))
				.Verifiable();
			var authorGroupRequestAccess = Mock.Of<IAuthorGroupRequestAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<AuthorGroupManager>>();
			var authorGroupManager = new AuthorGroupManager(authorGroupAccess, authorGroupPermissionAccess, authorGroupRequestAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await authorGroupManager.LeaveGroup(sender, id);
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""Author "{sender}" cannot leave group with ID "{id}" as they are not registered to it. (Parameter 'sender')""");
			Mock.Get(authorGroupPermissionAccess).Verify();
		}

		[Fact]
		public async Task LeaveGroupOrphanedFailTest()
		{
			var authorGroupAccess = Mock.Of<IAuthorGroupAccess>();
			var authorGroupPermissionAccess = Mock.Of<IAuthorGroupPermissionAccess>();
			Mock.Get(authorGroupPermissionAccess)
				.Setup(a => a.ReadAuthorGroupPermissionRangeByID(id))
				.Returns(Task.FromResult<IEnumerable<AuthorGroupPermission>>([new(id, sender, AuthorGroupPermissionType.SentencesInGroup | AuthorGroupPermissionType.UseGroup)]))
				.Verifiable();
			var authorGroupRequestAccess = Mock.Of<IAuthorGroupRequestAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<AuthorGroupManager>>();
			var authorGroupManager = new AuthorGroupManager(authorGroupAccess, authorGroupPermissionAccess, authorGroupRequestAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await authorGroupManager.LeaveGroup(sender, id);
			await act.Should().ThrowAsync<InvalidOperationException>().WithMessage($"""The {nameof(AuthorGroup)} with ID "{id}" has no members with permission {nameof(AuthorGroupPermissionType.DeleteGroup)}. This is unexpected.""");
			Mock.Get(authorGroupPermissionAccess).Verify();
		}

		[Fact]
		public async Task LeaveGroupLastMemberFailTest()
		{
			var authorGroupAccess = Mock.Of<IAuthorGroupAccess>();
			var authorGroupPermissionAccess = Mock.Of<IAuthorGroupPermissionAccess>();
			Mock.Get(authorGroupPermissionAccess)
				.Setup(a => a.ReadAuthorGroupPermissionRangeByID(id))
				.Returns(Task.FromResult<IEnumerable<AuthorGroupPermission>>([new(id, sender, AuthorGroupPermissionType.SentencesInGroup | AuthorGroupPermissionType.UseGroup | AuthorGroupPermissionType.DeleteGroup)]))
				.Verifiable();
			var authorGroupRequestAccess = Mock.Of<IAuthorGroupRequestAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<AuthorGroupManager>>();
			var authorGroupManager = new AuthorGroupManager(authorGroupAccess, authorGroupPermissionAccess, authorGroupRequestAccess, authorRetortConfigAccess, logger);
			Func<Task> act = async () => await authorGroupManager.LeaveGroup(sender, id);
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"""Author "{sender}" cannot leave group with ID "{id}" as they are the only member of it that has the permission {nameof(AuthorGroupPermissionType.DeleteGroup)}. Please use "{nameof(AuthorGroupManager.DeleteGroup)}" function instead. (Parameter 'sender')""");
			Mock.Get(authorGroupPermissionAccess).Verify();
		}

		[Fact]
		public async Task LeaveGroupTest()
		{
			var authorGroupAccess = Mock.Of<IAuthorGroupAccess>();
			var authorGroupPermissionAccess = Mock.Of<IAuthorGroupPermissionAccess>();
			Mock.Get(authorGroupPermissionAccess)
				.Setup(a => a.ReadAuthorGroupPermissionRangeByID(id))
				.Returns(Task.FromResult<IEnumerable<AuthorGroupPermission>>([new(id, sender, AuthorGroupPermissionType.SentencesInGroup | AuthorGroupPermissionType.UseGroup | AuthorGroupPermissionType.DeleteGroup), new(id, author, AuthorGroupPermissionType.SentencesInGroup | AuthorGroupPermissionType.UseGroup | AuthorGroupPermissionType.DeleteGroup)]))
				.Verifiable();
			var authorGroupRequestAccess = Mock.Of<IAuthorGroupRequestAccess>();
			var authorRetortConfigAccess = Mock.Of<IAuthorRetortConfigAccess>();
			var logger = Mock.Of<ILogger<AuthorGroupManager>>();
			var authorGroupManager = new AuthorGroupManager(authorGroupAccess, authorGroupPermissionAccess, authorGroupRequestAccess, authorRetortConfigAccess, logger);
			await authorGroupManager.LeaveGroup(sender, id);
			Mock.Get(authorGroupPermissionAccess).Verify();
			Mock.Get(authorGroupPermissionAccess).Verify(p => p.DeleteAuthorFromAuthorGroup(id, sender), Times.Once);
		}
	}
}
