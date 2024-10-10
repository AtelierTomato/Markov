using AtelierTomato.Markov.Model;
using FluentAssertions;

namespace AtelierTomato.Markov.Storage.Sqlite.Test
{
	public class SqliteLocationGroupPermissionAccessTests
	{
		[Fact]
		public Task ParseAndAggregateLocationGroupPermissionTypeTest()
		{
			IEnumerable<string> str = ["SentencesInGroup", "SentencesInGroup, UseGroup", "RenameGroup, DeleteGroup", "RenameGroup"];
			var result = SqliteLocationGroupPermissionAccess.ParseAndAggregateLocationGroupPermissionType(str);
			result.Should().Be(LocationGroupPermissionType.SentencesInGroup | LocationGroupPermissionType.UseGroup | LocationGroupPermissionType.RenameGroup | LocationGroupPermissionType.DeleteGroup);
			return Task.CompletedTask;
		}

		[Fact]
		public Task ParseAndAggregateLocationGroupPermissionTypeFailTest()
		{
			IEnumerable<string> str = ["SentencesInGroup", "SentencesInGroup, Cheese", "RenameGroup, DeleteGroup", "RenameGroup"];
			Action act = () => SqliteLocationGroupPermissionAccess.ParseAndAggregateLocationGroupPermissionType(str);
			act.Should().Throw<InvalidOperationException>().WithMessage($"One or more of listed permissions is invalid: SentencesInGroup, Cheese");
			return Task.CompletedTask;
		}
	}
}
