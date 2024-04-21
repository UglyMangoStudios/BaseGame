using Discord;
using SpaceCore.Game.Space.Bodies;

namespace SpaceDiscordBot.Utilities.Game
{
	internal static class StarUtility
	{
		public static EmbedBuilder BuildGeneralEmbed(Star star)
		{
			EmbedBuilder builder = new();

			builder.WithTitle($"{Emoji.Parse("⭐")} Star {star.Name} ({star.Id})")
				.WithColor(254, 202, 87)
				.WithFooter(star.WritePath())
				.WithCurrentTimestamp();

			return builder;
		}
	}
}
