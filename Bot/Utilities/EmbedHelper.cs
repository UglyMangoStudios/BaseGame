using Discord;
using SpaceDiscordBot.Frameworks;

namespace SpaceDiscordBot.Utilities
{

    /// <summary>
    /// A simple class that aids in the development of embeds. Increases reusability of code without needing to 
    /// write the same type of embeds over and over. Plus, they look nice.
    /// </summary>
    internal static class EmbedHelper
	{
		//Color Palette: https://flatuicolors.com/palette/defo

		/// <summary> A simple embed. Not much to offer, but at least its an embed. </summary>
		public static EmbedBuilder BasicEmbed(string title, params string[] description) =>
			CreateEmbedBuilder(title, stitchStringArray(description), (149, 165, 166));

		/// <summary> An embed used for successful operations </summary>
		public static EmbedBuilder CreateSuccessEmbed(string title, params string[] description) =>
			CreateEmbedBuilder(":white_check_mark:   Success. " + title, stitchStringArray(description), (46, 204, 113));

		/// <summary> An embed used for failed operations </summary>
		public static EmbedBuilder CreateFailureEmbed(string title, params string[] description) =>
			CreateEmbedBuilder(":x:   Failed. " + title, stitchStringArray(description), (231, 76, 60));

		/// <summary> An embed used as a general notification alert </summary>
		public static EmbedBuilder CreateNotificationEmbed(string title, params string[] description) =>
			CreateEmbedBuilder(":bell:   Ding ding! " + title, stitchStringArray(description), (230, 126, 34));

		/// <summary> An embed that mildly offers a suggestion. </summary>
		public static EmbedBuilder CreateNoteEmbed(string title, params string[] description) =>
			CreateEmbedBuilder(":smile:   Hey! " + title, stitchStringArray(description), (26, 188, 156));

		/// <summary> An embed thats timid yet corrective, saying something didn't work. Usually from a user's error. </summary>
		public static EmbedBuilder CreateApologistWarning(string title, params string[] description) =>
			CreateEmbedBuilder(":sweat_smile:   Oops, sorry! " + title, stitchStringArray(description), (243, 156, 18));

		/// <summary> An embed that serves a cautionary message. </summary>
		public static EmbedBuilder CreateWarningEmbed(string title, params string[] description) =>
			CreateEmbedBuilder(":warning:   Warning! " + title, stitchStringArray(description), (241, 196, 15));

		/// <summary> An attention-grabbing embed that alerts users of something serious. </summary>
		public static EmbedBuilder CreateAlertEmbed(string title, params string[] description) =>
			CreateEmbedBuilder(":rotating_light:   Attention! " + title, stitchStringArray(description), (231, 76, 60));

		/// <summary> An over-dramatic screaming embed for something extremely critical </summary>
		public static EmbedBuilder CreateScreamingAlert(string title, params string[] description) =>
			CreateEmbedBuilder(":scream:   AHHHH! " + title, stitchStringArray(description), (192, 57, 43));

		/// <summary> For debugging purposes. Used as a way to convey an unexpected error occured. </summary>
		/// <remarks>Returns an already built embed, so no new information can be added.</remarks>
		public static Embed ScreamErrorEmbed(string error)
		{
			Console.WriteLine("A guild received an awful error:\n" + error);
			return CreateScreamingAlert("We encountered a bad error!", "Let server staff know of this error ASAP!!", "Error: " + error).Build();
		}


		public static EmbedBuilder UnderConstruction(params string[] description) => 
			CreateApologistWarning("Under Construction!", description);

		public static Embed Loading() => CreateEmbedBuilder(":alarm_clock:    Loading...", "", (241, 196, 15)).Build();

		/// <summary> Helper method that creates an embed starting with a particular emoji </summary>
		public static EmbedBuilder CreateEmojiEmotionEmbed(EmotionEmoji emoji, string title, params string[] description) =>
			CreateEmbedBuilder(emoji + "  " + title, stitchStringArray(description), emoji.Color);

		/// <summary> Strings an array of strings into one string. </summary>
		private static string stitchStringArray(params string[] strings) => string.Join("\n", strings);

		/// <summary>
		/// Handy method that creates an embed instead of the verbose and vanilla method through Discord.NET
		/// </summary>
		/// <param name="title">The title of the embed.</param>
		/// <param name="description">The description of the embed</param>
		/// <param name="color">The color of the embed. It's displayed as a strip on the left side. Each of the RGB components is expressed 0-255</param>
		/// <param name="withTimestamp">A boolean to toggle if the current time should be displayed or not.</param>
		/// <returns>An <see cref="EmbedBuilder"/> that can continue being modified.</returns>
		public static EmbedBuilder CreateEmbedBuilder(string title, string description, (byte r, byte g, byte b) color, bool withTimestamp = true)
		{
			EmbedBuilder builder = new();

			builder
				.WithTitle(title)
				.WithDescription(description)
				.WithColor(color.r, color.g, color.b);

			if(withTimestamp) builder.WithCurrentTimestamp();

			return builder;
		}


		/// <summary>
		/// Helper method that extends any interaction that replies with an embed.
		/// </summary>
		/// <param name="interaction">the interaction to respond to</param>
		/// <param name="embed">The embed to reply with</param>
		/// <returns></returns>
		public static async Task RespondEmbedAsync(this IDiscordInteraction interaction, Embed embed, bool ephemeral = false)
			=> await interaction.RespondAsync(embed: embed, ephemeral: ephemeral);
		/// <summary>
		/// Helper method that extends any interaction that replies with an embed builder.
		/// </summary>
		/// <param name="interaction">the interaction to respond to</param>
		/// <param name="embed">The embed builder to reply with</param>
		/// <returns></returns>
		public static async Task RespondEmbedAsync(this IDiscordInteraction interaction, EmbedBuilder embedBuilder, bool ephemeral = false)
			=> await RespondEmbedAsync(interaction, embedBuilder.Build(), ephemeral);


		public static async Task RespondEmbedDeferredAsync(this IDiscordInteraction interaction, Embed embed) =>
			await interaction.ModifyOriginalResponseAsync(prop => prop.Embed = embed);

		/// <summary>
		/// Helper method that allows any interaction to respond with an embed that has a file (image)
		/// </summary>
		/// <param name="interaction">The interaction to extend</param>
		/// <param name="embed">The embed to respond with</param>
		/// <param name="path">The actual path of the file within this application</param>
		/// <param name="ephemeral">If true, the message will be ephemeral</param>
		/// <returns>An awaitable task</returns>
		public static async Task RespondImageAttachmentEmbedAsync(this IDiscordInteraction interaction, Embed embed, string path, bool ephemeral = false) 
			=> await interaction.RespondWithFileAsync(path, embed: embed, ephemeral: ephemeral);

		/// <summary>
		/// A simple method that adds an attachment image to this embed's thumbnail
		/// </summary>
		/// <param name="embedBuilder">The embed builder to extend</param>
		/// <param name="filename">The name of the file</param>
		public static void WithThumbnailAttachment(this EmbedBuilder embedBuilder, string filename) => embedBuilder.WithThumbnailUrl($"attachment://{filename}");

		/// <summary>
		/// A simple method that adds an attachment image to this embed's image
		/// </summary>
		/// <param name="embedBuilder">The embed builder to extend</param>
		/// <param name="filename">The name of the file</param>
		public static void WithImageAttachment(this EmbedBuilder embedBuilder, string filename) => embedBuilder.WithImageUrl($"attachment://{filename}");

	}
}
