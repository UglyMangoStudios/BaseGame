using Discord;
using Core.Game.Entities.Buildables;

namespace SpaceDiscordBot.Utilities.Game
{
	internal static class BuildableUtility
	{
		public static EmbedBuilder BuildGeneralEmbed(Buildable buildable)
		{
			EmbedBuilder builder = new();

			builder.WithTitle($"{Emoji.Parse("🗺️")} Building {buildable.Name} ({buildable.Id})")
				.WithColor(0, 210, 211)
				.WithFooter(buildable.WritePath())
				.WithCurrentTimestamp();

			return builder;
		}
	}
}
