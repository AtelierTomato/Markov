namespace AtelierTomato.Markov.Core.Generation
{
	public class MarkovChainOptions
	{
		public int MaximumOutputLength { get; set; } = 200;
		public int MaximumPrevListLength { get; set; } = 10;
		public int MaximumMarkovRerolls { get; set; } = 100;
		public int MaximumLengthForReroll { get; set; } = 100;
		public double CopyPastaKillingProbability { get; set; } = .02;
	}
}
