using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Storage.Sqlite.Model;
using FluentAssertions;

namespace AtelierTomato.Markov.Storage.Sqlite.Test
{
	public class AuthorPermissionRowTests
	{
		[Fact]
		public void AuthorPermissionRowNullTest()
		{
			AuthorPermission authorPermission = new(AuthorOID.Parse("Discord:discord.com:1"), null, null);
			AuthorPermissionRow authorPermissionRow = new AuthorPermissionRow(authorPermission);
			authorPermissionRow.Should().BeEquivalentTo(new AuthorPermissionRow("Discord:discord.com:1", "", null));
		}
	}
}
