using Discord;
using Discord.Interactions;
using SpaceDiscordBot.Services.Discord;
using SpaceDiscordBot.Utilities;

namespace SpaceDiscordBot.Modules.Guild
{
    [RegisterModule(ModuleScope.Guilds)]
	internal sealed class UtilityModule(ServiceBundle services) : BaseModule(services)
	{

		[SlashCommand("ping", "Retrieve this bot's latency")]
		public async Task PingCommand()
		{
			//Gets the bot's latency and concats into a message that is replied to the user issuing the command.
			int latency = Services.GetClient().Latency;

			EmbedBuilder embedBuilder = EmbedHelper.CreateEmbedBuilder(
			"Pong!",
				$"Latency: {latency}",
				(41, 128, 185)
			);

			await RespondEmbedAsync(embedBuilder, true);
		}


	}
}
