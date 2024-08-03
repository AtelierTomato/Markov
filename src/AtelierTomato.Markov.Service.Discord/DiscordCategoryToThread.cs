namespace AtelierTomato.Markov.Service.Discord
{
	internal record DiscordCategoryToThread
	(
		ulong Category,
		ulong Channel,
		ulong? Thread
	);
}
