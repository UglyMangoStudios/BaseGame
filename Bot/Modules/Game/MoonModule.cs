using Discord;
using Discord.Interactions;
using Serilog.Sinks.SystemConsole.Themes;
using Core.Game.Components;
using Core.Game.Space.Bodies;
using SpaceDiscordBot.Frameworks.Extensions;
using SpaceDiscordBot.Services.Discord;
using SpaceDiscordBot.Utilities.Game;

namespace SpaceDiscordBot.Modules.Game
{

	[RegisterModule(ModuleScope.EstablishedCompany)]
	internal class MoonModule(ServiceBundle services) : FocusingEntityModule(services)
	{

		public const string ROOT_SELECTED = "moon.root";

		private const string SELECT_MODIFY = "moon.modify";
		private const string SELECT_CHILD = "moon.focus.child";
		private const string SELECT_PARENT = "moon.focus.parent";
		private const string SELECT_DATA = "moon.data";
		private const string SELECT_BUILD = "moon.build";

		public const string CHILDREN_SELECTED = "moon.focus.children";
		public const string PARENTS_SELECTED = "moon.focus.parents";

		private const string MODIFY_MODAL_SUBMIT = "moon.modify.submit";

		private static SelectMenuBuilder BuildRoot(Moon moon)
		{
			var builder = BuildBaseRoot(moon, ROOT_SELECTED, "moon", (SELECT_DATA, SELECT_MODIFY, SELECT_CHILD, SELECT_PARENT))
				.AddOption("Build Menu", SELECT_BUILD, "Access the build menu for this moon", Emoji.Parse("⚒️"));

			return builder;
		}

		public static (ComponentBuilder componets, EmbedBuilder embed) BuildEntry(Moon moon)
		{
			var components = new ComponentBuilder().WithSelectMenu(BuildRoot(moon));
			var embed = MoonUtility.BuildGeneralEmbed(moon);

			return (components, embed);
		}


		[ComponentInteraction(ROOT_SELECTED)]
		public async Task OnRootSelection(string[] selections)
		{
			string path = GetEmbedFooter();
			string selection = selections.First();

			var self = await GetSelfAs<Moon>();

			var rootMenu = BuildRoot(self).SetDefault(selection);
			var components = new ComponentBuilder().WithSelectMenu(rootMenu);

			switch (selection)
			{
				case SELECT_MODIFY:
					await RespondWithModalAsync(MODIFY_MODAL_SUBMIT, new ModifyMoonModal(self));
					return;

				case SELECT_CHILD:
					components.AddFocusChildren(CHILDREN_SELECTED, self);
					break;
				case SELECT_PARENT:
					components.AddFocusParents(PARENTS_SELECTED, self);
					break;	
			}

			await DeferAsync();

			await ModifyOriginalResponseAsync(m => m.Components = components.Build());
		}



		[ComponentInteraction(CHILDREN_SELECTED)]
		public async Task OnChildrenSelected(string[] selections) => await base.OnFocusChildren(selections);

		[ComponentInteraction(PARENTS_SELECTED)]
		public async Task ParentsSelected(string[] selections) => await base.OnFocusParents(selections);


		[ModalInteraction(MODIFY_MODAL_SUBMIT)]
		public async Task ModifyModalSubmit(ModifyMoonModal modal)
		{
			await DeferAsync();

			var modifySelf = (Moon) await base.ModifySelfAsCosmicEntity(modal);

			await ModifyOriginalResponseAsync(m => m.Embed = BuildEntry(modifySelf).embed.Build());

			await SaveGameData();
		}



		public class ModifyMoonModal : ModifyCosmicEntityModal
		{
			public ModifyMoonModal() : base() { }

			public ModifyMoonModal(Moon moon) : base(moon)
			{
				Title = $"Modifying Moon:  {moon.Name}  ({moon.Id})";
			}

			public override string Title { get; set; } = "Modifying Moon";
		}
	}
}
