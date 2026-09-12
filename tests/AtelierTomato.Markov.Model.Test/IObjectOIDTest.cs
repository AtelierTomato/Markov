using AtelierTomato.Markov.Model.ObjectOID;
using FluentAssertions;

namespace AtelierTomato.Markov.Model.Test
{
	public class IObjectOIDTest
	{
		private const string Instance = "discord.com";
		private const ulong Server1 = 1312182108013465620;
		private const ulong Server2 = 1127358996890796062;
		private const ulong Category = 1312182109150117898;
		private const ulong CategoryMissingLastNumber = 131218210915011789;

		[Fact]
		public void IObjectOIDEqualsTest()
		{
			var oid1 = DiscordObjectOID.ForServer(Instance, Server1);
			var oid2 = DiscordObjectOID.Parse($"{ServiceType.Discord}:{Instance}:{Server1}");
			(oid1 == oid2).Should().BeTrue();
			(oid1 != oid2).Should().BeFalse();
			(oid1 >= oid2).Should().BeTrue();
			(oid1 <= oid2).Should().BeTrue();
			oid1.IsParentOrEqualTo(oid2).Should().BeTrue();
			oid1.IsChildOrEqualTo(oid2).Should().BeTrue();
			oid1.GetHashCode().Should().Be(oid2.GetHashCode());
		}

		[Fact]
		public void IObjectOIDDoesNotEqualDiffPropertyTest()
		{
			var oid1 = DiscordObjectOID.ForServer(Instance, Server1);
			var oid2 = DiscordObjectOID.ForServer(Instance, Server2);
			(oid1 == oid2).Should().BeFalse();
			(oid1 != oid2).Should().BeTrue();
			oid1.GetHashCode().Should().NotBe(oid2.GetHashCode());
		}

		[Fact]
		public void IObjectOIDDoesNotEqualDiffLengthTest()
		{
			var oid1 = DiscordObjectOID.ForServer(Instance, Server1);
			var oid2 = DiscordObjectOID.ForCategory(Instance, Server1, Category);
			(oid1 == oid2).Should().BeFalse();
			(oid1 != oid2).Should().BeTrue();
			oid1.GetHashCode().Should().NotBe(oid2.GetHashCode());
		}

		[Fact]
		public void IObjectOIDContainsTest()
		{
			List<IObjectOID> oids = [DiscordObjectOID.ForServer(Instance, Server1), DiscordObjectOID.ForServer(Instance, Server2)];
			var oid = DiscordObjectOID.Parse($"{ServiceType.Discord}:{Instance}:{Server1}");
			oids.Should().Contain(oid);
		}


		[Fact]
		public void IObjectOIDLessAndGreaterThanTest()
		{
			var oid1 = DiscordObjectOID.ForServer(Instance, Server1);
			var oid2 = DiscordObjectOID.ForServer(Instance, Server2);
			(oid1 > oid2).Should().BeTrue();
			(oid1 >= oid2).Should().BeTrue();
			(oid1 < oid2).Should().BeFalse();
			(oid1 <= oid2).Should().BeFalse();
			(oid2 > oid1).Should().BeFalse();
			(oid2 >= oid1).Should().BeFalse();
			(oid2 < oid1).Should().BeTrue();
			(oid2 <= oid1).Should().BeTrue();
		}

		[Fact]
		public void IsParentAndChildOfTest()
		{
			var parent = DiscordObjectOID.ForServer(Instance, Server1);
			var child = DiscordObjectOID.ForCategory(Instance, Server1, Category);
			parent.IsParentOf(child).Should().BeTrue();
			parent.IsChildOf(child).Should().BeFalse();
			parent.IsParentOrEqualTo(child).Should().BeTrue();
			parent.IsChildOrEqualTo(child).Should().BeFalse();
			child.IsParentOf(parent).Should().BeFalse();
			child.IsChildOf(parent).Should().BeTrue();
			child.IsParentOrEqualTo(parent).Should().BeFalse();
			child.IsChildOrEqualTo(parent).Should().BeTrue();
		}

		// To my knowledge, it's impossible to make this work without the ?'s, so this will serve as an example of how to do it, I guess.
		[Fact]
		public void IsParentAndChildOfNullTest()
		{
			DiscordObjectOID? parent = null;
			DiscordObjectOID child = DiscordObjectOID.ForServer(Instance, Server1);
			(parent?.IsParentOf(child) ?? child is not null).Should().BeTrue();
			(parent?.IsChildOf(child) ?? false).Should().BeFalse();
			(parent?.IsParentOrEqualTo(child) ?? true).Should().BeTrue();
			(parent?.IsChildOrEqualTo(child) ?? child is null).Should().BeFalse();
			child!.IsParentOf(parent).Should().BeFalse();
			child.IsChildOf(parent).Should().BeTrue();
			child.IsParentOrEqualTo(parent).Should().BeFalse();
			child.IsChildOrEqualTo(parent).Should().BeTrue();
		}
	}
}
