using System.Diagnostics;
using AtelierTomato.Markov.Console;
using AtelierTomato.Markov.Core;
using AtelierTomato.Markov.Core.Generation;
using AtelierTomato.Markov.Storage;
using AtelierTomato.Markov.Storage.Sqlite;

IHost host = Host.CreateDefaultBuilder(args)
	.UseSystemd()
	.ConfigureAppConfiguration((hostContext, builder) =>
	{
		// Add other providers for JSON, etc.

		// only use user secrets when debugging.
		if (Debugger.IsAttached)
		{
			builder.AddUserSecrets<Program>();
		}
	})
	.ConfigureLogging((hostContext, builder) =>
	{
		// if we're in fact using systemd, throw out the default console logger and only use the systemd journal
		if (Microsoft.Extensions.Hosting.Systemd.SystemdHelpers.IsSystemdService())
		{
			builder.ClearProviders();
			builder.AddJournal(options => options.SyslogIdentifier = hostContext.Configuration["SyslogIdentifier"]);
		}
	})
	.ConfigureServices((hostContext, services) =>
	{
		services.AddHostedService<Worker>();

		services.AddSingleton<ISentenceAccess, SqliteSentenceAccess>()
				.AddSingleton<IWordStatisticAccess, SqliteWordStatisticAccess>()
				.AddSingleton<MarkovChain>()
				.AddSingleton<KeywordProvider>()
				.AddSingleton<SentenceRenderer>();
		services.AddOptions<SqliteAccessOptions>()
				.Bind(hostContext.Configuration.GetSection("SqliteAccess"));
		services.AddOptions<MarkovChainOptions>()
				.Bind(hostContext.Configuration.GetSection("MarkovChain"));
		services.AddOptions<KeywordOptions>()
				.Bind(hostContext.Configuration.GetSection("Keyword"));
		services.AddOptions<ConsoleOptions>()
				.Bind(hostContext.Configuration.GetSection("Console"));
	})
	.Build();

await host.RunAsync();
