using Discord.Interactions;
using SpaceDiscordBot.Frameworks.Attributes;
using SpaceDiscordBot.Services.Discord;

namespace SpaceDiscordBot.Modules.Development
{

    [RegisterModule(ModuleScope.Guilds)]
	[LimitRuntime(Runtime.Development)]
	internal class SampleModule(ServiceBundle services) : BaseModule(services)
	{


		[SlashCommand("sample", "Just a little sample of a command")]
		public async Task Sample(string input)
		{
			await RespondAsync("Sample: " + input);
		}


	}
}
