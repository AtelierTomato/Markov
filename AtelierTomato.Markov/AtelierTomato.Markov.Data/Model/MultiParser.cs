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
}
