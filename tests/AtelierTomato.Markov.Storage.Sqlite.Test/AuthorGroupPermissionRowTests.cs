using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Storage.Sqlite.Model;
using FluentAssertions;

namespace AtelierTomato.Markov.Storage.Sqlite.Test
{
	public class AuthorGroupPermissionRowTests
	{
		[Fact]
		public void AuthorGroupPermissionRowFromAuthorGroupPermissionTest()
		{
			AuthorGroupPermission authorGroupPermission = new(
				1,
				new AuthorOID(ServiceType.Discord, "discord.com", "644249977840730118"),
				AuthorGroupPermissionType.SentencesInGroup |
				AuthorGroupPermissionType.UseGroup |
				AuthorGroupPermissionType.AddAuthor |
				AuthorGroupPermissionType.RemoveAuthor |
				AuthorGroupPermissionType.RenameGroup |
				AuthorGroupPermissionType.DeleteGroup
			);
			AuthorGroupPermissionRow authorGroupPermissionRow = new(authorGroupPermission);
			authorGroupPermissionRow.ID.Should().Be(1);
			authorGroupPermissionRow.Author.Should().Be("Discord:discord.com:644249977840730118");
			authorGroupPermissionRow.Permissions.Should().Be("SentencesInGroup, UseGroup, AddAuthor, RemoveAuthor, RenameGroup, DeleteGroup");
		}

		[Fact]
		public void AuthorGroupPermissionRowToAuthorGroupPermissionTest()
		{
			AuthorGroupPermissionRow authorGroupPermissionRow = new(1, "Discord:discord.com:644249977840730118", "SentencesInGroup, UseGroup, AddAuthor, RemoveAuthor, RenameGroup, DeleteGroup");
			AuthorGroupPermission authorGroupPermission = authorGroupPermissionRow.ToAuthorGroupPermission();
			authorGroupPermission.ID.Should().Be(1);
			authorGroupPermission.Author.Should().BeEquivalentTo(new AuthorOID(ServiceType.Discord, "discord.com", "644249977840730118"));
			authorGroupPermission.Permissions.Should().Be(
				AuthorGroupPermissionType.SentencesInGroup |
				AuthorGroupPermissionType.UseGroup |
				AuthorGroupPermissionType.AddAuthor |
				AuthorGroupPermissionType.RemoveAuthor |
				AuthorGroupPermissionType.RenameGroup |
				AuthorGroupPermissionType.DeleteGroup
			);
		}

		[Fact]
		public void AuthorGroupPermissionRowToAuthorGroupPermissionFailTest()
		{
			AuthorGroupPermissionRow authorGroupPermissionRow = new(1, "Discord:discord.com:644249977840730118", "CheeseInGroup, UseGroup, AddAuthor, RemoveAuthor, RenameGroup, DeleteGroup");
			Action act = () => authorGroupPermissionRow.ToAuthorGroupPermission();
			act.Should().Throw<InvalidOperationException>().WithMessage($"One or more of listed permissions is invalid: {authorGroupPermissionRow.Permissions}");
		}
	}
}
