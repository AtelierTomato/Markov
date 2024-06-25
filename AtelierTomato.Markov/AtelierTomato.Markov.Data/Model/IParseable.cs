using AtelierTomato.Markov.Data.Model.ObjectOID;

namespace AtelierTomato.Markov.Data.Model
{
	public interface IParseable
	{
		public bool CanParse(string input);

		public IObjectOID Parse(string input);
	}

	public class OIDParser(IEnumerable<IParseable> parseables)
	{
		public IObjectOID? Parse(string input) =>
			parseables.FirstOrDefault(parseable => parseable.CanParse(input))
			?.Parse(input) ?? throw new ArgumentException("The ServiceType was not able to be parsed from the given OID.");
	}

	public class InvalidParseable : IParseable
	{
		public bool CanParse(string input) => OIDEscapement.Split(input).First() == ServiceType.Invalid.ToString();

		public IObjectOID Parse(string input) => throw new ArgumentException("The IObjectOID given is of ServiceType Invalid, which is not a valid ServiceType.");
	}

	public class BookParseable : IParseable
	{
		public bool CanParse(string input) => OIDEscapement.Split(input).First() == ServiceType.Book.ToString();

		public IObjectOID Parse(string input) => BookObjectOID.Parse(input);
	}

	public class DiscordParseable : IParseable
	{
		public bool CanParse(string input) => OIDEscapement.Split(input).First() == ServiceType.Discord.ToString();

		public IObjectOID Parse(string input) => DiscordObjectOID.Parse(input);
	}
}
