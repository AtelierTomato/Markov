using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Model.ObjectOID;
using AtelierTomato.Markov.Model.ObjectOID.LocationTypes;
using Discord;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Bot.Discord.Core
{
	public class HelpContentBuilder
	{
		private readonly DiscordBotOptions options;
		public HelpContentBuilder(IOptions<DiscordBotOptions> options)
		{
			this.options = options.Value;
		}

		public HelpSubject ParseSubject(string input)
		{
			switch (input)
			{
				case string when input.Equals("general", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("help", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("h", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.General;
				case string when input.Equals("scope", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.Scope;
				case string when input.Equals("location", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("iobjectoid", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("objectoid", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("discordobjectoid", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.Location;
				case string when input.Equals("optin", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("oi", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.OptIn;
				case string when input.Equals("optout", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("oo", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.OptOut;
				case string when input.Equals("permsuser", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("pu", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("permissionsuser", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.PermissionsUser;
				case string when input.Equals("deletepermission", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("dp", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.DeletePermission;
				case string when input.Equals("permsserver", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("ps", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("permserver", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("permissionsserver", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("permissionserver", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.PermissionsServer;
				case string when input.Equals("permsall", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("pa", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("permissionsall", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.PermissionsAll;
				default: return HelpSubject.Unknown;
			}
		}

		public EmbedBuilder BuildForSubject(HelpSubject subject)
		{
			var helpEmbed = subject switch
			{
				HelpSubject.Unknown => throw new ArgumentNullException(nameof(subject)),
				HelpSubject.General => throw new NotImplementedException(), // DON'T FORGET TO DO LATER
				HelpSubject.Scope => new EmbedBuilder
				{
					Title = "Scope Help",
					Description = $"Scopes are used in multiple commands as parameters for {options.BotName}. " +
					"The scopes along with a brief description them can be found below. " +
					"Note that some scopes cannot be used in certain commands.",
					Fields =
					{
						new EmbedFieldBuilder{ IsInline = false, Name = $"**{DiscordLocationType.Global}**", Value = "This scope refers to all places the bot has access to, this can be on or off-platform." },
						new EmbedFieldBuilder{ IsInline = false, Name = $"**{DiscordLocationType.Discord}**", Value = $"This scope refers to all of the current platform you are on, Discord. If Discord ever ends up having other {DiscordLocationType.Instance}s, this will refer to them as well." },
						new EmbedFieldBuilder{ IsInline = false, Name = $"**{DiscordLocationType.Instance}**", Value = $"This scope refers to the specific Instance of Discord that you are on. If Discord ever ends up having other {DiscordLocationType.Instance}s, this will only refer to the one you are on right now. This is essentially the same as the Discord Scope otherwise." },
						new EmbedFieldBuilder{ IsInline = false, Name = $"**{DiscordLocationType.Server}**", Value = "The current Discord Server you are in." },
						new EmbedFieldBuilder{ IsInline = false, Name = $"**{DiscordLocationType.Category}**", Value = "The current Discord Category that you are in." },
						new EmbedFieldBuilder{ IsInline = false, Name = $"**{DiscordLocationType.Channel}**", Value = "The current Discord Channel that you are in. This includes Voice or Text Channels, as well as Forum channels, but not Thread Channels." },
						new EmbedFieldBuilder{ IsInline = false, Name = $"**{DiscordLocationType.Thread}**", Value = "The current Discord Thread you are in. If you are not in a thread, this will refer to the lack of a Thread in a Channel. This is important, as otherwise using Channel will inherently imply Threads under the Channel." },
						new EmbedFieldBuilder{ IsInline = false, Name = $"**{DiscordLocationType.Message}**", Value = "The Discord Message that the bot is currently reading where you executed your command. This is usually not used." },
						new EmbedFieldBuilder{ IsInline = false, Name = $"**{DiscordLocationType.Sentence}**", Value = "The Sentence in the Discord Message that the bot is currently reading. Sentences are separated by standard sentence-ending punctuation, as well as newlines. This is usually not used." }
					}
				},
				HelpSubject.Location => new EmbedBuilder
				{
					Title = "Location Help",
					Description = $"A Location, also known as an {nameof(IObjectOID)}, is how {options.BotName} internally stores locations from different platforms. " +
					$"A Location is made up of pieces that represent increasingly specific places in a hierarchy, for example, a {nameof(DiscordObjectOID)} has a Service (Discord), Instance (discord.com), Server (some server ID), Category (some category ID), et cetera. " +
					$"Some commands accept a Location instead of a Scope (see `{options.BotPrefix}help {HelpSubject.Scope}`) for a parameter. " +
					$"An {nameof(IObjectOID)} can cut off at any section, so some may go as far as Sentence or as near as Server or even Service. " +
					$"This is considered an advanced feature, and, generally, if you can use a Scope instead, you should, as writing a Location by hand is difficult. " +
					$"However, any commands that list out messages, permissions, et cetera, will give you a Location that you can copy and paste into a command." +
					Environment.NewLine + Environment.NewLine +
					$"Again, it is not recommended to freehand a Location, but if you must, here is a rough explanation of how a full {nameof(DiscordObjectOID)} is shaped:" +
					Environment.NewLine +
					$"With Placeholders: Discord:{options.DiscordInstance}:[Server ID]:[Category ID (0 if not in a category)]:[Channel ID]:[Thread ID (0 if not in a thread)]:[Message ID]:[Sentence (this is a number counting from 1 to the amount of sentences in a message)]" +
					Environment.NewLine +
					$"Full Example: Discord:{options.DiscordInstance}:1312182108013465620:1318052764559081514:1318052815092056094:0:1360138310235852951:1" +
					Environment.NewLine + Environment.NewLine +
					"Locations may refer to other platforms, and, in that case, they will not be formatted the same as the examples here.",
					Fields = { new EmbedFieldBuilder { IsInline = false, Name = "**TL;DR**", Value = $"You don't need to know what this is or how it works, just copy and paste whatever {nameof(IObjectOID)}s the bot spits out in list commands." } }
				},
				HelpSubject.OptIn => new EmbedBuilder
				{
					Title = "Opt In Help",
					Description = $"Opting in to message gathering allows {options.BotName} to gather your messages for use in generating markov chain sentences. " +
					$"This is done per a scope (such as `{DiscordLocationType.Server}` or `{DiscordLocationType.Channel}`, see `{options.BotPrefix}help {HelpSubject.Scope}` for more info) and to a scope. " +
					"The lowest Opt In setting will override higher settings, so if you opt into a server and out of a specific channel, only messages outside of that channel will be gathered, vice versa applies." +
					Environment.NewLine + Environment.NewLine +
					$"To opt in, use `{options.BotPrefix}optin` or `{options.BotPrefix}oo` and then two of any Scope in \"Valid Scopes\" (it can be the same Scope), the first being the \"from\" Scope (where the bot will gather messages from) and the second being the \"to\" Scope (where said messages will be available to use).",
					Fields =
					{
						new EmbedFieldBuilder{ IsInline = false, Name = "**Valid Scopes:**", Value = $"`{DiscordLocationType.Global}`, `{DiscordLocationType.Discord}`, `{DiscordLocationType.Instance}`, `{DiscordLocationType.Server}`, `{DiscordLocationType.Category}`, `{DiscordLocationType.Channel}`, `{DiscordLocationType.Thread}`" },
						new EmbedFieldBuilder{ IsInline = false, Name = "**Note:**", Value = "Two parameters are **REQUIRED**, you can no longer opt in with a single parameter." }
					},
					Footer = new EmbedFooterBuilder { Text = $"Example: {options.BotPrefix}optin {DiscordLocationType.Server} {DiscordLocationType.Global}" }
				},
				HelpSubject.OptOut => new EmbedBuilder
				{
					Title = "Opt Out Help",
					Description = $"Opting out of message gathering stops {options.BotName} from gathering your messages for a Scope (see {options.BotPrefix}help {HelpSubject.Scope}), as well as preventing any of your messages from the Scope in the database from being used by the markov generator. " +
					$"However, this does not delete the messages from the database. " +
					$"This is per-Scope or per-Location (see {options.BotPrefix}help {HelpSubject.Location}), and must be done per each Location you want to opt out of. " +
					$"You can use the command with a Location in order to opt out of a Location you no longer have access to." +
					Environment.NewLine + Environment.NewLine +
					$"To opt out, use `{options.BotPrefix}optout` or `{options.BotPrefix}oo` and then any of the below Scopes, or a Location ID.",
					Fields =
					{
						new EmbedFieldBuilder{ IsInline = false, Name = "**Valid Scopes:**", Value = $"`{DiscordLocationType.Global}`, `{DiscordLocationType.Discord}`, `{DiscordLocationType.Instance}`, `{DiscordLocationType.Server}`, `{DiscordLocationType.Category}`, `{DiscordLocationType.Channel}`, `{DiscordLocationType.Thread}`" },
						new EmbedFieldBuilder{ IsInline = false, Name = "**Note:**", Value = "The lowest (most specific) permission will override any higher permissions. " +
						$"This means if you opted in to a {DiscordLocationType.Server}, and then to a {DiscordLocationType.Channel}, and you opt out of the {DiscordLocationType.Server} later, then you will still be opted into the {DiscordLocationType.Channel}."}
					},
					Footer = new EmbedFooterBuilder { Text = $"Example: {options.BotPrefix}optout {DiscordLocationType.Server}" }
				},
				HelpSubject.DeletePermission => new EmbedBuilder
				{
					Title = "Delete Permission Help",
					Description = $"This command completely deletes a permission from {options.BotName} for a Location or a Scope (see `{options.BotPrefix}help {HelpSubject.Location}` and `{options.BotPrefix}help {HelpSubject.Location}`). " +
					$"This is different than opting out, as it acts as if the permission never existed at all. " +
					$"This command is mostly used in cases where you want to simplify the permissions that you have given to the bot, for example, if you opted in to both a Server and a Channel with the same permissions, if you delete the permission for the Channel, instead the bot will use the permissions for the Server for that channel. " +
					$"If you are instead want to explicitly deny permissions for a Location, instead, use `{options.BotPrefix}optout`. " +
					Environment.NewLine + Environment.NewLine +
					$"To delete a permission, use `{options.BotPrefix}deletepermission` or `{options.BotPrefix}dp` and then any of the below Scopes, or a Location ID.",
					Fields =
					{
						new EmbedFieldBuilder{ IsInline = false, Name = "**Valid Scopes:**", Value = $"`{DiscordLocationType.Global}`, `{DiscordLocationType.Discord}`, `{DiscordLocationType.Instance}`, `{DiscordLocationType.Server}`, `{DiscordLocationType.Category}`, `{DiscordLocationType.Channel}`, `{DiscordLocationType.Thread}`" },
					},
					Footer = new EmbedFooterBuilder { Text = $"Example: {options.BotPrefix}dp {DiscordLocationType.Channel}" }
				},
				HelpSubject.PermissionsUser => new EmbedBuilder
				{
					Title = "Permissions User Help",
					Description = "Sends you a message in DMs listing all the locations that you are opted into and to what locations.",
				},
				HelpSubject.PermissionsServer => new EmbedBuilder
				{
					Title = "Permissions Server Help",
					Description = "Only usable by server admins and bot developers. " +
					"Sends you a message in DMs that lists the permissions for all users that are opted into the server that you used the command in.",
				},
				HelpSubject.PermissionsAll => new EmbedBuilder
				{
					Title = "Permissions All Help",
					Description = "Only usable by bot developers. " +
					"Sends you a message in DMs that lists all of the permissions for all users and locations.",
				},
				_ => throw new NotImplementedException()
			};
			helpEmbed.Color = options.EmbedColor;
			return helpEmbed;
		}
	}
}
