using Discord;
using Serilog;

namespace SpaceDiscordBot.Frameworks.Extensions
{
	internal static class LoggingExtensions
	{
		public static void ToConsole(this LogMessage message)
		{
			string m = message.ToString();
			message.Severity.ToConsole(m);
		}

		public static void ToConsole(this LogSeverity severity, string message)
		{
			switch (severity)
			{
				case LogSeverity.Critical:
					Log.Fatal(message);
					break;
				case LogSeverity.Error:
					Log.Error(message);
					break;
				case LogSeverity.Warning:
					Log.Warning(message);
					break;
				case LogSeverity.Info:
					Log.Information(message);
					break;
				case LogSeverity.Verbose:
					Log.Verbose(message);
					break;
				case LogSeverity.Debug:
					Log.Debug(message);
					break;
			}
		}
	}
}
