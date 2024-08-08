
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
		private readonly object workerLock = new();
		private readonly List<int> availableWorkerIDs = [];
		private readonly List<Task> ongoingTasks = [];
		private readonly Dictionary<int, int> generationParameters = new();
		private readonly object generationParametersLock = new();
		public CliApplication(ILogger<CliApplication> logger, MarkovChain markovChain, SentenceRenderer sentenceRenderer, IOptions<ConsoleOptions> options)
		{
			this.logger = logger;
			this.markovChain = markovChain;
			this.sentenceRenderer = sentenceRenderer;
			this.options = options.Value;

			// Initialize worker IDs
			for (int i = 1; i <= 1000; i++)
			{
				availableWorkerIDs.Add(i);
			}
		}

		public void RunAsync()
		{
			logger.LogInformation("Application started.");

			Directory.CreateDirectory(options.OutputFolder);
			logger.LogInformation("Output directory successfully created at {OutputFolder}", options.OutputFolder);

			while (true)
			{
				string? command;
				System.Console.WriteLine("Enter a command name and press enter.");
				System.Console.WriteLine("Valid commands: generate");
				command = System.Console.ReadLine();
				try
				{
					if (command is "generate")
					{

						int workerID = registerWorkerID();
						GatherGenerateParameters(workerID);
						ongoingTasks.Add(Task.Run(() => GenerateAsync(workerID)));

					}
					else
					{
						System.Console.WriteLine("Unknown command.");
					}

					ongoingTasks.RemoveAll(task => task.IsCompleted);
				}
				catch (Exception ex)
				{
					logger.LogError(ex, "Failed to complete command \"{Command}\"", command);
				}
			}
		}

		private int registerWorkerID()
		{
			int workerID;
			lock (workerLock)
			{
				if (availableWorkerIDs.Count is 0)
				{
					throw new InvalidOperationException("No available worker IDs.");
				}

				// Find the smallest available worker ID
				workerID = availableWorkerIDs.Min();
				availableWorkerIDs.Remove(workerID);
				logger.LogInformation("Worker {WorkerID} assigned to command.", workerID);
			}
			return workerID;
		}

		private void GatherGenerateParameters(int workerID)
		{
			try
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

				lock (generationParametersLock)
				{
					generationParameters[workerID] = sentencesToGenerate;
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error occurred while gathering parameters.");
				lock (workerLock)
				{
					availableWorkerIDs.Add(workerID);
				}
			}
		}

		private async Task GenerateAsync(int workerID)
		{
			try
			{
				int sentencesToGenerate = generationParameters[workerID];
				logger.LogInformation("Worker {WorkerID} started generating {SentencesToGenerate} sentences...", workerID, sentencesToGenerate);
				var tasks = new List<Task<string>>();
				for (int i = 0; i < sentencesToGenerate; i++)
				{
					tasks.Add(markovChain.Generate(new SentenceFilter(null, null)));
				}

				var startTime = DateTime.UtcNow;
				List<string> sentences = (await Task.WhenAll(tasks)).Select(sentenceRenderer.Render).ToList();
				var endTime = DateTime.UtcNow;

				logger.LogInformation("Worker {WorkerID} finished generating {SentencesToGenerate} sentences in {Duration} seconds.", workerID, sentencesToGenerate, (endTime - startTime).TotalSeconds);

				string fileName = $"Generate Output - {DateTimeOffset.Now:yyyy-MM-dd_HH-mm-ss} {workerID}.txt";
				logger.LogInformation("Worker {WorkerID} successfully generated {Count} sentences, outputting them to {FileName}...", workerID, sentences.Count, fileName);
				using StreamWriter writer = new StreamWriter(Path.Combine(options.OutputFolder, fileName));
				for (int i = 0; i < sentences.Count; i++)
				{
					await writer.WriteLineAsync($"Sentence {i + 1}:");
					await writer.WriteLineAsync(sentences[i]);
					await writer.WriteLineAsync();
				}
				logger.LogInformation("Worker {WorkerID} successfully wrote sentences to {FileName}.", workerID, fileName);
			}
			finally
			{
				lock (workerLock)
				{
					availableWorkerIDs.Add(workerID);
				}
			}
		}
	}
}
