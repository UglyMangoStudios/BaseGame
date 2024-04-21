using Discord.Interactions;
using Discord;
using SpaceDiscordBot.Frameworks.Attributes;
using SpaceDiscordBot.Services.Discord;
using Discord.WebSocket;

namespace SpaceDiscordBot.Modules.Development
{
    [RegisterModule(ModuleScope.Guilds)]
	[LimitRuntime(Runtime.Development)]
	internal class SampleAutocomplete(ServiceBundle services) : BaseModule(services)
	{
		[AutocompleteCommand("parameter_name", "command_name")]
		public async Task Autocomplete()
		{
			var interaction = Context.Interaction as SocketAutocompleteInteraction;
			string userInput = interaction!.Data.Current.Value.ToString() ?? string.Empty;

			IEnumerable<AutocompleteResult> results = new[]
			{
				new AutocompleteResult("foo", "foo_value"),
				new AutocompleteResult("bar", "bar_value"),
				new AutocompleteResult("baz", "baz_value"),
			}.Where(x => x.Name.StartsWith(userInput, StringComparison.InvariantCultureIgnoreCase)); 
			// only send suggestions that starts with user's input; use case insensitive matching


			// max - 25 suggestions at a time
			await interaction.RespondAsync(results.Take(25));
		}

		// you need to add `Autocomplete` attribute before parameter to add autocompletion to it
		[SlashCommand("sample_autocomplete", "lets sample that autocomplete!")]
		public async Task ExampleCommand([Summary("parameter_name"), Autocomplete] string parameterWithAutocompletion)
			=> await RespondAsync($"Your choice: {parameterWithAutocompletion}");
	}
}
