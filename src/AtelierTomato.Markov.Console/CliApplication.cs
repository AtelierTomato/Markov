
using AtelierTomato.Markov.Console.Modules;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Console
{
	public class CliApplication
	{
		private readonly ILogger<CliApplication> logger;
		private readonly ConsoleOptions options;
		private readonly WorkerManager workerManager;
		private readonly GenerateModule generateModule;
		private readonly List<Task> ongoingTasks = [];
		public CliApplication(ILogger<CliApplication> logger, IOptions<ConsoleOptions> options, WorkerManager workerManager, GenerateModule generateModule)
		{
			this.logger = logger;
			this.options = options.Value;
			this.workerManager = workerManager;
			this.generateModule = generateModule;
		}

		public async Task RunAsync()
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

						int workerID = workerManager.RegisterWorkerID();
						await generateModule.GatherGenerateParameters(workerID);
						ongoingTasks.Add(Task.Run(() => generateModule.GenerateAsync(workerID)));

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
	}
}
