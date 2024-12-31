using AtelierTomato.Markov.Bot.Discord.Core.CommandModules.ParameterTypes;
using AtelierTomato.Markov.Core.Cooldown;
using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Model.ObjectOID;
using AtelierTomato.Markov.Model.ObjectOID.Types;
using AtelierTomato.Markov.Service.Discord;
using AtelierTomato.Markov.Storage;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Bot.Discord.Core.CommandModules
{
	public class PermissionsModule : ModuleBase<SocketCommandContext>
	{
		private readonly IAuthorPermissionAccess authorPermissionAccess;
		private readonly IAuthorAccess authorAccess;
		private readonly DiscordBotOptions options;
		private readonly Cooldown cooldown;
		private readonly DiscordObjectOIDBuilder objectOIDBuilder;
		private readonly MultiParser<IObjectOID> objectOIDParser;
		public PermissionsModule(IAuthorPermissionAccess authorPermissionAccess, IAuthorAccess authorAccess, IOptions<DiscordBotOptions> options, Cooldown cooldown, DiscordObjectOIDBuilder objectOIDBuilder, MultiParser<IObjectOID> objectOIDParser)
		{
			this.authorPermissionAccess = authorPermissionAccess;
			this.authorAccess = authorAccess;
			this.options = options.Value;
			this.cooldown = cooldown;
			this.objectOIDBuilder = objectOIDBuilder;
			this.objectOIDParser = objectOIDParser;
		}

		[Command("optin")]
		[Alias("oi")]
		[Summary("Lets the author opt in to having their messages gathered by the bot.")]
		public async Task OptIn(DiscordLocationType from, DiscordLocationType to)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			if (from is DiscordLocationType.Message or DiscordLocationType.Sentence || to is DiscordLocationType.Message or DiscordLocationType.Sentence)
			{
				await ReplyAsync("you cannot opt in to or from the scope of an individual message or sentence!");
				return;
			}
			if (Context.Channel is not IThreadChannel && (from is DiscordLocationType.Thread || to is DiscordLocationType.Thread))
			{
				await ReplyAsync("you're not currently in a thread!");
				return;
			}
			var fromOID = location.ForLocationType(from);
			var toOID = location.ForLocationType(to);
			await authorPermissionAccess.WriteAuthorPermission(new AuthorPermission(authorOID, fromOID, toOID));
			var author = new Author(authorOID, Context.User.GlobalName ?? Context.User.Username);
			await authorAccess.WriteAuthor(author);
			await ReplyAsync($"""opted author "{author.Name}" into {options.BotName} from {from} to {to}!""");
		}

		[Command("optin")]
		[Alias("oi")]
		[Summary("Lets the author opt in to having their messages gathered by the bot.")]
		public async Task OptIn([Remainder] string _)
		{
			// TODO: help reply
		}

		[Command("optout")]
		[Alias("oo")]
		[Summary("Allows the author to opt out of having their messages gather by the bot.")]
		public async Task OptOut(DiscordLocationType from)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			if (from is DiscordLocationType.Message or DiscordLocationType.Sentence)
			{
				await ReplyAsync("you cannot opt in from the scope of an individual message or sentence, so there's no need to opt out of one");
				return;
			}
			if (Context.Channel is not IThreadChannel && from is DiscordLocationType.Thread)
			{
				await ReplyAsync("you're not currently in a thread!");
				return;
			}
			var fromOID = location.ForLocationType(from);
			await authorPermissionAccess.WriteAuthorPermission(new AuthorPermission(authorOID, fromOID, new SpecialObjectOID(SpecialObjectOIDType.PermissionDenied)));
			var author = new Author(authorOID, Context.User.GlobalName ?? Context.User.Username);
			await authorAccess.WriteAuthor(author);
			await ReplyAsync($"""opted author "{author.Name}" out of {options.BotName} from {from}...""");
		}

		[Command("optout")]
		[Alias("oo")]
		[Summary("Allows the author to opt out of having their messages gather by the bot.")]
		public async Task OptOut([Remainder] string input)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			try
			{
				var locationForOptingOut = objectOIDParser.Parse(input);
				await authorPermissionAccess.WriteAuthorPermission(new AuthorPermission(authorOID, locationForOptingOut, new SpecialObjectOID(SpecialObjectOIDType.PermissionDenied)));
				var author = new Author(authorOID, Context.User.GlobalName ?? Context.User.Username);
				await authorAccess.WriteAuthor(author);
				await ReplyAsync($"""opted author "{author.Name}" out of {options.BotName} from {input}...""");
			}
			catch
			{
				// TODO: help reply
			}
		}

		[Command("deletepermission")]
		[Alias("dp")]
		[Summary("Allows the author to delete a permission for having their messages gather by the bot.")]
		public async Task DeletePermission(DiscordLocationType from)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			if (from is DiscordLocationType.Message or DiscordLocationType.Sentence)
			{
				await ReplyAsync("you cannot opt in from the scope of an individual message or sentence, so you can't delete a permission here either.");
				return;
			}
			if (Context.Channel is not IThreadChannel && from is DiscordLocationType.Thread)
			{
				await ReplyAsync("you're not currently in a thread!");
				return;
			}
			var fromOID = location.ForLocationType(from);
			await authorPermissionAccess.DeleteAuthorPermission(authorOID, fromOID);
			var author = new Author(authorOID, Context.User.GlobalName ?? Context.User.Username);
			await authorAccess.WriteAuthor(author);
			await ReplyAsync($"""deleted permissions for author "{author.Name}" in {options.BotName} from {from}...""");
		}

		[Command("deletepermission")]
		[Alias("dp")]
		[Summary("Allows the author to delete a permission for having their messages gather by the bot.")]
		public async Task DeletePermission([Remainder] string input)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			try
			{
				var locationForDeleting = objectOIDParser.Parse(input);
				await authorPermissionAccess.DeleteAuthorPermission(authorOID, locationForDeleting);
				var author = new Author(authorOID, Context.User.GlobalName ?? Context.User.Username);
				await authorAccess.WriteAuthor(author);
				await ReplyAsync($"""deleted permissions for author "{author.Name}" in {options.BotName} from {input}...""");
			}
			catch
			{
				// TODO: help reply
			}
		}
	}
}
