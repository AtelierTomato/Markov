using System.Reflection;
using AtelierTomato.Markov.Core;
using AtelierTomato.Markov.Core.Generation;
using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Model.ObjectOID;
using AtelierTomato.Markov.Service.Discord;
using AtelierTomato.Markov.Storage;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Bot.Discord.Core
{
	public class DiscordEventDispatcher
	{
		private readonly ILogger<DiscordEventDispatcher> logger;
		private readonly DiscordSocketClient client;
		private readonly DiscordSentenceParser sentenceParser;
		private readonly IWordStatisticAccess wordStatisticAccess;
		private readonly ISentenceAccess sentenceAccess;
		private readonly IAuthorPermissionAccess authorPermissionAccess;
		private readonly IAuthorRetortConfigAccess authorRetortConfigAccess;
		private readonly DiscordBotOptions options;
		private readonly MarkovChain markovChain;
		private readonly KeywordProvider keywordProvider;
		private readonly DiscordSentenceRenderer sentenceRenderer;
		private readonly DiscordSentenceBuilder sentenceBuilder;
		private readonly DiscordObjectOIDBuilder objectOIDBuilder;
		private readonly CommandService commandService;
		private readonly LocationGroupManager locationGroupManager;
		private readonly AuthorGroupManager authorGroupManager;
		private readonly IServiceProvider serviceProvider;
		public DiscordEventDispatcher(ILogger<DiscordEventDispatcher> logger, DiscordSocketClient client, DiscordSentenceParser sentenceParser, IWordStatisticAccess wordStatisticAccess, ISentenceAccess sentenceAccess, IAuthorPermissionAccess authorPermissionAccess, IAuthorRetortConfigAccess authorRetortConfigAccess, IOptions<DiscordBotOptions> options, MarkovChain markovChain, KeywordProvider keywordProvider, DiscordSentenceRenderer sentenceRenderer, DiscordSentenceBuilder sentenceBuilder, DiscordObjectOIDBuilder objectOIDBuilder, LocationGroupManager locationGroupManager, AuthorGroupManager authorGroupManager, CommandService commandService, IServiceProvider serviceProvider)
		{
			this.logger = logger;
			this.client = client;
			this.sentenceParser = sentenceParser;
			this.wordStatisticAccess = wordStatisticAccess;
			this.sentenceAccess = sentenceAccess;
			this.authorPermissionAccess = authorPermissionAccess;
			this.authorRetortConfigAccess = authorRetortConfigAccess;
			this.options = options.Value;
			this.markovChain = markovChain;
			this.keywordProvider = keywordProvider;
			this.sentenceRenderer = sentenceRenderer;
			this.sentenceBuilder = sentenceBuilder;
			this.objectOIDBuilder = objectOIDBuilder;
			this.locationGroupManager = locationGroupManager;
			this.authorGroupManager = authorGroupManager;
			this.commandService = commandService;
			this.serviceProvider = serviceProvider;

			this.client.Log += msg => Task.Run(() => this.Client_Log(msg));
			this.client.Ready += this.Client_Ready;

			this.client.MessageReceived += this.Client_MessageReceived;
			this.client.ReactionAdded += this.Client_ReactionAdded;

			commandService.CommandExecuted += (commandInfo, commandContext, result) => Task.Run(() => this.LogCommandServiceCommandExecuted(commandInfo, commandContext, result));
			commandService.AddModulesAsync(assembly: Assembly.GetAssembly(typeof(DiscordEventDispatcher)), services: serviceProvider);
		}

		private void LogCommandServiceCommandExecuted(Optional<CommandInfo> commandInfo, ICommandContext commandContext, IResult result)
		{
			// todo do more detailed logging here if necessary.
		}

		private static LogLevel MapSeverity(LogSeverity logSeverity) => logSeverity switch
		{
			LogSeverity.Critical => LogLevel.Critical,
			LogSeverity.Error => LogLevel.Error,
			LogSeverity.Warning => LogLevel.Warning,
			LogSeverity.Info => LogLevel.Information,
			LogSeverity.Verbose => LogLevel.Debug,
			LogSeverity.Debug => LogLevel.Trace,
			_ => LogLevel.None,
		};

		private void Client_Log(LogMessage logMessage)
		{
			this.logger.Log(
				logLevel: MapSeverity(logMessage.Severity),
				exception: logMessage.Exception ?? null,
				message: "Discord.NET ({Source}): {DiscordNetMessage}",
				logMessage.Source ?? "unknown",
				logMessage.Message ?? logMessage.Exception?.Message ?? "An error occurred."
			);
		}

		public async Task LoginAsync(string token)
		{
			await this.client.LoginAsync(TokenType.Bot, token);
		}

		public async Task StartAsync()
		{
			await this.client.StartAsync();
		}

		public async Task StopAsync()
		{
			await this.client.StopAsync();
		}

		private async Task Client_Ready()
		{
			await this.client.SetGameAsync(options.ActivityString, type: options.ActivityType);
		}

		private async Task Client_MessageReceived(SocketMessage messageParam)
		{
			// Don't process the message if it was a system message
			if (messageParam is not SocketUserMessage message)
				return;
			// Don't process the message if it was sent by a bot
			if (message.Author.IsBot)
				return;

			var context = new SocketCommandContext(this.client, message);

			// Create a number to track where the prefix ends and the command begins
			int argPos = 0;
			var prefixDetected = message.HasStringPrefix(options.BotPrefix, ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos);

			if (prefixDetected)
			{
				// Execute the command with the command context we just created, along with the service provider for precondition checks.
				using (context.Channel.EnterTypingState()) _ = await commandService.ExecuteAsync(context: context, argPos: argPos, services: serviceProvider);
			}

			_ = await ProcessForGathering(message, context);

			await ProcessForRetorting(message, context);
		}

		private async Task Client_ReactionAdded(Cacheable<IUserMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> originChannel, SocketReaction reaction)
		{
			// Don't process the reaction if it was sent by a bot.
			var reactionUser = reaction.User.GetValueOrDefault();
			if (reactionUser.IsBot)
				return;
			// Do not process if message is not a user message (or null).
			if (await cachedMessage.GetOrDownloadAsync() is not IUserMessage message)
				return;
			// Do not process if the user message is from a bot.
			if (message.Author.IsBot)
				return;

			var context = new CommandContext(client, message);
			var currentEmojis = context.Guild.Emotes;
			var otherAvailableEmojis = client.Guilds.Where(g => g.Id != context.Guild.Id).SelectMany(g => g.Emotes);

			// Set up emojis to check the reactions for. TODO: make this a ... Discord-specific location setting? This is pissing me off.
			IEnumerable<IEmote> writeEmojis = [], deleteEmojis = [], failEmojis = [];
			writeEmojis = options.WriteDiscordEmojiNames.SelectMany(n => ParseEmotesFromName(n, currentEmojis, otherAvailableEmojis))
				.Concat((IEnumerable<IEmote>)options.WriteEmojis.Select(e => new Emoji(e)));
			deleteEmojis = options.DeleteDiscordEmojiNames.SelectMany(n => ParseEmotesFromName(n, currentEmojis, otherAvailableEmojis))
				.Concat((IEnumerable<IEmote>)options.DeleteEmojis.Select(e => new Emoji(e)));
			if (options.FailDiscordEmojiName is not "")
			{
				failEmojis = failEmojis.Append(ParseEmotesFromName(options.FailDiscordEmojiName, currentEmojis, otherAvailableEmojis).First());
			}
			failEmojis = failEmojis.Append(new Emoji(options.FailEmoji));

			var reactionUserIsAllowed =
				reactionUser == message.Author ||
				reactionUser.Id == context.Guild.OwnerId ||
				options.DeveloperIDs.Contains(reactionUser.Id);

			if (!reactionUserIsAllowed)
			{
				// React back on the message with a fail emoji
				if (writeEmojis.Contains(reaction.Emote) || deleteEmojis.Contains(reaction.Emote))
				{
					await message.AddReactionAsync(failEmojis.First());
				}
				return;
			}

			if (writeEmojis.Contains(reaction.Emote))
			{
				if (await ProcessForGathering(message, context))
				{
					await message.AddReactionAsync(reaction.Emote);
				}
				else
				{
					await message.AddReactionAsync(failEmojis.First());
				}
			}
			else if (deleteEmojis.Contains(reaction.Emote))
			{
				// Delete all sentences made from this message from the database
				await ProcessForDeleting(message, context);
				await message.AddReactionAsync(reaction.Emote);
			}
		}

		private static IEnumerable<Emote> ParseEmotesFromName(string n, IEnumerable<Emote> currentEmojis, IEnumerable<Emote> otherAvailableEmojis)
		{
			IEnumerable<Emote> emoji = currentEmojis.Where(e => e.Name == n);
			emoji = emoji.Concat(otherAvailableEmojis.Where(e => e.Name == n));
			if (emoji.Any())
			{
				return emoji;
			}
			else
			{
				throw new InvalidOperationException($"Emoji with name '{n}' not found.");
			}
		}

		private async Task<bool> ProcessForGathering(IUserMessage message, ICommandContext context)
		{
			var location = await GetLocation(context);
			var authorPermission = await authorPermissionAccess.ReadAuthorPermission(new AuthorOID(ServiceType.Discord, options.DiscordInstance, context.User.Id.ToString()), location);
			// Check whether or not we're allowed to gather this message.
			if (authorPermission is null || authorPermission.AllowedScope == new SpecialObjectOID(Model.ObjectOID.Types.SpecialObjectOIDType.Invalid))
				return false;

			// Parse the text of the message, write the words in it to the WordStatistic table, write the sentences into the Sentences table
			IEnumerable<string> messageSentenceTexts = sentenceParser.ParseIntoSentenceTexts(message.Content, message.Tags, message.CreatedAt);
			if (!messageSentenceTexts.Any())
				return false;

			foreach (string text in messageSentenceTexts)
			{
				await wordStatisticAccess.WriteWordStatisticsFromString(text);
			}

			IEnumerable<Sentence> sentences = await sentenceBuilder.Build(context.Guild, context.Channel, message.Id, context.User.Id, message.CreatedAt, messageSentenceTexts, options.DiscordInstance);
			await sentenceAccess.WriteSentenceRange(sentences);

			return true;
		}

		private async Task ProcessForRetorting(IUserMessage message, ICommandContext context)
		{
			var isMention = message.Content.Contains(options.BotName, StringComparison.InvariantCultureIgnoreCase);
			var isReply = message.ReferencedMessage is not null && (message.ReferencedMessage.Author.Id == client.CurrentUser.Id);

			if (!isMention && !isReply)
				return; // no reason to butt in here.

			using (context.Channel.EnterTypingState())
			{
				var location = await GetLocation(context);
				var author = new AuthorOID(ServiceType.Discord, options.DiscordInstance, context.User.Id.ToString());
				// Get the user's retort settings.
				var retortSetting = await authorRetortConfigAccess.ReadAuthorRetortConfig(author, location);
				// Get the effective filter
				var effectiveFilter = new SentenceFilter(
					(await locationGroupManager.GetLocationsForFilter(author, location)).ToList(), // TODO: maybe combine these functions? I can't foresee any instance where we're not doing both
					(await authorGroupManager.GetAuthorsForFilter(author, location)).ToList()
				);
				var responseText = await markovChain.Generate(effectiveFilter, retortSetting?.Keyword ?? await keywordProvider.Find(context.Message.Content), retortSetting?.FirstWord, location);
				var responseSentence = sentenceRenderer.Render(responseText, context.Guild.Emotes, client.Guilds.SelectMany(g => g.Emotes));
				if (!string.IsNullOrEmpty(responseSentence))
				{
					if (retortSetting is not null && retortSetting.DisplayOption is DisplayOptionType.Mimic)
					{
						// TODO: mimic stuff, do later, hate mimic stuff so bad
						await context.Channel.SendMessageAsync(responseSentence);
					}
					else
					{
						await context.Channel.SendMessageAsync(responseSentence);
					}
				}
				else
				{
					await context.Channel.SendMessageAsync(options.EmptyMarkovReturn);
				}
			}
		}

		private async Task ProcessForDeleting(IUserMessage message, ICommandContext context)
		{
			var oid = (await GetLocation(context)).WithMessage(message.Id);
			await sentenceAccess.DeleteSentenceRange(new SentenceFilter([oid], []), null);
		}

		private async Task<DiscordObjectOID> GetLocation(ICommandContext context)
		{
			DiscordObjectOID location;
			if (context.Channel is IGuildChannel guildChannel)
			{
				location = await objectOIDBuilder.Build(context.Guild, guildChannel, options.DiscordInstance);
			}
			else
			{
				location = DiscordObjectOID.ForChannel(options.DiscordInstance, 0, 0, context.Channel.Id);
			}

			return location;
		}
	}
}
