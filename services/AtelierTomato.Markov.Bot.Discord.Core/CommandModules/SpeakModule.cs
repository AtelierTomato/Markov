using AtelierTomato.Markov.Core;
using AtelierTomato.Markov.Core.Cooldown;
using AtelierTomato.Markov.Core.Generation;
using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Service.Discord;
using Discord.Commands;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Bot.Discord.Core.CommandModules
{
	public class SpeakModule : ModuleBase<SocketCommandContext>
	{
		private readonly DiscordBotOptions options;
		private readonly DiscordObjectOIDBuilder objectOIDBuilder;
		private readonly Cooldown cooldown;
		private readonly MarkovChain markovChain;
		private readonly WebhookHandler webhookHandler;
		private readonly LocationGroupManager locationGroupManager;
		private readonly KeywordProvider keywordProvider;
		private readonly DiscordSentenceRenderer sentenceRenderer;
		private readonly HelpContentBuilder helpContentBuilder;

		public SpeakModule(IOptions<DiscordBotOptions> options, DiscordObjectOIDBuilder objectOIDBuilder, Cooldown cooldown, MarkovChain markovChain, WebhookHandler webhookHandler, LocationGroupManager locationGroupManager, KeywordProvider keywordProvider, DiscordSentenceRenderer sentenceRenderer, HelpContentBuilder helpContentBuilder)
		{
			this.options = options.Value;
			this.objectOIDBuilder = objectOIDBuilder;
			this.cooldown = cooldown;
			this.markovChain = markovChain;
			this.webhookHandler = webhookHandler;
			this.locationGroupManager = locationGroupManager;
			this.keywordProvider = keywordProvider;
			this.sentenceRenderer = sentenceRenderer;
			this.helpContentBuilder = helpContentBuilder;
		}

		[Command("speak")]
		[Alias("s")]
		public async Task Speak([Remainder] string? keywordString = null)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Speak))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}

			string? keyword = null;
			if (keywordString is not null) { keyword = await keywordProvider.Find(keywordString); }
			string sentence;
			if (!Context.IsPrivate)
			{
				// We call with an Invalid author to skip the Author section
				var oidFilter = (await locationGroupManager.GetLocationsForFilter(new AuthorOID(ServiceType.Special, "_", "Invalid"), location)).ToList();
				sentence = await markovChain.Generate(new(oidFilter, []), keyword, null, location);
			}
			else
			{
				sentence = await markovChain.Generate(new([], []), keyword, null, location);
			}
			if (string.IsNullOrEmpty(sentence))
			{
				await ReplyAsync(options.EmptyMarkovReturn);
			}
			else
			{
				await ReplyAsync(sentenceRenderer.Render(sentence, Context.Guild.Emotes, Context.Client.Guilds.SelectMany(g => g.Emotes)));
			}
		}

		[Command("mimic")]
		[Alias("m")]
		public async Task Mimic([Remainder] string? keywordString = null)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Speak))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}

			string? keyword = null;
			if (keywordString is not null) { keyword = await keywordProvider.Find(keywordString); }
			string sentence;
			if (!Context.IsPrivate)
			{
				// We call with an Invalid author to skip the Author section
				var oidFilter = (await locationGroupManager.GetLocationsForFilter(new AuthorOID(ServiceType.Special, "_", "Invalid"), location)).ToList();
				sentence = await markovChain.Generate(new(oidFilter, [authorOID]), keyword, null, location);
			}
			else
			{
				sentence = await markovChain.Generate(new([], [authorOID]), keyword, null, location);
			}
			if (string.IsNullOrEmpty(sentence))
			{
				await ReplyAsync(options.EmptyMarkovReturn);
			}
			else
			{
				var renderedSentence = sentenceRenderer.Render(sentence, Context.Guild.Emotes, Context.Client.Guilds.SelectMany(g => g.Emotes));
				var messageID = await webhookHandler.SendWebhookMessageAsync(renderedSentence, Context, Context.User);
				if (messageID is null)
				{
					await ReplyAsync(renderedSentence);
				}
			}
		}

		[Command("speakstartswith")]
		[Alias("ssw")]
		public async Task SpeakStartsWith(string firstWord)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Speak))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}

			string sentence;
			if (!Context.IsPrivate)
			{
				// We call with an Invalid author to skip the Author section
				var oidFilter = (await locationGroupManager.GetLocationsForFilter(new AuthorOID(ServiceType.Special, "_", "Invalid"), location)).ToList();
				sentence = await markovChain.Generate(new(oidFilter, []), null, firstWord, location);
			}
			else
			{
				sentence = await markovChain.Generate(new([], []), null, firstWord, location);
			}
			if (string.IsNullOrEmpty(sentence))
			{
				await ReplyAsync(options.EmptyMarkovReturn);
			}
			else
			{
				await ReplyAsync(sentenceRenderer.Render(sentence, Context.Guild.Emotes, Context.Client.Guilds.SelectMany(g => g.Emotes)));
			}
		}

		[Command("speakstartswith")]
		[Alias("ssw")]
		public async Task SpeakStartsWith()
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			await ReplyAsync(embed: helpContentBuilder.BuildForSubject(HelpSubject.SpeakStartsWith).Build());
		}

		[Command("speakkeyword")]
		[Alias("sk")]
		public async Task SpeakKeyword(string keyword)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Speak))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}

			string sentence;
			if (!Context.IsPrivate)
			{
				// We call with an Invalid author to skip the Author section
				var oidFilter = (await locationGroupManager.GetLocationsForFilter(new AuthorOID(ServiceType.Special, "_", "Invalid"), location)).ToList();
				sentence = await markovChain.Generate(new(oidFilter, []), keyword, null, location);
			}
			else
			{
				sentence = await markovChain.Generate(new([], []), keyword, null, location);
			}
			if (string.IsNullOrEmpty(sentence))
			{
				await ReplyAsync(options.EmptyMarkovReturn);
			}
			else
			{
				await ReplyAsync(sentenceRenderer.Render(sentence, Context.Guild.Emotes, Context.Client.Guilds.SelectMany(g => g.Emotes)));
			}
		}

		[Command("speakkeyword")]
		[Alias("sk")]
		public async Task SpeakKeyword()
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			await ReplyAsync(embed: helpContentBuilder.BuildForSubject(HelpSubject.SpeakKeyword).Build());
		}
	}
}
