using Discord.Interactions;
using Discord.WebSocket;
using SpaceCore.Data.Discord;
using SpaceCore.Types;
using SpaceDiscordBot.Frameworks;
using SpaceDiscordBot.Services.Discord;
using SpaceDiscordBot.Utilities;

namespace SpaceDiscordBot.Modules.Player.CompanyManagement
{

	[RegisterModule(ModuleScope.EstablishedCompany)]
	internal class DeployModule(ServiceBundle services) : BaseModule(services)
	{


		[SlashCommand("deploy", "Move your district to this company's jurisdiction.")]
		public async Task Command()
		{
			//Defer the message to allow future modification and 15 minutes to perform background operations
			await DeferAsync(ephemeral: true);

			//Get the company data corresponding to the guild they are in
			var guild = AsSocketGuild();
			CompanyDiscordData companyDiscordData = await Services.EnsureCompanyDiscordDataAsync(guild.Id);

			//Forces the user to be a guild user
			SocketGuildUser user = EnsureGuildUser(Context.User);

			//Gets the user's player data
			PlayerDiscordData playerDiscordData = await Services.GetPlayerDiscordDataAsync(user.Id);

			//If the user already is a part of another company
			if (playerDiscordData.GuildId != 0)
			{
				var warning = EmbedHelper.CreateApologistWarning("",
					"Our data is showing you already in a jurisdiction of another company. " +
					"In order to deploy here, you must undeploy in your other company. ").Build();
				await ModifyOriginalResponseAsync(m => m.Embed = warning);
				return;
			}

			//Deploy the player to the guild
			await Services.GuildService.DeployPlayerToCompany(guild, user, companyDiscordData, playerDiscordData);

			MultiTask multiTask = new();

			//Save the new data 
			multiTask += Services.PlayerDiscordDataService.UpdateAsync(user.Id, playerDiscordData);
			multiTask += Services.CompanyDiscordDataService.UpdateAsync(guild.Id, companyDiscordData);

			multiTask.Run(async () => await ModifyOriginalResponseAsync(m =>
			{
				m.Embed = EmbedHelper.CreateSuccessEmbed("Deployment is a go!",
						"You are officially part of this server. Congrats! You may begin chatting and playing. " +
						"Ensure you follow the rules and guidelines of this company."
				).Build();
			}));

			multiTask += guild.GetTextChannel(companyDiscordData.Channels.AnnouncementChannelId).SendMessageAsync(embed:
				EmbedHelper.CreateEmojiEmotionEmbed(EmotionEmoji.Wave,
					"A new recruit has joined this company!",
					$"Everyone welcome {user.Id.AsMentionableUser()} and their district!"
				).Build()
			);

			//Await all the tasks above
			await multiTask.WaitAll();
		}

	}
}
