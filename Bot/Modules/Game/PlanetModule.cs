using Discord;
using Discord.Interactions;
using SpaceCore.Game.Components;
using SpaceCore.Game.Space;
using SpaceCore.Game.Space.Bodies;
using SpaceDiscordBot.Frameworks.Extensions;
using SpaceDiscordBot.Services.Discord;
using SpaceDiscordBot.Utilities.Game;

namespace SpaceDiscordBot.Modules.Game
{

	[RegisterModule(ModuleScope.Guilds)]
	internal class PlanetModule(ServiceBundle services) : FocusingEntityModule(services)
	{
		public const string ROOT_SELECTED = "planet.root";

		private const string SELECT_MODIFY = "planet.modify";
		private const string SELECT_CHILD = "planet.child";
		private const string SELECT_PARENT = "planet.parent";
		private const string SELECT_DATA = "planet.data";
		private const string SELECT_BUILD = "planet.build";

		public const string CHILDREN_SELECTED = "planet.focus.children";
		public const string PARENTS_SELECTED = "planet.focus.parents";

		private const string MODIFY_MODAL_SUBMIT = "planet.modify.submit";

		private static SelectMenuBuilder BuildRoot(Planet planet)
		{
			var builder = BuildBaseRoot(planet, ROOT_SELECTED, "planet", (SELECT_DATA, SELECT_MODIFY, SELECT_CHILD, SELECT_PARENT))
				.AddOption("Build Menu", SELECT_BUILD, "Access the build menu for this planet", Emoji.Parse("⚒️"));

			return builder;
		}


		public static (ComponentBuilder components, EmbedBuilder embed) BuildEntry(Planet planet)
		{
			ComponentBuilder builder = new();
			builder.WithSelectMenu(BuildRoot(planet));

			EmbedBuilder embed = PlanetUtility.BuildGeneralEmbed(planet);

			return (builder, embed);
		}


		[ComponentInteraction(ROOT_SELECTED)]
		public async Task RootSelectMenu(string[] selections)
		{
			string path = GetEmbedFooter();
			string selection = selections.First();

			var self = await GetSelfAs<Planet>();

			var rootMenu = BuildRoot(self).SetDefault(selection);
			var components = new ComponentBuilder().WithSelectMenu(rootMenu);

			switch (selection)
			{
				case SELECT_MODIFY:
					await RespondWithModalAsync(MODIFY_MODAL_SUBMIT, new ModifyPlanetModal(self));
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
		public async Task ChildrenSelected(string[] selections) => await base.OnFocusChildren(selections);

		[ComponentInteraction(PARENTS_SELECTED)]
		public async Task ParentsSelected(string[] selections) => await base.OnFocusParents(selections);


		[ModalInteraction(MODIFY_MODAL_SUBMIT)]
		public async Task ModifyModalSubmit(ModifyPlanetModal modal)
		{
			await DeferAsync();

			var modifySelf =  (Planet) await base.ModifySelfAsCosmicEntity(modal);

			await ModifyOriginalResponseAsync(m => m.Embed = BuildEntry(modifySelf).embed.Build());

			await SaveGameData();
		}



		public class ModifyPlanetModal : ModifyCosmicEntityModal
		{
			public ModifyPlanetModal() : base() { }

			public ModifyPlanetModal(Planet planet) : base(planet)
			{
				Title = $"Modifying Planet:  {planet.Name}  ({planet.Id})";
			}

			public override string Title { get; set; } = "Modifying Planet";
		}

	}
}
