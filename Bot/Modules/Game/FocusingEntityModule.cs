using Discord;
using Discord.Interactions;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using SpaceCore;
using SpaceCore.Game.Components;
using SpaceCore.Game.Entities.Buildables;
using SpaceCore.Game.Space;
using SpaceCore.Game.Space.Base;
using SpaceCore.Game.Space.Bodies;
//using SpaceCore.Game.Space.Bodies.Components;
using SpaceCore.Types;
using SpaceDiscordBot.Services.Discord;
using SpaceDiscordBot.Utilities;
using SpaceDiscordBot.Utilities.Game;
using System.Net;

namespace SpaceDiscordBot.Modules.Game
{
	internal class FocusingEntityModule(ServiceBundle services) : BaseModule(services)
	{
		public static (ComponentBuilder, EmbedBuilder) BuildForFocus(IFocusable focus)
		{
			ComponentBuilder components;
			EmbedBuilder embed;

			switch (focus)
			{
				case CosmicSystem system:
					(components, embed) = SystemModule.BuildEntry(system);
					break;

				case Star star:
					(components, embed) = StarModule.BuildEntry(star);
					break;

				case Planet planet:
					(components, embed) = PlanetModule.BuildEntry(planet);
					break;

				case Moon moon:
					(components, embed) = MoonModule.BuildEntry(moon);
					break;

				//case Region region:
				//	(components, embed) = RegionModule.BuildEntry(region);
				//	break;

				case Buildable buildable:
					(components, embed) = BuildableModule.BuildEntry(buildable);
					break;

				default:
					embed = EmbedHelper.CreateApologistWarning("", "Child is an unknown type", "Typeof: " + focus.GetType().FullName);
					components = new();
					break;
			};

			return (components, embed);
		}


		protected static SelectMenuBuilder BuildBaseRoot(IFocusable focus, string customId, string entityName, 
			(string data, string modify, string child, string parent) listeners)
		{
			var builder = new SelectMenuBuilder()
				.WithCustomId(customId)
				.AddOption("More Data", listeners.data, "Display more data pertaining to this system", Emoji.Parse("📱"))
				.AddOption("Modify", listeners.modify, $"Modify this {entityName}", Emoji.Parse("📝"));

			if (focus.HasChildren())
				builder.AddOption("Focus Child", listeners.child, $"Select a child of this {entityName} to focus", Emoji.Parse("🔎"));

			if (focus.HasParent())
				builder.AddOption("Focus Parent", listeners.parent, $"Select a parent of this {entityName} to focus", Emoji.Parse("📡"));

			return builder;
		}


		public async Task<T> GetSelfAs<T>()
			where T : IFocusable
		{
			string path = GetEmbedFooter();
			return (T)(await GetChannelDefaultFocus()).FromPath(path)!;
		}

		protected async Task OnFocusChildren(string[] selections)
		{
			string path = GetEmbedFooter();
			IFocusable defaultFocus = await GetChannelDefaultFocus();
			IFocusable self = defaultFocus.FromPath(path)!;

			foreach (string selection in selections)
			{
				IFocusable? child = self.GetChild(selection);

				ComponentBuilder? components = null;
				EmbedBuilder? embed = null;

				if (child is null)
				{
					embed = EmbedHelper.CreateApologistWarning("", $"We cannot find a child with the ID {selection}");
				}
				else
					(components, embed) = BuildForFocus(child);

				if (selection == selections.First())
					await RespondAsync(embed: embed?.Build(), components: components?.Build());
				else
					await ReplyAsync(embed: embed?.Build(), components: components?.Build());
			}
		}

		protected async Task OnFocusParents(string[] selections)
		{
			string path = GetEmbedFooter();
			IFocusable defaultFocus = await GetChannelDefaultFocus();
			IFocusable self = defaultFocus.FromPath(path)!;

			var parents = self.GetParents();

			foreach (string selection in selections)
			{
				IFocusable? parent = parents.FirstOrDefault(p => p.FocusId == selection);

				ComponentBuilder? components = null;
				EmbedBuilder? embed = null;

				if (parent is null)
				{
					embed = EmbedHelper.CreateApologistWarning("", $"We cannot find a parent with the id {selection}");
				}
				else
					(components, embed) = BuildForFocus(parent);

				if (selection == selections.First())
					await RespondAsync(embed: embed?.Build(), components: components?.Build());
				else
					await ReplyAsync(embed: embed?.Build(), components: components?.Build());
			}
		}


		protected static ModalBuilder CosmicEntityModifyModal(string customId, CosmicEntity? entity, string? title = null)
		{
			title ??= $"Modifying {entity?.Id ?? "[unknown]"}";

			var builder = new ModalBuilder()
				.WithCustomId(customId)
				.WithTitle(title);

			if (entity is null) return builder;

			string name = entity.Name;
			string? description = entity.Description;

			builder.AddTextInput("Name", "name", placeholder: name, value: name);
			builder.AddTextInput("Description", "description", placeholder: description ?? "No description has been set", value: description);

			return builder;
		}

		protected async Task<CosmicEntity> ModifySelfAsCosmicEntity(ModifyCosmicEntityModal modal)
		{
			var self = await GetSelfAs<CosmicEntity>();

			bool isRandom = Ref.Random.Has(modal.Name);
			if (isRandom) modal.Name = (await Services.NamingService.GetRandomAsync()).Value;

			self.Name = modal.Name ?? self.Name;
			self.Description = modal.Description ?? self.Description;

			return self;
		}



		public class ModifyCosmicEntityModal() : IModal
		{
			protected ModifyCosmicEntityModal(CosmicEntity entity) : this() 
			{
				Title += entity.Id;
				Name = entity.Name;
				Description = entity.Description;
			}

			public virtual string Title { get; set; } = $"Modifying entity ";


			[RequiredInput(false), ModalTextInput("modify.name", maxLength: 100)]
			public string? Name { get; set; }


			[RequiredInput(false), ModalTextInput("modify.description", style: TextInputStyle.Paragraph)]
			public string? Description { get; set; }
		}


	}
}
