
using AtelierTomato.Markov.Core;
using AtelierTomato.Markov.Core.Generation;
using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Console
{
	public class Worker : IHostedService
	{
		private readonly ILogger<Worker> logger;
		private readonly MarkovChain markovChain;
		private readonly SentenceRenderer sentenceRenderer;

		public Worker(ILogger<Worker> logger, MarkovChain markovChain, SentenceRenderer sentenceRenderer)
		{
			this.logger = logger;
			this.markovChain = markovChain;
			this.sentenceRenderer = sentenceRenderer;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			this.logger.LogInformation("hello");

			_ = Looper(cancellationToken);
		}

		private async Task Looper(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				System.Console.WriteLine("Enter a command name and press enter.");
				System.Console.WriteLine("Valid commands: generate");
				string? command = System.Console.ReadLine();
				if (command is not null and "generate")
				{
					bool inputAccepted = false;
					int sentencesToGenerate = 0;
					while (!inputAccepted)
					{
						System.Console.WriteLine("Enter amount of sentences to generate, or press enter to use the default (10):");
						string? input = System.Console.ReadLine();
						if (input is null or "")
						{
							sentencesToGenerate = 10;
							inputAccepted = true;
						}
						else if (int.TryParse(input, out sentencesToGenerate))
						{
							inputAccepted = true;
						}
						else
						{
							System.Console.WriteLine("Please enter an integer values.");
						}
					}
					System.Console.WriteLine($"Generating {sentencesToGenerate} sentences...");
					var tasks = new List<Task<string>>();
					for (int i = 0; i < sentencesToGenerate; i++)
					{
						tasks.Add(markovChain.Generate(new SentenceFilter(null, null)));
					}
					List<string> sentences = (await Task.WhenAll(tasks)).Select(sentenceRenderer.Render).ToList();

					for (int i = 0; i < sentences.Count; i++)
					{
						System.Console.WriteLine($"Sentence {i + 1}:");
						System.Console.WriteLine(sentences[i]);
					}
				}
			}
		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			this.logger.LogInformation("goodbye");
		}
	}
}
