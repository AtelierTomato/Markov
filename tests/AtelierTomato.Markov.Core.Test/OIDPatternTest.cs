using AtelierTomato.Markov.Core.Model;
using AtelierTomato.Markov.Core.Model.ObjectOID;
using FluentAssertions;
using System.Text.RegularExpressions;

namespace AtelierTomato.Markov.Core.Test
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
			Action act = () => myNewPatternLol = OIDPattern.Generate(["bees"]);
			act.Should().Throw<ArgumentException>().WithMessage("OIDPattern failed to construct as less than 2 fields were given. (Parameter 'fields')");
		}
	}
}
