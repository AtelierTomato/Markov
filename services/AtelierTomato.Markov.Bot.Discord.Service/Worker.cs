using AtelierTomato.Markov.Bot.Discord.Core;

namespace AtelierTomato.Markov.Bot.Discord.Service
{
	public class Worker : IHostedService
	{
		private readonly ILogger<Worker> logger;
		private readonly IConfiguration configuration;
		private readonly DiscordEventDispatcher discordEventDispatcher;
		public Worker(ILogger<Worker> logger, IConfiguration configuration, DiscordEventDispatcher discordEventDispatcher)
		{
			this.logger = logger;
			this.configuration = configuration;
			this.discordEventDispatcher = discordEventDispatcher;
		}

		public async Task StartAsync(CancellationToken stoppingToken)
		{
			this.logger.LogInformation("hello");

			var token = this.configuration["Discord-API-Key"] ?? throw new InvalidOperationException("The discord API key is not set.");

			await this.discordEventDispatcher.LoginAsync(token);
			await this.discordEventDispatcher.StartAsync();

			this.logger.LogInformation("ready to go");
		}

		public async Task StopAsync(CancellationToken stoppingToken)
		{
			await this.discordEventDispatcher.StopAsync();

			this.logger.LogInformation("goodbye");
		}
	}
}
