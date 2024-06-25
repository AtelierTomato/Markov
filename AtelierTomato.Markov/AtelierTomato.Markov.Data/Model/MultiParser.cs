using AtelierTomato.Markov.Data.Model.ObjectOID;

namespace AtelierTomato.Markov.Data.Model
{
	public class MultiParser<TOID>(IEnumerable<IParser<TOID>> parsers) where TOID : class
	{
		public TOID Parse(string input) =>
			parsers.FirstOrDefault(parser => parser.CanParse(input))
			?.Parse(input) ?? throw new ArgumentException("The ServiceType was not able to be parsed from the given OID.");
	}

	public interface IParser<TOID>
	{
		public bool CanParse(string input);

		public TOID Parse(string input);
	}

	public class InvalidObjectOIDParser : IParser<IObjectOID>
	{
		public bool CanParse(string input) => OIDEscapement.Split(input).First() == ServiceType.Invalid.ToString();

		public IObjectOID Parse(string input) => throw new ArgumentException("The IObjectOID given is of ServiceType Invalid, which is not a valid ServiceType.");
	}

	public class BookObjectOIDParser : IParser<IObjectOID>
	{
		public bool CanParse(string input) => OIDEscapement.Split(input).First() == ServiceType.Book.ToString();

		public IObjectOID Parse(string input) => BookObjectOID.Parse(input);
	}

	public class DiscordObjectOIDParser : IParser<IObjectOID>
	{
		public bool CanParse(string input) => OIDEscapement.Split(input).First() == ServiceType.Discord.ToString();

		public IObjectOID Parse(string input) => DiscordObjectOID.Parse(input);
	}
}
