using Discord;
using Discord.WebSocket;
using SpaceDiscordBot.Frameworks;
using SpaceDiscordBot.http.Modals;
using SpaceDiscordBot.Services.Discord;
using SpaceDiscordBot.Utilities;

namespace SpaceDiscordBot.http.Services
{
    internal class NotificationService(DiscordClientService discordClientService)
	{
		private DiscordClientService _discordClientService { get; } = discordClientService;

		public async Task<bool> NotifyAsync(NotifyBundle notifyBundle)
        {
            var client = _discordClientService.GetClient();

			Console.WriteLine(">>> Client state: " + client.LoginState);
            var channel = await client.GetChannelAsync(notifyBundle.Channel);
            if (channel is not SocketTextChannel textChannel) return false;

            EmotionEmoji emoji = notifyBundle.NotificationType switch
            {
                NotificationType.Information => EmotionEmoji.Wave,
                NotificationType.GameUpdate => EmotionEmoji.Ring,
                NotificationType.GameAnnounce => EmotionEmoji.SoftAnnouncement,
                NotificationType.GlobalAnnounce => EmotionEmoji.StrongAnnouncement,
                _ => EmotionEmoji.Smile
            };

            Embed notification = EmbedHelper.CreateEmojiEmotionEmbed(
                emoji, notifyBundle.MessageTitle, notifyBundle.MessageBody
            ).Build();

            await textChannel.SendMessageAsync(embed: notification);

            return true;
        }
    }
}
