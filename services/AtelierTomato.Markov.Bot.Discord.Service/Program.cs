using System.Diagnostics;
using AtelierTomato.Markov.Bot.Discord.Core;
using AtelierTomato.Markov.Bot.Discord.Service;
using AtelierTomato.Markov.Core;
using AtelierTomato.Markov.Core.Cooldown;
using AtelierTomato.Markov.Core.Generation;
using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Model.ObjectOID.Parser;
using AtelierTomato.Markov.Service.Discord;
using AtelierTomato.Markov.Storage;
using AtelierTomato.Markov.Storage.Sqlite;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSystemd();

if (Debugger.IsAttached)
{
	builder.Configuration.AddUserSecrets<Program>();
}

if (Microsoft.Extensions.Hosting.Systemd.SystemdHelpers.IsSystemdService())
{
	builder.Logging.ClearProviders();
	builder.Logging.AddJournal(options => options.SyslogIdentifier = builder.Configuration["SyslogIdentifier"]);
}

builder.Services.AddOptions<DiscordBotOptions>().Bind(builder.Configuration.GetSection("DiscordBot"));
builder.Services.AddOptions<SentenceParserOptions>().Bind(builder.Configuration.GetSection("SentenceParser"));
builder.Services.AddOptions<KeywordOptions>().Bind(builder.Configuration.GetSection("Keyword"));
builder.Services.AddOptions<MarkovChainOptions>().Bind(builder.Configuration.GetSection("MarkovChain"));
builder.Services.AddOptions<DiscordSentenceParserOptions>().Bind(builder.Configuration.GetSection("DiscordSentenceParser"));
builder.Services.AddOptions<SqliteAccessOptions>().Bind(builder.Configuration.GetSection("SqliteAccess"));
builder.Services.AddOptions<CooldownOptions>().Bind(builder.Configuration.GetSection("Cooldown"));

builder.Services.AddHostedService<Worker>();

var discordSocketConfig = new DiscordSocketConfig
{
	// request all unprivileged but unrequest the ones that keep causing log spam
	GatewayIntents = GatewayIntents.AllUnprivileged & ~GatewayIntents.GuildInvites & ~GatewayIntents.GuildScheduledEvents | GatewayIntents.MessageContent,
};
var client = new DiscordSocketClient(config: discordSocketConfig);

builder.Services.AddSingleton(client);

builder.Services
	.AddSingleton<DiscordEventDispatcher>()
	.AddSingleton<DiscordSentenceParser>()
	.AddSingleton<IAuthorAccess, SqliteAuthorAccess>()
	.AddSingleton<IAuthorGroupAccess, SqliteAuthorGroupAccess>()
	.AddSingleton<IAuthorGroupPermissionAccess, SqliteAuthorGroupPermissionAccess>()
	.AddSingleton<IAuthorGroupRequestAccess, SqliteAuthorGroupRequestAccess>()
	.AddSingleton<IAuthorPermissionAccess, SqliteAuthorPermissionAccess>()
	.AddSingleton<IAuthorRetortConfigAccess, SqliteAuthorRetortConfigAccess>()
	.AddSingleton<ILocationAccess, SqliteLocationAccess>()
	.AddSingleton<ILocationGroupAccess, SqliteLocationGroupAccess>()
	.AddSingleton<ILocationGroupPermissionAccess, SqliteLocationGroupPermissionAccess>()
	.AddSingleton<ILocationGroupRequestAccess, SqliteLocationGroupRequestAccess>()
	.AddSingleton<ILocationSettingAccess, SqliteLocationSettingAccess>()
	.AddSingleton<ISentenceAccess, SqliteSentenceAccess>()
	.AddSingleton<IWordStatisticAccess, SqliteWordStatisticAccess>()
	.AddSingleton<MarkovChain>()
	.AddSingleton<KeywordProvider>()
	.AddSingleton<DiscordSentenceRenderer>()
	.AddSingleton<DiscordObjectOIDBuilder>()
	.AddSingleton<DiscordSentenceBuilder>()
	.AddSingleton<AuthorGroupManager>()
	.AddSingleton<LocationGroupManager>()
	.AddSingleton<Cooldown>()
	.AddSingleton(_ => new MultiParser<IObjectOID>([new BookObjectOIDParser(), new SpecialObjectOIDParser(), new DiscordObjectOIDParser()]))
	.AddSingleton(_ => new CommandService(new CommandServiceConfig { DefaultRunMode = RunMode.Async }));

var host = builder.Build();

await host.RunAsync();
