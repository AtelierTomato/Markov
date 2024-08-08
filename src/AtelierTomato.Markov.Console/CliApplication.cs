
using AtelierTomato.Markov.Core;
using AtelierTomato.Markov.Core.Generation;
using AtelierTomato.Markov.Model;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Console
{
	public class CliApplication
	{
		private readonly ILogger<CliApplication> logger;
		private readonly MarkovChain markovChain;
		private readonly SentenceRenderer sentenceRenderer;
		private readonly ConsoleOptions options;
		public CliApplication(ILogger<CliApplication> logger, MarkovChain markovChain, SentenceRenderer sentenceRenderer, IOptions<ConsoleOptions> options)
		{
			this.logger = logger;
			this.markovChain = markovChain;
			this.sentenceRenderer = sentenceRenderer;
			this.options = options.Value;
		}

		public async Task RunAsync()
		{
			logger.LogInformation("Application started.");

			Directory.CreateDirectory(options.OutputFolder);
			System.Console.WriteLine($"Output director successfully created at {options.OutputFolder}");

			while (true)
			{
				System.Console.WriteLine("Enter a command name and press enter.");
				System.Console.WriteLine("Valid commands: generate");
				string? command = System.Console.ReadLine();
				if (command is not null and "generate")
				{
					await Generate();
				}
			}
		}

		private async Task Generate()
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
			string fileName = $"Generate Output - {DateTimeOffset.Now:yyyy-MM-dd_HH-mm-ss}.txt";
			System.Console.WriteLine($"Successfully generated {sentences.Count} sentences, outputting them to {fileName}");
			using StreamWriter writer = new StreamWriter(Path.Combine(options.OutputFolder, fileName));
			for (int i = 0; i < sentences.Count; i++)
			{
				await writer.WriteLineAsync($"Sentence {i + 1}:");
				await writer.WriteLineAsync(sentences[i]);
				await writer.WriteLineAsync();
			}
			System.Console.WriteLine($"Sentences written to {fileName}");
		}
	}
}
