using AtelierTomato.Markov.Data.Model;
using AtelierTomato.Markov.Data.Model.ObjectOID;
using FluentAssertions;
using System.Text.RegularExpressions;

namespace AtelierTomato.Markov.Data.Test
{
	public class OIDPatternTest
	{
		[Fact]
		public void OIDPatternGenerateSuccessTest()
		{
			var discordOidPattern = $@"
^
(?<{nameof(ServiceType)}>(?:[^:]|\^:)+)
:
(?<{nameof(DiscordObjectOID.Instance)}>(?:[^:]|\^:)+)
:?
(?<{nameof(DiscordObjectOID.Server)}>(?:[^:]|\^:)+)?
:?
(?<{nameof(DiscordObjectOID.Category)}>(?:[^:]|\^:)+)?
:?
(?<{nameof(DiscordObjectOID.Channel)}>(?:[^:]|\^:)+)?
:?
(?<{nameof(DiscordObjectOID.Thread)}>(?:[^:]|\^:)+)?
:?
(?<{nameof(DiscordObjectOID.Message)}>(?:[^:]|\^:)+)?
:?
(?<{nameof(DiscordObjectOID.Sentence)}>(?:[^:]|\^:)+)?
$";
			Regex writtenDiscordOIDPattern = new Regex(discordOidPattern, RegexOptions.IgnorePatternWhitespace);
			Regex generatedDiscordOIDPattern = OIDPattern.Generate([
				nameof(ServiceType),
				nameof(DiscordObjectOID.Instance),
				nameof(DiscordObjectOID.Server),
				nameof(DiscordObjectOID.Category),
				nameof(DiscordObjectOID.Channel),
				nameof(DiscordObjectOID.Thread),
				nameof(DiscordObjectOID.Message),
				nameof(DiscordObjectOID.Sentence)
			]);
			generatedDiscordOIDPattern.Should().BeEquivalentTo(writtenDiscordOIDPattern);
		}

		[Fact]
		public void OIDPatternGenerateFailTest()
		{
			Regex myNewPatternLol;
			var exception = Assert.Throws<ArgumentException>(() => myNewPatternLol = OIDPattern.Generate(["bees"]));
			Assert.Equal("OIDPattern failed to construct as less than 2 fields were given.", exception.Message);
		}
	}
}
