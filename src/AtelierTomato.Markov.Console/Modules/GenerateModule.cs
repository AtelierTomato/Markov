using AtelierTomato.Markov.Console.Modules.Parameters;
using AtelierTomato.Markov.Core;
using AtelierTomato.Markov.Core.Generation;
using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Model.ObjectOID.Parser;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Console.Modules
{
	public class GenerateModule
	{
		private readonly ILogger<GenerateModule> logger;
		private readonly WorkerManager workerManager;
		private readonly MarkovChain markovChain;
		private readonly MultiParser<IObjectOID> oidParser = new([new BookObjectOIDParser(), new InvalidObjectOIDParser(), new DiscordObjectOIDParser()]);
		private readonly KeywordProvider keywordProvider;
		private readonly SentenceRenderer sentenceRenderer;
		private readonly ConsoleOptions options;
		private readonly Dictionary<int, GenerateParameter> generationParameters = [];
		private readonly object generationParametersLock = new();
		public GenerateModule(ILogger<GenerateModule> logger, WorkerManager workerManager, MarkovChain markovChain, KeywordProvider keywordProvider, SentenceRenderer sentenceRenderer, IOptions<ConsoleOptions> options)
		{
			this.workerManager = workerManager;
			this.logger = logger;
			this.markovChain = markovChain;
			this.keywordProvider = keywordProvider;
			this.sentenceRenderer = sentenceRenderer;
			this.options = options.Value;
		}

		public async Task GatherGenerateParameters(int workerID)
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
				inputAccepted = false;
				IObjectOID? objectOID = null;
				while (!inputAccepted)
				{
					System.Console.WriteLine("Enter an IObjectOID to filter by, or press enter to leave the value empty:");
					string? input = System.Console.ReadLine();
					if (input is null or "")
					{
						inputAccepted = true;
					}
					else
					{
						try
						{
							objectOID = oidParser.Parse(input);
							inputAccepted = true;
						}
						catch (Exception ex)
						{
							logger.LogError(ex, "Failed to parse valid IObjectOID from \"{Input}\"", input);
							System.Console.WriteLine("Please enter a valid IObjectOID as a string.");
						}
					}

				}
				inputAccepted = false;
				AuthorOID? author = null;
				while (!inputAccepted)
				{
					System.Console.WriteLine("Enter an AuthorOID to filter by, or press enter to leave the value empty:");
					string? input = System.Console.ReadLine();
					if (input is null or "")
					{
						inputAccepted = true;
					}
					else
					{
						try
						{
							author = AuthorOID.Parse(input);
							inputAccepted = true;
						}
						catch (Exception ex)
						{
							logger.LogError(ex, "Failed to parse valid AuthorOID from \"{Input}\"", input);
							System.Console.WriteLine("Please enter a valid AuthorOID as a string.");
						}
					}
				}
				inputAccepted = false;
				string? keyword = null;
				while (!inputAccepted)
				{
					System.Console.WriteLine("Enter a word to use as a keyword, or press enter to leave the value empty, if multiple words are inputted, the KeywordProvider will determine which to use:");
					string? input = System.Console.ReadLine();
					if (input is null or "")
					{
						inputAccepted = true;
					}
					else
					{
						try
						{
							keyword = await keywordProvider.Find(input);
							inputAccepted = true;
						}
						catch (Exception ex)
						{
							logger.LogError(ex, "Failed to find keyword from \"{Input}\"", input);
						}
					}
				}
				inputAccepted = false;
				string? firstWord = null;
				while (!inputAccepted)
				{
					System.Console.WriteLine("Enter a word to use as the firstWord, or press enter to leave the value empty:");
					string? input = System.Console.ReadLine();
					if (input is null or "")
					{
						inputAccepted = true;
					}
					else
					{
						firstWord = input;
						inputAccepted = true;
					}
				}
				GenerateParameter generationParameter = new(sentencesToGenerate, new(objectOID, author), keyword, firstWord);
				lock (generationParametersLock)
				{
					generationParameters[workerID] = generationParameter;
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error occurred while gathering parameters.");
				workerManager.ReleaseWorkerID(workerID);
			}
		}

		public async Task GenerateAsync(int workerID)
		{
			try
			{
				int sentencesToGenerate = generationParameters[workerID].SentencesToGenerate;
				logger.LogInformation("Worker {WorkerID} started generating {SentencesToGenerate} sentences...", workerID, sentencesToGenerate);
				var tasks = new List<Task<string>>();
				var startTime = DateTime.UtcNow;
				for (int i = 0; i < sentencesToGenerate; i++)
				{
					tasks.Add(markovChain.Generate(generationParameters[workerID].filter, generationParameters[workerID].keyword, generationParameters[workerID].firstWord));
				}

				List<string> sentences = (await Task.WhenAll(tasks)).Select(sentenceRenderer.Render).ToList();
				var endTime = DateTime.UtcNow;

				var duration = (endTime - startTime).TotalSeconds;
				var average = duration / sentences.Count;
				logger.LogInformation("Worker {WorkerID} finished generating {SentencesToGenerate} sentences in {Duration} seconds, average: {Average}", workerID, sentencesToGenerate, duration, average);

				string fileName = $"Generate Output - {DateTimeOffset.Now:yyyy-MM-dd_HH-mm-ss} {workerID}.txt";
				logger.LogInformation("Worker {WorkerID} successfully generated {Count} sentences, outputting them to {FileName}...", workerID, sentences.Count, fileName);
				using StreamWriter writer = new StreamWriter(Path.Combine(options.OutputFolder, fileName));
				await writer.WriteLineAsync(fileName);
				await writer.WriteLineAsync($"Time elapsed: {duration}, average: {average}");
				await writer.WriteLineAsync();
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
				workerManager.ReleaseWorkerID(workerID);
			}
		}
	}
}
