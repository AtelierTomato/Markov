using AtelierTomato.Markov.Core.Cooldown;
using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Model.ObjectOID;
using AtelierTomato.Markov.Service.Discord;
using Discord;
using Discord.Commands;
using Discord.Webhook;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Bot.Discord.Core
{
	public class WebhookHandler
	{
		private readonly ILogger<WebhookHandler> logger;
		private readonly DiscordBotOptions options;
		private readonly DiscordObjectOIDBuilder objectOIDBuilder;
		private readonly IHttpClientFactory httpClientFactory;
		private readonly Cooldown cooldown;
		private readonly IDiscordClient client;
		public WebhookHandler(ILogger<WebhookHandler> logger, IOptions<DiscordBotOptions> options, DiscordObjectOIDBuilder objectOIDBuilder, IHttpClientFactory httpClientFactory, Cooldown cooldown, IDiscordClient client)
		{
			this.logger = logger;
			this.options = options.Value;
			this.objectOIDBuilder = objectOIDBuilder;
			this.httpClientFactory = httpClientFactory;
			this.cooldown = cooldown;
			this.client = client;
		}

		public async Task<ulong?> SendWebhookMessageAsync(string text, ICommandContext context, IUser user)
		{
			try
			{
				var displayName = user.GlobalName;
				var author = new AuthorOID(ServiceType.Discord, options.DiscordInstance, user.Id.ToString());
				var locationDiscord = DiscordObjectOID.ForInstance(options.DiscordInstance);                                    // download cooldown should be global for a bot, we will assume that only one bot exists per instance
				var locationChannel = await objectOIDBuilder.Build(context.Guild, context.Channel, options.DiscordInstance);    // upload cooldown should be by-channel, as we make webhooks by-channel
				if (cooldown.CheckCooldown(author, locationDiscord, CooldownType.ImageDownload))
				{
					await UpdateLocalAvatar(user);

					cooldown.WriteCooldownTime(author, locationDiscord, CooldownType.ImageDownload);
				}

				var webhook =
					(await context.Guild.GetWebhooksAsync())
						.Where(x =>
							x.Creator.Id == client.CurrentUser.Id &&
							x.ChannelId == context.Channel.Id &&
							x.Name == displayName
						)
						.FirstOrDefault();
				if (webhook is null)
				{
					webhook = await ((SocketTextChannel)context.Channel).CreateWebhookAsync(displayName, GetImage(user).Stream);
					cooldown.WriteCooldownTime(author, locationChannel, CooldownType.ImageUpload);
				}
				if (cooldown.CheckCooldown(author, locationChannel, CooldownType.ImageUpload))
				{
					await webhook.ModifyAsync(x =>
					{
						x.Image = GetImage(user);
					});
				}

				using var webhookClient = new DiscordWebhookClient(webhook);
				return await webhookClient.SendMessageAsync(text);
			}
			catch
			{
				return null;
			}
		}

		private Image GetImage(IUser user) => File.Exists(Path.Join(options.BaseImageStorage, "profile pics", $"{user.Id}.png"))
			? new Image(Path.Join(options.BaseImageStorage, "profile pics", $"{user.Id}.png"))
			: new Image(Path.Join(options.BaseImageStorage, "profile pics", "default.png"));

		private async Task UpdateLocalAvatar(IUser user)
		{
			if (user.GetAvatarUrl(ImageFormat.Png) is not null and var imageUrl)
			{
				try
				{
					using var httpClient = httpClientFactory.CreateClient();
					using var outputFileStream = File.OpenWrite(Path.Join(options.BaseImageStorage, "profile pics", $"{user.Id}.png"));

					var imageStream = await httpClient.GetStreamAsync(new Uri(imageUrl));
					await imageStream.CopyToAsync(outputFileStream);
				}
				catch (HttpRequestException e)
				{
					logger.LogError(e, "Could not download the avatar for user {Username} ({UserId}), URL: {ImageUrl}: HTTP Error: {HttpErrorCode})",
						user.Username,
						user.Id,
						imageUrl,
						e.StatusCode);
				}
			}
		}
	}
}
