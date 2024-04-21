using Discord.Interactions;
using SpaceDiscordBot.Services.Discord;
using SpaceDiscordBot.Utilities;

namespace SpaceDiscordBot.Modules.Guild
{

    [RegisterModule(ModuleScope.Guilds)]
	internal sealed class DocumentationModule(ServiceBundle services) : BaseModule(services)
	{


		[SlashCommand("docs", "Allows you to surf the documentation in Discord!")]
		public async Task DocsCommand()
		{
			await RespondEmbedAsync(EmbedHelper.UnderConstruction(), true);
		}


	}
}
