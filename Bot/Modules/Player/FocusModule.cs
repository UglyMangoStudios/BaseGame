using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using SpaceCore.Game.Components;
using SpaceCore.Game.Space;
using SpaceCore.Game.Space.Bodies;
using SpaceDiscordBot.Modules.Game;
using SpaceDiscordBot.Services.Discord;
using SpaceDiscordBot.Utilities;
using SpaceDiscordBot.Utilities.Game;
using System.IO;
using System.Linq;

namespace SpaceDiscordBot.Modules.Player
{

	[RegisterModule(ModuleScope.EstablishedCompany)]
	internal class FocusModule(ServiceBundle services) : BaseModule(services)
	{

		[AutocompleteCommand("path", "focus")]
		public async Task AutocompletePath()
		{
			var interaction = (SocketAutocompleteInteraction) Context.Interaction;
			string input = interaction.Data.Current.Value.ToString()!.Trim();
			var split = input.Split('/');

			IFocusable? current = (await GetActionPacket()).GetChannelFocus(ChannelId);

			string appendingPath = string.Empty;
			string currentDelimiter = split[0];

			List<AutocompleteResult> potentials = [];

			if (split.Length > 1)
			{
				appendingPath = string.Join('/', split.SkipLast(1));
				currentDelimiter = split.Last();

				current = current?.FromPath(appendingPath);
			} else
			{
				potentials.Add(new("./", "./"));

				if (current?.HasParent() == true) potentials.Add(new("../", "../"));
			}

			if (split.All(s => s.Equals("..")) && current?.HasParent() == true)
			{
				string path = appendingPath + "/../";
				potentials.Add(new(path, path));
			}

			if (current is null)
			{
				await interaction.RespondAsync();
				return;
			}

			foreach (var child in current.GetChildren())
			{
				string path = child.FocusId + "/";
				if (appendingPath != string.Empty)
					path = appendingPath + "/" + path;

				potentials.Add(new AutocompleteResult(path, path));
			}

			var filtered = potentials
				.Where(result =>
				{
					//Takes the result's name and trims any whitespaces and '/' characters
					//Then takes the current option the user is typing and checks to see
					//if there exists an child with said input
					
					//Ignores case
					return result.Name.Trim(' ', '/').Split("/").Last().Contains(currentDelimiter, StringComparison.CurrentCultureIgnoreCase);
				})
				.Take(25);      //cap set by discord API

			await interaction.RespondAsync(filtered);
		}



		[SlashCommand("focus", "Navigate your focus via a shell-like path or send an interactable message")]
		public async Task Focus([Autocomplete] string? path = null)
		{
			await DeferAsync();

			if (path is null)
				await HandleFocusInteractable(await GetChannelDefaultFocus());
			else
				await HandleFocusPath(path);
		}

		private async Task HandleFocusInteractable(IFocusable? focus, string? path = null)
		{

			(ComponentBuilder components, EmbedBuilder embed) = focus switch
			{
				null => (new(), EmbedHelper.CreateFailureEmbed(
					"Invalid path.",
					"We could not figure out what you tried to select.",
					"You entered `" + (path ?? "NOTFOUND") + "`")),
				_ => FocusingEntityModule.BuildForFocus(focus)
			};

			await ModifyOriginalResponseAsync(m =>
			{
				m.Embed = embed.Build();
				m.Components = components.Build();
			});
		}

		private async Task HandleFocusPath(string path)
		{
			var packet = await GetActionPacket();
			IFocusable? destination = packet.GetChannelFocus(ChannelId)?.FromPath(path);

			if (destination is not null)
				packet.SetChannelFocus(ChannelId, destination);

			await HandleFocusInteractable(destination, path);
		}
	}
}
