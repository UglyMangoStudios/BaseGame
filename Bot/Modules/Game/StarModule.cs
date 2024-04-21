using Discord;
using Discord.Interactions;
using SpaceCore.Game.Components;
using SpaceCore.Game.Space.Bodies;
using SpaceDiscordBot.Frameworks.Extensions;
using SpaceDiscordBot.Services.Discord;
using SpaceDiscordBot.Utilities.Game;

namespace SpaceDiscordBot.Modules.Game
{

	[RegisterModule(ModuleScope.EstablishedCompany)]
	internal class StarModule(ServiceBundle services) : FocusingEntityModule(services)
	{

		public const string ROOT_SELECTED = "star.root";
		
		private const string SELECT_MODIFY = "star.modify";
		private const string SELECT_CHILD = "star.child";
		private const string SELECT_PARENT = "star.parent";
		private const string SELECT_DATA = "star.data";
		private const string SELECT_BUILD = "star.build";

		public const string CHILDREN_SELECTED = "star.focus.children";
		public const string PARENTS_SELECTED = "star.focus.parents";

		private const string MODIFY_MODAL_SUBMIT = "star.modify.submit";
		private static SelectMenuBuilder BuildRoot(Star star)
		{
			var builder = BuildBaseRoot(star, ROOT_SELECTED, "star", (SELECT_DATA, SELECT_MODIFY, SELECT_CHILD, SELECT_PARENT))
				.AddOption("Build Menu", SELECT_BUILD, "Access the build menu for this star", Emoji.Parse("⚒️"));

			return builder;
		}

		public static (ComponentBuilder components, EmbedBuilder embed) BuildEntry(Star star)
		{
			var components = new ComponentBuilder().WithSelectMenu(BuildRoot(star));
			var embed = StarUtility.BuildGeneralEmbed(star);

			return (components, embed);
		}


		[ComponentInteraction(ROOT_SELECTED)]
		public async Task OnRootSelection(string[] selections)
		{
			string path = GetEmbedFooter();
			string selection = selections.First();

			var self = await GetSelfAs<Star>();

			var rootMenu = BuildRoot(self).SetDefault(selection);
			var components = new ComponentBuilder().WithSelectMenu(rootMenu);

			switch (selection)
			{
				case SELECT_MODIFY:
					await RespondWithModalAsync(MODIFY_MODAL_SUBMIT, new ModifyStarModal(self));
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
		public async Task ModifyModalSubmit(ModifyStarModal modal)
		{
			await DeferAsync();

			var modifySelf = (Star) await base.ModifySelfAsCosmicEntity(modal);

			await ModifyOriginalResponseAsync(m => m.Embed = BuildEntry(modifySelf).embed.Build());

			await SaveGameData();
		}



		public class ModifyStarModal : ModifyCosmicEntityModal
		{
			public ModifyStarModal() : base() { }

			public ModifyStarModal(Star star) : base(star)
			{
				Title = $"Modifying Star:  {star.Name}  ({star.Id})";
			}

			public override string Title { get; set; } = "Modifying Star";
		}
	}
}
