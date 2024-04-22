using Discord;
using Core.Game.Space.Bodies;

namespace SpaceDiscordBot.Utilities.Game
{
	internal static class MoonUtility
	{
		public static EmbedBuilder BuildGeneralEmbed(Moon moon)
		{
			EmbedBuilder builder = new();

			builder.WithTitle($"{Emoji.Parse("🌙")} Moon {moon.Name} ({moon.Id})")
				.WithColor(87, 101, 116)
				.WithFooter(moon.WritePath())
				.WithCurrentTimestamp();

			return builder;
		}
	}
}
