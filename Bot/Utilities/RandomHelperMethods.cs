using System.Text;

namespace SpaceDiscordBot.Utilities
{
	internal static class RandomHelperMethods
	{

		public static string SexyToString<K, V>(this Dictionary<K, V> dick) where K : notnull
		{
			StringBuilder sb = new();
			foreach(var kvp in dick)
			{
				sb.Append($"{kvp.Key?.ToString()}:{kvp.Value?.ToString()}\n");
			}

			return sb.ToString();
		}

		//thanks to: https://stackoverflow.com/questions/1064901/random-number-between-2-double-numbers
		public static double NextDouble(this Random random, double min, double max) 
			=> random.NextDouble() * (max - min) + min;

		public static string AsMentionableRole(this ulong id) => $"<@&{id}>";
		public static string AsMentionableChannel(this ulong id) => $"<#{id}>";

		public static string AsMentionableUser(this ulong id) => $"<@{id}>";
	}
}
