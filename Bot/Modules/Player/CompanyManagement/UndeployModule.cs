using Discord.WebSocket;
using Discord;
using Core.Data.Discord;
using Core.Types;
using SpaceDiscordBot.Frameworks;
using SpaceDiscordBot.Utilities;
using Discord.Interactions;
using SpaceDiscordBot.Services.Discord;

namespace SpaceDiscordBot.Modules.Player.CompanyManagement
{

    [RegisterModule(ModuleScope.EstablishedCompany)]
	internal class UndeployModule(ServiceBundle services) : BaseModule(services)
	{
		[SlashCommand("undeploy", "Removes your district from this company's jurisdiction. Game ticks halt while in company limbo.")]
		public async Task Command()
		{
			await DeferAsync(true);

			var guild = AsSocketGuild();
			CompanyDiscordData companyDiscordData = await Services.EnsureCompanyDiscordDataAsync(guild.Id);

			SocketGuildUser user = EnsureGuildUser(Context.User);

			PlayerDiscordData playerDiscordData = await Services.GetPlayerDiscordDataAsync(user.Id);
			if (playerDiscordData.GuildId != guild.Id)
			{
				await ModifyOriginalResponseAsync( m => m.Embed = EmbedHelper.CreateApologistWarning("",
					"You do not have your district under this company's jurisdiction. No action has been taken.")
				.Build());

				return;
			}

			MultiTask multiTask = new();

			var channels = playerDiscordData.Channels;
			if (channels.LobbyChannelId != 0)
			{
				SocketGuildChannel? lobbyChannel = guild.GetChannel(channels.LobbyChannelId);
				if (lobbyChannel != null && lobbyChannel is ITextChannel textChannel)
				{
					//Wrap so all threads are deleted first, then the channel can delete.
					//I'm not sure what the behavior would be if the channel was deleted then the threads tried to be.
					multiTask.Run(async () =>
					{
						await Services.GuildService.DeleteThreadsFromChannel(guild, textChannel);
						await textChannel.DeleteAsync();
					});
				}
			}

			playerDiscordData.GuildId = 0;
			playerDiscordData.Channels = new();

			companyDiscordData.RegisteredPlayers.Remove(user.Id);

			var announcementChannelNullable = Services.ChannelService.GetCompanyTextChannel(ChannelScope.AnnouncementChannel, guild, companyDiscordData);
			var announcementChannel = Ensure(nameof(announcementChannelNullable), announcementChannelNullable);

			multiTask += announcementChannel.SendMessageAsync(embed: EmbedHelper.CreateEmojiEmotionEmbed(
							EmotionEmoji.BigCri, "A valued member left this company.",
							"What did y'all do?? Poor " + user.Id.AsMentionableUser() + " has left the jurisdiction of this company"
						).Build()
				);


			multiTask += ModifyOriginalResponseAsync(message =>
			{
				message.Embed = EmbedHelper.CreateSuccessEmbed(
								"Goodbye 😭",
								"You have unrooted yourself from this company. Let's hope you don't regret this decision."
							).Build();
			});

			multiTask += Services.PlayerDiscordDataService.UpdateAsync(user.Id, playerDiscordData);
			multiTask += Services.CompanyDiscordDataService.UpdateAsync(guild.Id, companyDiscordData);

			await multiTask.WaitAll();
		}
	}
}
