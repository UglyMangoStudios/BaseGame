using Discord;
using Discord.Interactions;
using SpaceCore.Data.Discord;
using SpaceCore.Types;
using SpaceDiscordBot.Frameworks;
using SpaceDiscordBot.Frameworks.Extensions;
using SpaceDiscordBot.Services.Discord;
using SpaceDiscordBot.Utilities;

namespace SpaceDiscordBot.Modules.Company
{
    [RegisterModule(ModuleScope.EstablishedCompany)]
	internal class DisbandCompanyModule(ServiceBundle services) : BaseModule(services)
	{

		[ComponentInteraction("disband_company_cancel")]
		public async Task CancelButton()
		{
			//When the cancel button is clicked, replace embed with cancelation acknowledgement
			await ModifyOriginalResponseAsync(p =>
			{
				p.Embed = EmbedHelper.BasicEmbed("Operation canceled.").Build();
				p.Components = new ComponentBuilder().Build();
			});
		}


		[ComponentInteraction("disband_company_confirm")]
		public async Task ConfirmButton()
		{
			var guild = AsSocketGuild();
			var companyDiscordData = await Services.GetCompanyDiscordDataAsync(guild.Id);

			MultiTask multiTask = new();
			multiTask += Services.GuildService.CleanCompany(guild, companyDiscordData);
			multiTask += Services.CompanyDiscordDataService.DeleteAsync(guild.Id);

			//Undeploy all users within the guild
			var result = await Services.PlayerDiscordDataService.GetForCompany(guild.Id);

			result.EnsureSuccess(nameof(DisbandCompanyModule), LogSeverity.Critical, "Could not retrieve list of players.");

			foreach (var playerData in result.Value!)
			{
				multiTask.Run(async () =>
				{
					playerData.GuildId = 0;
					playerData.Channels = new();

					var updateResult = await Services.PlayerDiscordDataService.UpdateAsync(playerData.UserId, playerData);
					updateResult.EnsureSuccess(nameof(DisbandCompanyModule) + "/UndeployPlayers");
				});

				multiTask.Run(async () =>
				{
					var user = await Services.GetClient().GetUserAsync(playerData.UserId);
					await user.SendMessageAsync(embed:
						EmbedHelper.CreateEmojiEmotionEmbed(
							EmotionEmoji.BigCri,
							"Your company disbanded!",
							"Don't worry! Your game data is still saved. However, your resource generation and fleet activity is at a standstill.",
							"To resume your district operations, you must join another company.").Build()
							);
				});
			}

			await multiTask.WaitAll();
			await Services.ModuleService.UpdateGuildModules(guild.Id);
		}


		[SlashCommand("disband-company", "If all else fails, delete this company's data.")]
		public async Task DisbandCommand()
		{
			await DeferAsync();

			// TODO: Configure for localization
			//Retrieves proper id references
			ulong userId = Context.User.Id;

			var guild = AsSocketGuild();

			//Get the company data
			CompanyDiscordData companyDiscordData = await Services.EnsureCompanyDiscordDataAsync(guild.Id);

			EmbedBuilder areYouSureEmbed = EmbedHelper.CreateAlertEmbed("You are about to delete this company, forever and always.", "The following with happen:",
				"All of your game related channels WILL be deleted.",
				"Any active player in your company will be unable to play and must find a new company to join.",
				"Any resource generation will come a halt.",
				"And much more.",
				"ARE YOU SURE YOU WANT TO DO THIS?");

			//Create the confirm button
			ButtonBuilder confirmButton = new ButtonBuilder().AttachDelegate(ConfirmButton)
				.WithStyle(ButtonStyle.Danger).WithLabel("Confirm");

			//Create the cancel button
			ButtonBuilder cancelButton = new ButtonBuilder().AttachDelegate(CancelButton)
				.WithStyle(ButtonStyle.Secondary).WithLabel("Cancel");


			//Create and add the components
			ComponentBuilder componentBuilder = new();
			componentBuilder.WithButton(cancelButton).WithButton(confirmButton);

			var message = await ModifyOriginalResponseAsync((message) =>
			{
				message.Embed = areYouSureEmbed.Build();
				message.Components = componentBuilder.Build();
			});
		}

	}
}
