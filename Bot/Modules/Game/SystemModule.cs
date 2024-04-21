using Discord;
using Discord.Interactions;
using SpaceCore;
using SpaceCore.Game.Components;
using SpaceCore.Game.Space;
using SpaceDiscordBot.Frameworks.Extensions;
using SpaceDiscordBot.Services.Discord;
using SpaceDiscordBot.Utilities;
using SpaceDiscordBot.Utilities.Game;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace SpaceDiscordBot.Modules.Game
{

	[RegisterModule(ModuleScope.Guilds)]
	internal class SystemModule(ServiceBundle services) : FocusingEntityModule(services)
	{
		public const string ROOT_SELECTED = "system.root";

		private const string SELECT_MODIFY = "system.modify";
		private const string SELECT_CHILD = "system.child";
		private const string SELECT_PARENT = "system.parent";
		private const string SELECT_DATA = "system.data";
		private const string SELECT_BUILD = "system.build";
		private const string SELECT_WAREHOUSE = "system.warehouse";

		public const string CHILDREN_SELECTED = "system.focus.children";
		public const string PARENTS_SELECTED = "system.focus.parents";

		private const string MODIFY_MODAL_SUBMIT = "system.modify.submit";

		private static SelectMenuBuilder BuildRoot(CosmicSystem system)
		{
			var builder = BuildBaseRoot(system, ROOT_SELECTED, "system", (SELECT_DATA, SELECT_MODIFY, SELECT_CHILD, SELECT_PARENT))
				.AddOption("Warehouse", SELECT_WAREHOUSE, "Manage this system's warehouse", Emoji.Parse("📦"))
				.AddOption("Build Menu", SELECT_BUILD, "Access the build menu for this system", Emoji.Parse("⚒️"));

			return builder; 
		}

		public static (ComponentBuilder components, EmbedBuilder embed) BuildEntry(CosmicSystem system)
		{
			var components = new ComponentBuilder().WithSelectMenu(BuildRoot(system));
			var general = CosmicSystemUtility.BuildGeneralEmbed(system);

			return (components, general);
		}


		[ComponentInteraction(ROOT_SELECTED)]
		public async Task RootSelection(string[] selections)
		{
			string selection = selections.First();

			var self = await GetSelfAs<CosmicSystem>();

			var rootMenu = BuildRoot(self).SetDefault(selection);
			var components = new ComponentBuilder().WithSelectMenu(rootMenu);

			switch(selection)
			{
				case SELECT_MODIFY:
					await RespondWithModalAsync(MODIFY_MODAL_SUBMIT, new ModifySystemModal(self));
					return;

				case SELECT_CHILD:
					components.AddFocusChildren(CHILDREN_SELECTED, self);
					break;

				case SELECT_PARENT:
					components.AddFocusParents(CHILDREN_SELECTED, self);
					break;
			}

			await DeferAsync();

			await ModifyOriginalResponseAsync(m => m.Components = components.Build());
		}


		[ComponentInteraction(CHILDREN_SELECTED)]
		public async Task ChildSelected(string[] selections) => await base.OnFocusChildren(selections);
		

		[ComponentInteraction(PARENTS_SELECTED)]
		public async Task ParentsSelected(string[] selections) => await base.OnFocusParents(selections);


		[ModalInteraction(MODIFY_MODAL_SUBMIT)]
		public async Task ModifyModalSubmit(ModifySystemModal modal)
		{
			await DeferAsync();

			var modifySelf = (CosmicSystem) await base.ModifySelfAsCosmicEntity(modal);

			await ModifyOriginalResponseAsync(m => m.Embed = BuildEntry(modifySelf).embed.Build());

			await SaveGameData();
		}


		public class ModifySystemModal : ModifyCosmicEntityModal
		{
			public ModifySystemModal() : base() { }

			public ModifySystemModal(CosmicSystem system) : base(system) 
			{
				Title = $"Modifying System:  {system.Name}  ({system.Id})";
			}

			public override string Title { get; set; } = "Modifying System";
		}

	}
}
