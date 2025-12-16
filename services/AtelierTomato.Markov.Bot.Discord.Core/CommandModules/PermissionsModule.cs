using System.Text;
using AtelierTomato.Markov.Core.Cooldown;
using AtelierTomato.Markov.Core.TableFormatters;
using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Model.ObjectOID;
using AtelierTomato.Markov.Model.ObjectOID.LocationTypes;
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
		private readonly AuthorPermissionTableFormatter authorPermissionTableFormatter;
		private readonly HelpContentBuilder helpContentBuilder;
		public PermissionsModule(IAuthorPermissionAccess authorPermissionAccess, IAuthorAccess authorAccess, IOptions<DiscordBotOptions> options, Cooldown cooldown, DiscordObjectOIDBuilder objectOIDBuilder, MultiParser<IObjectOID> objectOIDParser, AuthorPermissionTableFormatter authorPermissionTableFormatter, HelpContentBuilder helpContentBuilder)
		{
			this.authorPermissionAccess = authorPermissionAccess;
			this.authorAccess = authorAccess;
			this.options = options.Value;
			this.cooldown = cooldown;
			this.objectOIDBuilder = objectOIDBuilder;
			this.objectOIDParser = objectOIDParser;
			this.authorPermissionTableFormatter = authorPermissionTableFormatter;
			this.helpContentBuilder = helpContentBuilder;
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
		public async Task OptIn([Remainder] string? _ = null)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}

			await ReplyAsync(embed: helpContentBuilder.BuildForSubject(HelpSubject.OptIn).Build());
			return;
		}

		[Command("optout")]
		[Alias("oo")]
		[Summary("Allows the author to opt out of having their messages gathered by the bot.")]
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
		[Summary("Allows the author to opt out of having their messages gathered by the bot.")]
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
				await ReplyAsync(embed: helpContentBuilder.BuildForSubject(HelpSubject.OptOut).Build());
			}
		}

		[Command("optout")]
		[Alias("oo")]
		[Summary("Allows the author to opt out of having their messages gathered by the bot.")]
		public async Task OptOut()
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			await ReplyAsync(embed: helpContentBuilder.BuildForSubject(HelpSubject.OptOut).Build());
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
				await ReplyAsync(embed: helpContentBuilder.BuildForSubject(HelpSubject.DeletePermission).Build());
			}
		}

		[Command("deletepermission")]
		[Alias("dp")]
		[Summary("Allows the author to delete a permission for having their messages gather by the bot.")]
		public async Task DeletePermission()
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			await ReplyAsync(embed: helpContentBuilder.BuildForSubject(HelpSubject.DeletePermission).Build());
		}

		[Command("permsuser")]
		[Alias("pu", "permissionsuser")]
		[Summary("Checks if the user is opted in and replies showing where and with what permissions.")]
		public async Task PermissionsUser([Remainder] string? _ = null)
		{
			var author = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(author, location, CooldownType.PermissionsCheck))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}

			var authorPermissions = await authorPermissionAccess.ReadAuthorPermissionRangeByAuthor(author);
			if (authorPermissions.Any())
			{
				var message = await authorPermissionTableFormatter.Format(authorPermissions, $"Permissions for Author '{Context.User.GlobalName}'");
				using var stream = new MemoryStream(Encoding.UTF8.GetBytes(message));
				await ReplyAsync(message: "sending you the output in DM!");
				await Context.User.SendFileAsync(stream, Context.User.GlobalName + " permissions.txt");
			}
			else
			{
				await ReplyAsync(message: "you aren't in the database!");
			}
		}

		[Command("permsserver")]
		[Alias("ps", "permserver", "permissionsserver", "permissionserver")]
		[Summary("Checks if the server has any opted in members and replies showing who and with what permissions.")]
		public async Task PermissionsServer([Remainder] string? _ = null)
		{
			if (Context.Channel is IDMChannel)
			{
				await ReplyAsync(message: "you're not in a server right now!");
				return;
			}
			if (!(Context.User.Id == Context.Guild.OwnerId || options.DeveloperIDs.Contains(Context.User.Id)))
			{
				await ReplyAsync(message: "sorry, only the owner of the server is allowed to use this command!");
				return;
			}
			var author = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(author, location, CooldownType.PermissionsCheck))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}

			var authorPermissions = await authorPermissionAccess.ReadAuthorPermissionRangeByBaseLocation(DiscordObjectOID.ForServer(options.DiscordInstance, Context.Guild.Id));
			if (authorPermissions.Any())
			{
				var message = await authorPermissionTableFormatter.Format(authorPermissions, $"Permissions for Location '{Context.Guild.Name}'");
				using var stream = new MemoryStream(Encoding.UTF8.GetBytes(message));
				await ReplyAsync(message: "sending you the output in DM!");
				await Context.User.SendFileAsync(stream, Context.Guild.Name + " permissions.txt");
			}
			else
			{
				await ReplyAsync(message: "there's nobody in the database for this server!");
			}
		}

		[Command("permsall")]
		[Alias("pa", "permissionsall")]
		[Summary("Checks all of the permissions in the entire database, bot developers only!")]
		public async Task PermissionsAll([Remainder] string? _ = null)
		{
			if (!options.DeveloperIDs.Contains(Context.User.Id))
			{
				await ReplyAsync(message: "sorry, only the bot developers are allowed to use this command!");
				return;
			}
			var author = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(author, location, CooldownType.PermissionsCheck))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}

			var authorPermissions = await authorPermissionAccess.ReadAllAuthorPermissions();
			if (authorPermissions.Any())
			{
				var message = await authorPermissionTableFormatter.Format(authorPermissions, $"Permissions for {options.BotName}");
				using var stream = new MemoryStream(Encoding.UTF8.GetBytes(message));
				await ReplyAsync(message: "sending you the output in DM!");
				await Context.User.SendFileAsync(stream, options.BotName + " permissions.txt");
			}
			else
			{
				await ReplyAsync(message: "there's nobody in the database!");
			}
		}
	}
}
