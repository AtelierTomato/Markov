namespace AtelierTomato.Markov.Core.Generation
{
	public class MarkovChainOptions
	{
		public int MaximumOutputLength { get; set; } = 200;
		public int MaximumPrevListLength { get; set; } = 5;
		public int MaximumMarkovRerolls { get; set; } = 5;
		public int MaximumLengthForReroll { get; set; } = 10;
		public double CopyPastaKillingProbability { get; set; } = 0.02;
		public double MarkovChainKillingProbability { get; set; } = 0.01;
	}
}
