namespace AtelierTomato.Markov.Core.Generation
{
	public class MarkovGenerationOptions
	{
		public int MaximumOutputLength { get; set; } = 50;
		public int MaximumPrevListLength { get; set; } = 10;
		public int MaximumMarkovRerolls { get; set; } = 10;
		public int MaximumLengthForReroll { get; set; } = 10;
		public double CopyPastaKillingProbability { get; set; } = .02;
	}
}
