using Discord;
using SpaceDiscordBot.Frameworks.Extensions;
using SpaceDiscordBot.Utilities;

namespace SpaceDiscordBot.Frameworks.Exceptions
{
	internal class EmbedException: Exception
	{
		public Embed Embed { get; }

		public LogSeverity Severity { get; }
		public string Title { get; }
		public string[] Description { get; }

		public EmbedException(LogSeverity severity, string title, params string[] description)
		{
			Severity = severity;
			Title = title;
			Description = description;

			EmbedBuilder builder = severity switch
			{
				LogSeverity.Critical => EmbedHelper.CreateScreamingAlert(title, description),
				LogSeverity.Error => EmbedHelper.CreateAlertEmbed(title, description),
				LogSeverity.Warning => EmbedHelper.CreateWarningEmbed(title, description),
				LogSeverity.Info => EmbedHelper.CreateApologistWarning(title, description),
				_ => EmbedHelper.CreateNoteEmbed(title, description)
			};

			Embed = builder.Build();
		}

		public EmbedException(LogSeverity severity, Embed embed)
		{
			Embed = embed;
			Severity = severity;
			Title = embed.Title;
			Description = embed.Description.Split('\n');
		}

		public void Log()
		{
			LogMessage message = new(Severity, Title, string.Join(' ', Description), this);
			message.ToConsole();
		}
	}
}
