namespace AtelierTomato.Markov.Generation
{
	public class MarkovGenerationOptions
	{
		public int maximumOutputLength { get; set; } = 50;
		public int maximumPrevListLength { get; set; } = 10;
		public int maximumMarkovRerolls { get; set; } = 10;
		public int maximumLengthForReroll { get; set; } = 10;
		public double copyPastaKillingProbability { get; set; } = .02;
	}
}
