using Discord.Interactions;
using Core.Data.Discord;
using Core.Types;
using SpaceDiscordBot.Frameworks;
using SpaceDiscordBot.Services.Discord;
using SpaceDiscordBot.Utilities;

namespace SpaceDiscordBot.Modules.Player.CompanyManagement
{
	[RegisterModule(ModuleScope.EstablishedCompany)]
	internal class CompanyUtilityModules(ServiceBundle services) : BaseModule(services)
	{



		[SlashCommand("wake", "Reactivates your threads so you can continue playing. Welcome back!")]
		public async Task WakeCommand()
		{
			var wake_embed = EmbedHelper.CreateEmojiEmotionEmbed(EmotionEmoji.SleepyHead,
								"Good Morning! ☀️",
								"It looks like you have been away for a while. Welcome back! We've missed you :smiling_face_with_tear:"
			).Build();

			var success_embed = EmbedHelper.CreateSuccessEmbed("", "Your channels have been awoken.").Build();

			var user = EnsureGuildUser(Context.User);
			ulong userId = user.Id;

			var guild = AsSocketGuild();

			PlayerDiscordData playerDiscordData = await Services.GetPlayerDiscordDataAsync(userId);
			MultiTask multiTask = new();

			foreach (ChannelScope scope in Services.ChannelService.GetPlayerThreadScopes())
			{
				var thread = Services.ChannelService.GetPlayerThread(scope, guild, playerDiscordData);
				if (thread is null) continue;

				multiTask.Run(async () =>
				{
					if (!thread.Users.Any(u => u.Id == user.Id))
						await thread.AddUserAsync(user);

					await thread.SendMessageAsync(embed: wake_embed);
				});
			}

			multiTask += RespondEmbedAsync(success_embed, true);
			await multiTask.WaitAll();
		}
	}
}
