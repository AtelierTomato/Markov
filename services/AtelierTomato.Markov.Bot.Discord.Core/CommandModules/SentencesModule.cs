using System.Text;
using AtelierTomato.Markov.Core.Cooldown;
using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Model.ObjectOID.LocationTypes;
using AtelierTomato.Markov.Service.Discord;
using AtelierTomato.Markov.Storage;
using ConsoleTableExt;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Bot.Discord.Core.CommandModules
{
	public class SentencesModule : InteractionModuleBase<SocketInteractionContext>
	{
		private readonly ISentenceAccess sentenceAccess;
		private readonly IAuthorPermissionAccess authoerPermissionAccess;
		private readonly ILocationAccess locationAccess;
		private readonly IAuthorGroupPermissionAccess authorGroupPermissionAccess;
		private readonly ILocationGroupPermissionAccess locationGroupPermissionAccess;
		private readonly DiscordObjectOIDBuilder objectOIDBuilder;
		private readonly MultiParser<IObjectOID> objectOIDParser;
		private readonly DiscordBotOptions options;
		private readonly Cooldown cooldown;
		private readonly DiscordSocketClient client;
		public SentencesModule(ISentenceAccess sentenceAccess, IAuthorPermissionAccess authoerPermissionAccess, ILocationAccess locationAccess, IAuthorGroupPermissionAccess authorGroupPermissionAccess, ILocationGroupPermissionAccess locationGroupPermissionAccess, DiscordObjectOIDBuilder objectOIDBuilder, MultiParser<IObjectOID> objectOIDParser, IOptions<DiscordBotOptions> options, Cooldown cooldown, DiscordSocketClient client)
		{
			this.sentenceAccess = sentenceAccess;
			this.authoerPermissionAccess = authoerPermissionAccess;
			this.locationAccess = locationAccess;
			this.authorGroupPermissionAccess = authorGroupPermissionAccess;
			this.locationGroupPermissionAccess = locationGroupPermissionAccess;
			this.objectOIDBuilder = objectOIDBuilder;
			this.objectOIDParser = objectOIDParser;
			this.options = options.Value;
			this.cooldown = cooldown;
			this.client = client;
		}

		[SlashCommand("querysentences", "Query sentences based on various parameters.")]
		public async Task QuerySentencesCommand(
			[Summary("authorgroup", "ID for the author group")] string? authorGroup = null,
			[Summary("locationgroup", "ID for the location group")] string? locationGroup = null,
			[Summary("authorfilter", "Triple colon (:::) separated list of authors")] string? authorFilter = null,
			[Summary("locationfilter", "Triple colon (:::) separated list of locations")] string? locationFilter = null,
			[Summary("searchstring", "Text to search for")] string? searchString = null,
			[Summary("count", "Number of sentences to return")] int count = 100
		)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var locationOID = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, locationOID, CooldownType.MessagesCheck))
			{
				await RespondAsync(text: "slow down!!", ephemeral: true);
				return;
			}
			var isDev = options.DeveloperIDs.Contains(Context.User.Id);    // Some guards only need to be checked if not a developer.
			if (!isDev && authorFilter is null && authorGroup is null && locationFilter is null && locationGroup is null)
			{
				await RespondAsync("only devs can query from all locations and all authors!", ephemeral: true);
				return;
			}
			List<AuthorOID> effectiveAuthorFilter = [];
			List<IObjectOID> effectiveLocationFilter = [];
			if (authorGroup is not null)
			{
				if (!ulong.TryParse(authorGroup, out var authorGroupID))
				{
					await RespondAsync("authorgroup was not a valid ulong!", ephemeral: true);
					return;
				}

				if (!isDev)
				{
					var authorGroupPermission = await authorGroupPermissionAccess.ReadAuthorGroupPermission(authorGroupID, authorOID);
					if (authorGroupPermission is null || !authorGroupPermission.Permissions.HasFlag(AuthorGroupPermissionType.UseGroup))
					{
						await RespondAsync("you do not have access to this author group!", ephemeral: true);
						return;
					}
				}

				// Add authors that have sentences in the group
				var authorsFromGroup = (await authorGroupPermissionAccess.ReadAuthorGroupPermissionRangeByID(authorGroupID))
					.Where(agp => agp.Permissions.HasFlag(AuthorGroupPermissionType.SentencesInGroup))
					.Select(agp => agp.Author)
					.ToList();
				if (authorsFromGroup.Count is 0)
				{
					await RespondAsync("no authors were found in the author group...", ephemeral: true);
					return;
				}

				effectiveAuthorFilter = authorsFromGroup;
			}
			bool authorDoesNotHaveUsePermissionForLocationGroup = false;
			if (locationGroup is not null)
			{
				if (!ulong.TryParse(locationGroup, out var locationGroupID))
				{
					await RespondAsync("locationgroup was not a valid ulong!", ephemeral: true);
					return;
				}

				if (!isDev)
				{
					var locationGroupPermission = await locationGroupPermissionAccess.ReadLocationGroupPermission(locationGroupID, locationOID);
					if (locationGroupPermission is null || !locationGroupPermission.Permissions.HasFlag(LocationGroupPermissionType.UseGroup))
					{
						await RespondAsync("this location does not have access to this location group!", ephemeral: true);
						return;
					}

					authorDoesNotHaveUsePermissionForLocationGroup = !(await locationGroupPermissionAccess.ReadLocationGroupPermissionsForOwner(locationGroupID, authorOID)).HasFlag(LocationGroupPermissionType.UseGroup);
					if (locationFilter is null && authorFilter is null && authorGroup is null && authorDoesNotHaveUsePermissionForLocationGroup)
					{
						await RespondAsync("since you do not have permissions in this location group, you cannot query all of its messages.", ephemeral: true);
						return;
					}
				}

				// Add locations that have sentences in the group
				var locationsFromGroup = (await locationGroupPermissionAccess.ReadLocationGroupPermissionRangeByID(locationGroupID))
					.Where(lgp => lgp.Permissions.HasFlag(LocationGroupPermissionType.UseGroup))
					.Select(lgp => lgp.Location)
					.ToList();
				if (locationsFromGroup.Count is 0)
				{
					await RespondAsync("no locations were found in the location group...", ephemeral: true);
					return;
				}

				effectiveLocationFilter = locationsFromGroup;
			}
			if (authorFilter is not null)
			{
				List<AuthorOID> newAuthorsForFilter = [];
				try
				{
					newAuthorsForFilter = authorFilter
						.Split(":::")
						.Select(entry => ulong.TryParse(entry, out _)
							? new AuthorOID(ServiceType.Discord, options.DiscordInstance, entry)
							: AuthorOID.Parse(entry))
						.ToList();
				}
				catch
				{
					await RespondAsync("failed to parse the author filter", ephemeral: true);
					return;
				}
				if (!isDev && authorGroup is null && locationGroup is null && locationFilter is null && newAuthorsForFilter.Any(a => a != authorOID))
				{
					await RespondAsync("you cannot filter by authors that are not you without providing a locationgroup, locationfilter, or authorgroup", ephemeral: true);
					return;
				}

				if (authorGroup is not null && !isDev && locationGroup is not null && locationFilter is not null)
				{
					if (newAuthorsForFilter.Except(effectiveAuthorFilter).Any(a => a != authorOID))
					{
						await RespondAsync("you cannot filter by authors that are not you or not in your group unless there is also a locationgroup of locationfilter specified");
						return;
					}
				}
				effectiveAuthorFilter = effectiveAuthorFilter.Concat(newAuthorsForFilter).ToList();
			}
			if (locationFilter is not null)
			{
				List<IObjectOID> newLocationsForFilter = [];
				try
				{
					newLocationsForFilter = locationFilter
						.Split(":::")
						.Select(entry =>
						{
							if (Enum.TryParse<DiscordLocationType>(entry, true, out var locationDepth))
							{
								var parsedLocation = locationOID.ForLocationType(locationDepth);
								if (parsedLocation is not null)
								{
									return parsedLocation;
								}
								else
								{
									throw new InvalidOperationException();
								}
							}
							else
							{
								return objectOIDParser.Parse(entry);
							}
						})
						.ToList();
				}
				catch
				{
					await RespondAsync("failed to parse the location filter", ephemeral: true);
					return;
				}
				if (!isDev)
				{
					var locations = (await locationAccess.ReadLocationRange(newLocationsForFilter)).ToList();
					var missingLocations = newLocationsForFilter.Where(li => !locations.Any(l => l.ID == li));
					if (missingLocations.Any())
					{
						// Not ephemeral so that they can easily do this contacting
						await RespondAsync("locations without entries in the location database found:\n" +
							$"{missingLocations}\n" +
							"please contact the bot owner in order to fix this.");
						return;
					}
					if (authorGroup is null && authorFilter is null)
					{
						var ownersForLocationsNotInGroup = locations.Where(l => !effectiveLocationFilter.Any(li => li.IsParentOrEqualTo(l.ID))).Select(l => l.Owner).ToList();
						if (ownersForLocationsNotInGroup.Any(lo => lo != authorOID))
						{
							await RespondAsync("you cannot filter by locations that either you do not own or are not in the given locationgroup unless you provide an authorgroup or authorfilter", ephemeral: true);
							return;
						}
					}
					if (authorGroup is null && authorFilter is null && locationGroup is null)
					{
						var locationOwners = locations.Select(l => l.Owner).Distinct().ToList();
						if (locationOwners.Any(lo => lo != authorOID))
						{
							await RespondAsync("you cannot filter by locations that you do not own unless you give an authorgroup, authorfilter, or locationgroup", ephemeral: true);
							return;
						}
					}
				}
				effectiveLocationFilter = effectiveLocationFilter.Concat(newLocationsForFilter).ToList();
			}

			var sentences = await sentenceAccess.ReadSentenceRange(new SentenceFilter(effectiveLocationFilter, effectiveAuthorFilter), searchString, count);
			bool success = await TrySendMessagesToUser(Context.User, sentences);
			if (success)
			{
				await ReplyAsync("sent you the output in DMs!");
			}
			else
			{
				await ReplyAsync("failed to send the output! maybe the file was too large?");
			}
		}

		[SlashCommand("deletesentences", "Delete sentences based on various parameters.")]
		public async Task DeleteSentencesCommand(
			[Summary("authorfilter", "Triple colon (:::) separated list of authors")] string? authorFilter = null,
			[Summary("locationfilter", "Triple colon (:::) separated list of locations")] string? locationFilter = null,
			[Summary("searchstring", "Text to search for")] string? searchString = null
		)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var locationOID = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, locationOID, CooldownType.MessagesCheck))
			{
				await RespondAsync(text: "slow down!!", ephemeral: true);
				return;
			}
			var isDev = options.DeveloperIDs.Contains(Context.User.Id);    // Some guards only need to be checked if not a developer.
			List<AuthorOID> effectiveAuthorFilter = [];
			List<IObjectOID> effectiveLocationFilter = [];
			if (authorFilter is not null)
			{
				try
				{
					effectiveAuthorFilter = authorFilter
						.Split(":::")
						.Select(entry => ulong.TryParse(entry, out _)
							? new AuthorOID(ServiceType.Discord, options.DiscordInstance, entry)
							: AuthorOID.Parse(entry))
						.ToList();
				}
				catch
				{
					await RespondAsync("failed to parse the author filter", ephemeral: true);
					return;
				}
				if (!isDev && locationFilter is null && effectiveAuthorFilter.Any(a => a != authorOID))
				{
					await RespondAsync("you cannot filter by authors for that are not you unless there is also a locationfilter specified");
					return;
				}
			}
			else if (locationFilter is null)
			{
				// If a LocationFilter is not handed to the bot as well, we specify that we are just deleting messages from the user who sent the command.
				effectiveAuthorFilter = [authorOID];
			}
			if (locationFilter is not null)
			{
				try
				{
					effectiveLocationFilter = locationFilter
						.Split(":::")
						.Select(entry =>
						{
							if (Enum.TryParse<DiscordLocationType>(entry, true, out var locationDepth))
							{
								var parsedLocation = locationOID.ForLocationType(locationDepth);
								if (parsedLocation is not null)
								{
									return parsedLocation;
								}
								else
								{
									throw new InvalidOperationException();
								}
							}
							else
							{
								return objectOIDParser.Parse(entry);
							}
						})
						.ToList();
				}
				catch
				{
					await RespondAsync("failed to parse the location filter", ephemeral: true);
					return;
				}
				if (!isDev)
				{
					var locations = (await locationAccess.ReadLocationRange(effectiveLocationFilter)).ToList();

					var missingLocations = effectiveLocationFilter.Where(li => !locations.Any(l => l.ID == li));
					if (missingLocations.Any())
					{
						// Not ephemeral so that they can easily do this contacting
						await RespondAsync("locations without entries in the location database found:\n" +
							$"{missingLocations}\n" +
							"please contact the bot owner in order to fix this.");
						return;
					}
					var ownersForLocations = locations.Select(l => l.Owner).ToList();
					if (ownersForLocations.Any(lo => lo != authorOID))
					{
						if (authorFilter is not null && effectiveAuthorFilter.Any(a => a != authorOID))
						{
							await RespondAsync("you cannot filter by locations that you do not own and authors that are not you at once", ephemeral: true);
							return;
						}
						if (authorFilter is null)
						{
							await RespondAsync("you cannot filter by locations that you do not own unless you provide an authorfilter", ephemeral: true);
							return;
						}
					}
				}
			}

			SentenceFilter filter = new(effectiveLocationFilter, effectiveAuthorFilter);
			var sentences = await sentenceAccess.ReadSentenceRange(filter, searchString);
			var builder = new ComponentBuilder()
				.WithButton("Yes, delete sentences", customId: "confirm_delete", ButtonStyle.Danger)
				.WithButton("Show message", customId: "show_messages", ButtonStyle.Primary)
				.WithButton("Cancel", customId: "cancel_delete", ButtonStyle.Secondary);
			string authorString;
			if (filter.Authors is not null && filter.Authors.Count is not 0)
			{
				authorString = "authors " + string.Join(' ', filter.Authors);
			}
			else
			{
				authorString = "all Authors";
			}
			string locationString;
			if (filter.OIDs is not null && filter.OIDs.Count is not 0)
			{
				locationString = "locations " + string.Join(' ', filter.OIDs);
			}
			else
			{
				locationString = "all Locations";
			}

			if (searchString is not null)
			{
				await RespondAsync(
					text: $"this will delete {sentences.Count()} sentences from {locationString} and {authorString} that match the string \"{searchString}\". " +
					$"are you sure you would like to continue?",
					components: builder.Build(),
					ephemeral: true
				);
			}
			else
			{
				await RespondAsync(
					text: $"this will delete {sentences.Count()} sentences from {locationString} and {authorString}. " +
					$"are you sure you would like to continue?",
					components: builder.Build(),
					ephemeral: true
				);
			}

			var originalResponse = await GetOriginalResponseAsync();

			while (true)
			{
				var interaction = await WaitForButtonAsync(originalResponse.Id, Context.User.Id);

				if (interaction is null)
				{
					await FollowupAsync("no response, cancelled.", ephemeral: true);
					return;
				}

				if (interaction.Data.CustomId == "confirm_delete")
				{
					await interaction.UpdateAsync(msg =>
					{
						msg.Content = "confirmed, deleting messages...";
						msg.Components = new ComponentBuilder().Build();
					});

					await sentenceAccess.DeleteSentenceRange(filter, searchString);

					await FollowupAsync("deletion complete!", ephemeral: true);
					return;
				}
				else if (interaction.Data.CustomId == "cancel_delete")
				{
					await interaction.UpdateAsync(msg =>
					{
						msg.Content = "cancelled.";
						msg.Components = new ComponentBuilder().Build();
					});
					return;
				}
				else if (interaction.Data.CustomId == "show_messages")
				{
					bool success = await TrySendMessagesToUser(Context.User, sentences);

					var followUpBuilder = new ComponentBuilder()
						.WithButton("Yes, delete sentences", customId: "confirm_delete", ButtonStyle.Danger)
						.WithButton("Cancel", customId: "cancel_delete", ButtonStyle.Secondary);

					await interaction.UpdateAsync(msg =>
					{
						msg.Content = success
							? "messages sent in DMs. would you like to delete or cancel?"
							: "failed to send messages, maybe the file was too large? would you like to delete or cancel anyway?";
						msg.Components = followUpBuilder.Build();
					});
				}
			}
		}

		private static async Task<bool> TrySendMessagesToUser(SocketUser user, IEnumerable<Sentence> sentences)
		{
			try
			{
				var listBuilder = ConsoleTableBuilder
					.From(sentences.ToList())
					.WithFormat(ConsoleTableBuilderFormat.Minimal)
					.Export();
				using var stream = new MemoryStream(Encoding.UTF8.GetBytes(listBuilder.ToString()));
				await user.SendFileAsync(stream, "query results.txt");
				return true;
			}
			catch
			{
				return false;
			}
		}

		private async Task<SocketMessageComponent?> WaitForButtonAsync(ulong messageId, ulong userId)
		{
			var tcs = new TaskCompletionSource<SocketMessageComponent>();

			Task Handler(SocketMessageComponent component)
			{
				if (component.Message.Id == messageId && component.User.Id == userId)
				{
					tcs.TrySetResult(component);
				}

				return Task.CompletedTask;
			}

			client.ButtonExecuted += Handler;

			var timeoutTask = Task.Delay(TimeSpan.FromMinutes(3));
			var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

			client.ButtonExecuted -= Handler;

			return completedTask == tcs.Task ? tcs.Task.Result : null;
		}
	}
}
