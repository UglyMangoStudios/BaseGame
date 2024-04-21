using Discord;
using SpaceDiscordBot.Frameworks.Exceptions;
using SpaceDiscordBot.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpaceDiscordBot.Frameworks.Extensions
{
	internal static class ExceptionExtensions
	{

		public static EmbedException ToEmbedException(this Exception exception, LogSeverity severity = LogSeverity.Error) =>
			new(severity, exception.ToEmbed(severity));


		public static EmbedBuilder ToEmbedBuilder(this Exception exception, LogSeverity severity = LogSeverity.Error)
		{
			string title = "We encountered an error.";
			string[] description = [$"`{exception.Message}`", $"{exception.StackTrace ?? "No stack trace found"}"];

			return severity switch
			{
				LogSeverity.Critical => EmbedHelper.CreateScreamingAlert(title, description),
				LogSeverity.Error => EmbedHelper.CreateAlertEmbed(title, description),
				LogSeverity.Warning => EmbedHelper.CreateWarningEmbed(title, description),
				LogSeverity.Info => EmbedHelper.CreateApologistWarning(title, description),
				_ => EmbedHelper.CreateNoteEmbed(title, description)
			};
		}

		public static Embed ToEmbed(this Exception exception, LogSeverity severity = LogSeverity.Error) =>
			exception.ToEmbedBuilder(severity).Build();

	}
}
