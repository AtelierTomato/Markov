namespace AtelierTomato.Markov.Data.Model
{
	public class MultiParser<TOidCategory>(IEnumerable<IParser<TOidCategory>> parsers) where TOidCategory : class
	{
		public TOidCategory Parse(string input) =>
			parsers.FirstOrDefault(parser => parser.CanParse(input))
			?.Parse(input) ?? throw new ArgumentException($"The MultiParser was not able to find any {typeof(TOidCategory).Name} that it could parse.", nameof(input));
	}

	public interface IParser<out TOidCategory>
	{
		public bool CanParse(string input);

		public TOidCategory Parse(string input);
	}
}
