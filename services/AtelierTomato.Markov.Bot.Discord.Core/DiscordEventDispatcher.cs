using System.Diagnostics;
using System.Reflection;
using AtelierTomato.Markov.Core;
using AtelierTomato.Markov.Core.Generation;
using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Model.ObjectOID;
using AtelierTomato.Markov.Model.ObjectOID.Types;
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
		private readonly ILocationGroupPermissionAccess locationGroupPermissionAccess;
		private readonly ILocationGroupRequestAccess locationGroupRequestAccess;
		private readonly ILocationAccess locationAccess;
		private readonly ILocationSettingAccess locationSettingAccess;
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
		public DiscordEventDispatcher(ILogger<DiscordEventDispatcher> logger, DiscordSocketClient client, DiscordSentenceParser sentenceParser, IWordStatisticAccess wordStatisticAccess, ISentenceAccess sentenceAccess, IAuthorPermissionAccess authorPermissionAccess, IAuthorRetortConfigAccess authorRetortConfigAccess, ILocationAccess locationAccess, ILocationGroupPermissionAccess locationGroupPermissionAccess, ILocationGroupRequestAccess locationGroupRequestAccess, ILocationSettingAccess locationSettingAccess, IOptions<DiscordBotOptions> options, MarkovChain markovChain, KeywordProvider keywordProvider, DiscordSentenceRenderer sentenceRenderer, DiscordSentenceBuilder sentenceBuilder, DiscordObjectOIDBuilder objectOIDBuilder, LocationGroupManager locationGroupManager, AuthorGroupManager authorGroupManager, CommandService commandService, IServiceProvider serviceProvider)
		{
			this.logger = logger;
			this.client = client;
			this.sentenceParser = sentenceParser;
			this.wordStatisticAccess = wordStatisticAccess;
			this.sentenceAccess = sentenceAccess;
			this.authorPermissionAccess = authorPermissionAccess;
			this.authorRetortConfigAccess = authorRetortConfigAccess;
			this.locationAccess = locationAccess;
			this.locationGroupPermissionAccess = locationGroupPermissionAccess;
			this.locationGroupRequestAccess = locationGroupRequestAccess;
			this.locationSettingAccess = locationSettingAccess;
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
			this.client.GuildUpdated += this.Client_GuildUpdated;
			this.client.ChannelUpdated += this.Client_ChannelUpdated;
			this.client.ChannelCreated += this.Client_ChannelCreated;
			this.client.RoleUpdated += this.Client_RoleUpdated;
			this.client.GuildMemberUpdated += this.Client_GuildMemberUpdated;
			this.client.ThreadCreated += this.Client_ThreadCreated;
			this.client.ThreadUpdated += this.Client_ThreadUpdated;

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
			// In order to prevent ratelimiting, do not sync locations in the testing environment where we are frequently stopping and starting the bot.
			if (!Debugger.IsAttached)
			{
				await SyncLocationsAsync();
			}

			// Register the Service and Instance name in the bot in order to ensure they output correctly in tables. The owner is more or less a placeholder that doesn't matter.
			var firstDeveloper = new AuthorOID(ServiceType.Discord, options.DiscordInstance, options.DeveloperIDs.First().ToString());
			await locationAccess.WriteLocationRange([
				new Location(DiscordObjectOID.ForService(), ServiceType.Discord.ToString(), firstDeveloper),
				new Location(DiscordObjectOID.ForInstance(options.DiscordInstance), options.DiscordInstance, firstDeveloper)
			]);

			// Register slash commands
			await RegisterCommandsAsync();
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

		public async Task Client_GuildUpdated(SocketGuild oldGuild, SocketGuild newGuild)
		{
			if (oldGuild.Name == newGuild.Name && oldGuild.OwnerId == newGuild.OwnerId)
				return; // we don't care

			var locationOID = DiscordObjectOID.ForServer(options.DiscordInstance, oldGuild.Id);
			if (oldGuild.Name != newGuild.Name)
			{
				var location = await locationAccess.ReadLocation(locationOID);
				if (location is null)
					logger.LogWarning("Server with ID '{ID}' expected to have value in database, however, no value was found.", locationOID);
				var updatedLocation = new Location(locationOID, newGuild.Name, location?.Owner ?? new AuthorOID(ServiceType.Discord, options.DiscordInstance, newGuild.OwnerId.ToString()));
				await locationAccess.WriteLocation(updatedLocation);

				logger.LogInformation("Server with ID '{ID}' name updated: {OldName} -> {NewName}", oldGuild.Id, oldGuild.Name, newGuild.Name);
			}

			if (oldGuild.OwnerId != newGuild.OwnerId)
			{
				var locations = await locationAccess.ReadLocationRangeByBaseLocation(locationOID);
				var newOwner = new AuthorOID(ServiceType.Discord, options.DiscordInstance, newGuild.OwnerId.ToString());
				var updatedLocations = locations.Select(l => new Location(l.ID, l.Name, newOwner));
				await locationAccess.WriteLocationRange(updatedLocations);

				logger.LogInformation("Server with ID '{ID}' owner updated: {OldOwner} -> {NewOwner}", oldGuild.Id, oldGuild.OwnerId, newGuild.OwnerId);
			}
		}

		public async Task Client_ChannelUpdated(SocketChannel oldChannel, SocketChannel newChannel)
		{
			if (oldChannel is SocketGuildChannel oldGuildChannel && newChannel is SocketGuildChannel newGuildChannel)
			{
				// We can't be sure whether we saw the channel before because of permissions, so we update the channel always.
				await UpdateChannel(newGuildChannel);
				if (oldGuildChannel.Name != newGuildChannel.Name)
				{
					logger.LogInformation("Channel with ID '{ID}' name updated: {OldName} -> {NewName}", oldGuildChannel.Id, oldGuildChannel.Name, newGuildChannel.Name);
				}
				else
				{
					logger.LogInformation("Channel with ID '{ID}' updated in database", oldGuildChannel.Id);
				}
				if (oldGuildChannel is INestedChannel oldNestedChannel && newGuildChannel is INestedChannel newNestedChannel)
				{
					if (oldNestedChannel.CategoryId != newNestedChannel.CategoryId)
					{
						logger.LogInformation("The category of a channel with ID '{ID}' changed, we will update AuthorPermissions, AuthorRetortConfigs, Locations, " +
							"LocationGroupPermissions and Requests, and LocationSettings. All Sentences will remain the same, and existing of aforementioned will be kept " +
							"in order to ensure that those Sentences are still usable.", newNestedChannel.Id);
						var oldLocation = await objectOIDBuilder.Build(oldNestedChannel.Guild, oldNestedChannel, options.DiscordInstance);
						var newCategory = newNestedChannel.CategoryId ?? 0;

						// AuthorPermissions
						var oldAuthorPermissions = await authorPermissionAccess.ReadAuthorPermissionRangeByBaseLocation(oldLocation);
						// Update category only where we need to
						var newAuthorPermissions = oldAuthorPermissions.Select(authorPermission => new AuthorPermission(
							authorPermission.Author,
							authorPermission.QueryScope?.IsChildOrEqualTo(oldLocation) ?? false
								? ((DiscordObjectOID)authorPermission.QueryScope!).UpdateCategory(newCategory)
								: authorPermission.QueryScope,
							authorPermission.AllowedScope?.IsChildOrEqualTo(oldLocation) ?? false
								? ((DiscordObjectOID)authorPermission.AllowedScope!).UpdateCategory(newCategory)
								: authorPermission.AllowedScope));
						await authorPermissionAccess.WriteAuthorPermissionRange(newAuthorPermissions);

						// AuthorRetortConfigs
						var oldAuthorRetortConfigs = await authorRetortConfigAccess.ReadAuthorRetortConfigRangeByBaseLocation(oldLocation);
						var newAuthorRetortConfigs = oldAuthorRetortConfigs.Select(a => new AuthorRetortConfig(
							a.Author,
							((DiscordObjectOID)a.Location!).UpdateCategory(newCategory),
							a.DisplayOption,
							a.Filter,
							a.AuthorGroup,
							a.LocationGroup,
							a.Keyword,
							a.FirstWord));
						await authorRetortConfigAccess.WriteAuthorRetortConfigRange(newAuthorRetortConfigs);

						// Locations
						var oldLocations = await locationAccess.ReadLocationRangeByBaseLocation(oldLocation);
						var newLocations = oldLocations.Select(l => new Location(
							((DiscordObjectOID)l.ID).UpdateCategory(newCategory),
							l.Name,
							l.Owner));
						await locationAccess.WriteLocationRange(newLocations);

						// LocationGroupPermissions
						var oldLocationGroupPermissions = await locationGroupPermissionAccess.ReadLocationGroupPermissionRangeByBaseLocation(oldLocation);
						var newLocationGroupPermissions = oldLocationGroupPermissions.Select(l => new LocationGroupPermission(
							l.ID,
							((DiscordObjectOID)l.Location).UpdateCategory(newCategory),
							l.Permissions));
						await locationGroupPermissionAccess.WriteLocationGroupPermissionRange(newLocationGroupPermissions);

						// LocationGroupRequests
						var oldLocationGroupRequests = await locationGroupRequestAccess.ReadLocationGroupRequestRangeByBaseLocation(oldLocation);
						var newLocationGroupRequests = oldLocationGroupRequests.Select(l => new LocationGroupPermission(
							l.ID,
							((DiscordObjectOID)l.Location).UpdateCategory(newCategory),
							l.Permissions));
						await locationGroupRequestAccess.WriteLocationGroupRequestRange(newLocationGroupRequests);

						// LocationSettings
						var oldLocationSettings = await locationSettingAccess.ReadLocationSettingRangeByBaseLocation(oldLocation);
						var newLocationSettings = oldLocationSettings.Select(l => new LocationSetting(
							((DiscordObjectOID)l.ID).UpdateCategory(newCategory),
							l.WriteReactions,
							l.DeleteReactions,
							l.FailReactions,
							l.GlobalAllowed,
							l.LocationGroup));
						await locationSettingAccess.WriteLocationSettingRange(newLocationSettings);

						logger.LogInformation("Wrote {APCount} AuthorPermissions, {ARCCount} AuthorRetortConfigs, {LCount} Locations, {LGPCount} LocationGroupPermissions, " +
							"{LGRCount} LocationGroupRequests, and {LSCount} LocationSettings with update Category.", newAuthorPermissions.Count(), newAuthorRetortConfigs.Count(),
							newLocations.Count(), newLocationGroupPermissions.Count(), newLocationGroupRequests.Count(), newLocationSettings.Count());
					}
				}
			}
		}

		public async Task Client_ChannelCreated(SocketChannel channel)
		{
			if (channel is SocketGuildChannel guildChannel)
			{
				await UpdateChannel(guildChannel);
				logger.LogInformation("Channel with ID '{ID}' created, added to database", guildChannel.Id);
			}
		}

		public async Task Client_ThreadCreated(SocketThreadChannel threadChannel)
		{
			await UpdateChannel(threadChannel);
			logger.LogInformation("Channel with ID '{ID}' created, added to database", threadChannel.Id);
		}

		public async Task Client_ThreadUpdated(Cacheable<SocketThreadChannel, ulong> oldThreadChannel, SocketThreadChannel newThreadChannel)
		{
			await UpdateChannel(newThreadChannel);
			if (oldThreadChannel.Value is null)
			{
				logger.LogInformation("Thread with ID '{ID}' became active after being inactive, updated in database.", newThreadChannel.Id);
			}
			else if (oldThreadChannel.Value.Name != newThreadChannel.Name)
			{
				logger.LogInformation("Thread with ID '{ID}' name updated: {OldName} -> {NewName}", newThreadChannel.Id, oldThreadChannel.Value.Name ?? "unknown name", newThreadChannel.Name);
			}
			else
			{
				logger.LogInformation("Thread with ID '{ID}' updated in database", newThreadChannel.Id);
			}
		}

		public async Task Client_RoleUpdated(SocketRole oldRole, SocketRole newRole)
		{
			if (newRole.Guild.CurrentUser.Roles.Contains(newRole))
			{
				var guildChannels = newRole.Guild.Channels.ToList();
				await UpdateChannels(guildChannels);
				logger.LogInformation("A role the bot has in server with ID '{Server}' was updated, just in case we have access to additional channels, we updated {Count} Locations.", newRole.Guild.Id, guildChannels.Count);
			}
		}

		public async Task Client_GuildMemberUpdated(Cacheable<SocketGuildUser, ulong> oldUser, SocketGuildUser newUser)
		{
			if (newUser.Guild.CurrentUser.Id == newUser.Id && oldUser.Value.Roles != newUser.Roles)
			{
				var guildChannels = newUser.Guild.Channels.ToList();
				await UpdateChannels(guildChannels);
				logger.LogInformation("The bot's roles were updated in server with ID '{Server}', just in case we have access to additional channels, we updated {Count} Locations.", newUser.Guild.Id, guildChannels.Count);
			}
		}

		private async Task UpdateChannel(SocketGuildChannel guildChannel) => await UpdateChannels([guildChannel]);
		private async Task UpdateChannels(IList<SocketGuildChannel> guildChannels)
		{
			var locationOIDs = await Task.WhenAll(guildChannels.Select(async g => await objectOIDBuilder.Build(g.Guild, g, options.DiscordInstance)));
			List<Location> updatedLocations = [];
			for (int i = 0; i < guildChannels.Count; i++)
			{
				updatedLocations.Add(new Location(locationOIDs[i], guildChannels[i].Name, new AuthorOID(ServiceType.Discord, options.DiscordInstance, guildChannels[i].Guild.OwnerId.ToString())));
			}
			await locationAccess.WriteLocationRange(updatedLocations);
		}

		public async Task SyncLocationsAsync()
		{
			IEnumerable<Location> locations = client.Guilds.Select(g => new Location(DiscordObjectOID.ForServer(options.DiscordInstance, g.Id), g.Name, new AuthorOID(ServiceType.Discord, options.DiscordInstance, g.OwnerId.ToString())));
			foreach (var guild in client.Guilds)
			{
				var owner = locations.Where(l => ((DiscordObjectOID)l.ID).Server == guild.Id).FirstOrDefault()!.Owner;
				locations = locations.Concat(await Task.WhenAll(guild.Channels.Select(async c => new Location(await objectOIDBuilder.Build(guild, c, options.DiscordInstance), c.Name, owner))));
			}
			await locationAccess.WriteLocationRange(locations);
			logger.LogInformation("Wrote {Number} locations to the database from currently accessible locations.", locations.Count());
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
			var location = await objectOIDBuilder.Build(context.Guild, context.Channel, options.DiscordInstance);
			var authorPermission = await authorPermissionAccess.ReadAuthorPermission(new AuthorOID(ServiceType.Discord, options.DiscordInstance, context.User.Id.ToString()), location);
			// Check whether or not we're allowed to gather this message.
			if (authorPermission is null || authorPermission.AllowedScope is SpecialObjectOID { Type: SpecialObjectOIDType.PermissionDenied })
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
				var location = await objectOIDBuilder.Build(context.Guild, context.Channel, options.DiscordInstance);
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
			var oid = (await objectOIDBuilder.Build(context.Guild, context.Channel, options.DiscordInstance)).WithMessage(message.Id);
			await sentenceAccess.DeleteSentenceRange(new SentenceFilter([oid], []), null);
		}

		private async Task RegisterCommandsAsync()
		{
			List<SlashCommandBuilder> commandBuilders = [
				new SlashCommandBuilder()
					.WithName("querysentences")
					.WithDescription("Query sentences based on various parameters.")
					.AddOption("authorgroup", ApplicationCommandOptionType.String, "GUID for the author group", isRequired: false)
					.AddOption("locationgroup", ApplicationCommandOptionType.String, "GUID for the location group", isRequired: false)
					.AddOption("authorfilter", ApplicationCommandOptionType.String, "Double colon (::) separated list of authors", isRequired: false)
					.AddOption("locationfilter", ApplicationCommandOptionType.String, "Double colon (::) separated list of locations", isRequired: false)
					.AddOption("searchstring", ApplicationCommandOptionType.String, "Text to search for", isRequired: false)
					.AddOption("count", ApplicationCommandOptionType.Integer, "Number of sentences to return", isRequired: false),
			];

			foreach (var commandBuilder in commandBuilders)
			{
				var command = commandBuilder.Build();
				try
				{
					foreach (var guild in client.Guilds)
					{
						await client.Rest.CreateGuildCommand(command, guild.Id);
					}

					logger.LogInformation("Slash command '{Command}' registered for all connected guilds.", command.Name);
				}
				catch (Exception ex)
				{
					logger.LogError(ex, "Failed to register slash command '{Command}'.", command.Name);
				}
			}
		}
	}
}
