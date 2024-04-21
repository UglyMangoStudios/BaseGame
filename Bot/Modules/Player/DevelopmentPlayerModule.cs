using Discord;
using Discord.Interactions;
using SpaceDiscordBot.Services.Discord;
using SpaceDiscordBot.Utilities;
using System.Net;

namespace SpaceDiscordBot.Modules.Player
{
	[RegisterModule(ModuleScope.EstablishedCompany)]
	internal class DevelopmentPlayerModule(ServiceBundle services) : BaseModule(services)
	{

		[SlashCommand("delete-player-data", "DANGEROUS: Wipes a player's data or your own if not specified.")]
		public async Task DeletePlayerDataCommand(IUser? target = null)
		{
			ulong user = target?.Id ?? Context.User.Id;

			var result = await Services.PlayerGameDataService.DeleteAsync(user);

			if (result.Code == HttpStatusCode.NotFound)
			{
				await RespondEmbedAsync(EmbedHelper.CreateApologistWarning("", "The user you selected does not have any data."));
				return;
			}

			result.EnsureSuccess(nameof(DeletePlayerDataCommand));
			await RespondEmbedAsync(EmbedHelper.CreateSuccessEmbed("", "Your game data deletion was successful"));
		}

	}
}
