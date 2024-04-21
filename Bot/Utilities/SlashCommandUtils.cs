using Discord.WebSocket;

namespace SpaceDiscordBot.Utilities
{
	/// <summary>
	/// A collection of utility methods that aid in slash command development
	/// </summary>
	internal static class SlashCommandUtils
	{
		/// <summary>
		/// Deconstructs a slash command into key/value components from what the user provided.
		/// </summary>
		/// <param name="command">The command to split up. Chop chop. Splice 'n dice. Yum yum dinner</param>
		/// <returns>The dictionary that picked the juicy slash command meats off the bones</returns>
		public static Dictionary<string, object> DeconstructOptionsToDict(this SocketSlashCommand command)
		{
			return createDictFromOptions(command.Data.Options);
		}

		/// <summary>
		/// A private helper method that recursively to get every morsel of data
		/// </summary>
		/// <param name="options">The option to recursively go from.</param>
		/// <returns>The delicious dict.</returns>
		private static Dictionary<string, object> createDictFromOptions(IEnumerable<SocketSlashCommandDataOption> options)
		{
			Dictionary<string, object> dict = new();
			foreach(var option in options)
			{
				bool isSubcommand = option.Type == Discord.ApplicationCommandOptionType.SubCommand;
				if (isSubcommand) dict["subcommand"] = option.Name;

				bool isSubcommandGroup = option.Type == Discord.ApplicationCommandOptionType.SubCommandGroup;
				if (isSubcommandGroup) dict["subcommand_group"] = option.Name;

				if(isSubcommand || isSubcommandGroup)
				{
					var newDict = createDictFromOptions(option.Options);
					foreach((string key,  object value) in newDict)
					{
						dict[key] = value;
					}
				} else
				{
					dict[option.Name] = option.Value;
				}

			}

			return dict;
		}
	}
}
