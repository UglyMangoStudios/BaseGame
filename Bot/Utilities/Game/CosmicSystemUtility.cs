using Discord;
using SpaceCore.Game.Space;

namespace SpaceDiscordBot.Utilities.Game
{
	internal static class CosmicSystemUtility
    {

        public static EmbedBuilder BuildGeneralEmbed(CosmicSystem system)
        {
            EmbedBuilder embed = new();

            embed.WithTitle($"System {system.Name} ({system.Id})").WithColor(29, 209, 161).WithCurrentTimestamp();
            embed.WithDescription(system.Description ?? "No description has been set for this system");
            embed.WithFooter(system.WritePath());

            return embed;
        }


    }
}
