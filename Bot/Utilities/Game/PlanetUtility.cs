using Discord;
using SpaceCore.Game.Space.Bodies;

namespace SpaceDiscordBot.Utilities.Game
{
	internal static class PlanetUtility
	{
		public static EmbedBuilder BuildGeneralEmbed(Planet planet)
		{
			EmbedBuilder embed = new();

			embed.WithTitle($"{Emoji.Parse("🌍")} Planet {planet.Name} ({planet.Id})").WithColor(243, 104, 224).WithCurrentTimestamp();
			embed.WithFooter(planet.WritePath());

			return embed;
		}
	}
}
