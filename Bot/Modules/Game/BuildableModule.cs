using Discord;
using Discord.Interactions;
using Core.Game.Components;
using Core.Game.Entities.Buildables;
using SpaceDiscordBot.Frameworks.Extensions;
using SpaceDiscordBot.Services.Discord;
using SpaceDiscordBot.Utilities.Game;

namespace SpaceDiscordBot.Modules.Game
{
	internal class BuildableModule(ServiceBundle services) : FocusingEntityModule(services)
	{

		public const string ROOT_SELECTED = "buildable.selection.root";
		public const string CHILDREN_SELECTED = "buildable.selection.child.focus";

		private const string FOCUS_CHILD = "buildable.focus.child";
		private const string FOCUS_PARENT = "buildable.focus.parent";
		private const string SELECT_DATA = "buildable.data";
		private const string SELECT_BUILD = "buildable.build";

		private static SelectMenuBuilder BuildRoot(IFocusable entity)
		{
			var builder = new SelectMenuBuilder()
				.WithCustomId(ROOT_SELECTED)
				.AddOption("More Data", SELECT_DATA, "Display more data pertaining to this building", Emoji.Parse("📱"))
				.AddOption("Build Menu", SELECT_BUILD, "Access the build menu for this building", Emoji.Parse("⚒️"));

			if (entity.HasChildren())
				builder.AddOption("Focus Child", FOCUS_CHILD, "Select a child of this system to building", Emoji.Parse("🔎"));

			if (entity.HasParent())
				builder.AddOption("Focus Parent", FOCUS_PARENT, "Select a parent of this system to building", Emoji.Parse("📡"));

			return builder;
		}

		public static (ComponentBuilder, EmbedBuilder) BuildEntry(Buildable buildable)
		{
			var components = new ComponentBuilder().WithSelectMenu(BuildRoot(buildable));
			var embed = BuildableUtility.BuildGeneralEmbed(buildable);

			return (components, embed);
		}


		[ComponentInteraction(ROOT_SELECTED)]
		public async Task OnRootSelection(string[] selections)
		{
			await DeferAsync();

			string path = GetEmbedFooter();
			string selection = selections.First();

			IFocusable self = (await GetChannelDefaultFocus()).FromPath(path)!;

			var rootMenu = BuildRoot(self).SetDefault(selection);
			var components = new ComponentBuilder().WithSelectMenu(rootMenu);

			switch (selection)
			{
				case FOCUS_CHILD:
					components.AddFocusChildren(CHILDREN_SELECTED, self);
					break;
			}

			await ModifyOriginalResponseAsync(m => m.Components = components.Build());
		}



		[ComponentInteraction(CHILDREN_SELECTED)]
		public async Task OnChildrenSelected(string[] selections) => await base.OnFocusChildren(selections);
	}
}
