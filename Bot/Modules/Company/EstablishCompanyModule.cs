using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using SpaceCore.Data.Discord;
using SpaceCore.Types;
using SpaceDiscordBot.Services.Discord;
using SpaceDiscordBot.Utilities;

namespace SpaceDiscordBot.Modules.Company
{

    [RegisterModule(ModuleScope.PreEstablishedCompany)]
	internal class EstablishCompanyModule(ServiceBundle services) : BaseModule(services)
	{
		public class EstablishCompanyModal : IModal
		{
			public string Title => "Establish Your New Company";

			[RequiredInput, InputLabel("Company Name")]
			[ModalTextInput("company_name", placeholder: "No Carp Corp", minLength: 1, maxLength: 20)]
			public string CompanyName { get; set; } = "";


			[RequiredInput, InputLabel("Company Shorthand")]
			[ModalTextInput("company_shorthand", placeholder: "CARP", minLength: 1, maxLength: 5)]
			public string CompanyShorthand { get; set; } = "";


			[RequiredInput(false), InputLabel("Company Description")]
			[ModalTextInput("company_description", placeholder: "We take no carp. That's on cod. Yes, for eel", style: TextInputStyle.Paragraph)]
			public string CompanyDescription { get; set; } = "";
		}


		[ModalInteraction("establish_modal")]
		public async Task OnEstablishModalSubmit(EstablishCompanyModal modal)
		{
			ulong userId = Context.User.Id;
			PlayerDiscordData playerDiscordData = await Services.GetPlayerDiscordDataAsync(userId);

			SocketGuild guild = AsSocketGuild();
			ulong guildId = guild.Id;

			CompanyDiscordData companyData = new(guildId)
			{
				CompanyStatus = CompanyStatus.CompleteAndEstablished,

				Name = modal.CompanyName,
				Shorthand = modal.CompanyShorthand,
				Description = modal.CompanyDescription
			};


			ulong ownerId = guild.OwnerId;
			companyData.OwnerId = ownerId;
			companyData.ExecutiveId = ownerId;
			companyData.RegisteredAdmins.Add(ownerId);

			//playerDiscordData.CompanyPermits--;

			await Services.GuildService.CleanAndDeployCompany(guild, companyData);

			//Update the data
			MultiTask multiTask = new();
			multiTask += Services.PlayerDiscordDataService.UpdateAsync(userId, playerDiscordData);
			multiTask += Services.CompanyDiscordDataService.CreateAsync(companyData);


			multiTask += RespondEmbedAsync(EmbedHelper.CreateSuccessEmbed($"Company created! " +
				$"`{modal.CompanyName}` is ready to go! Tell friends, neighbors, family, and pets they are able to play!"));

			await multiTask.WaitAll();
			await Services.ModuleService.UpdateGuildModules(guild.Id);
		}


		[SlashCommand("establish-company", "Get started here. Begins the Server Management process.")]
		public async Task Command()
		{
			var guild = AsSocketGuild();
			var guildId = guild.Id;

			ulong userId = Context.User.Id;

			//Checks if the company can be established and returns a message as an embed to the user.
			CompanyDiscordData? companyDiscordData = await Services.GetCompanyDiscordDataAsync(guildId);

			//Checks for null and truthy at the same time.
			if (companyDiscordData?.IsEstablished() == true)
			{
				await RespondEmbedAsync(
					EmbedHelper.CreateApologistWarning("A company already belongs to this server. Disband it to create a new one here."),
					true);
				return;
			}

			PlayerDiscordData playerDiscordData = await Services.GetPlayerDiscordDataAsync(userId);
			if (playerDiscordData.CompanyPermits < 1)
			{
				await RespondEmbedAsync(
					EmbedHelper.CreateApologistWarning("Not enough permits",
					"You do not have any more company creation permits :(", "No action has been taken."));
				return;
			}

			await RespondWithModalAsync<EstablishCompanyModal>("establish_modal");
		}

	}
}
