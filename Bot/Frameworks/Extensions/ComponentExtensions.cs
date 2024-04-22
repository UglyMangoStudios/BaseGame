using Discord;
using Discord.Interactions;
using Serilog;
using Core.Game.Components;
using SpaceDiscordBot.Utilities.Game;
using System.Reflection;

namespace SpaceDiscordBot.Frameworks.Extensions
{
	internal static class ComponentExtensions
	{
		public static void SetDefault(this List<SelectMenuOptionBuilder> options, string? value = null)
		{
			if (value is null) return;
			
			var find = options.FirstOrDefault(o => o.Value == value);
			if (find is null) return;

			find.WithDefault(true);
		}

		public static SelectMenuBuilder SetDefault(this SelectMenuBuilder selectMenu, string? value = null)
		{
			selectMenu.Options.SetDefault(value);
			return selectMenu;
		}


		public static SelectMenuBuilder SetDefault(this SelectMenuBuilder selectMenu, params string?[] tries)
		{
			foreach(string? attempt in tries)
			{
				selectMenu.SetDefault(attempt);
			}
			return selectMenu;
		}


		public static ButtonBuilder AttachDelegate(this ButtonBuilder buttonBuilder, Delegate @delegate)
		{
			var attribute = @delegate.Method.GetCustomAttribute<ComponentInteractionAttribute>();
			if (attribute is not null)
				buttonBuilder.WithCustomId(attribute.CustomId);
			if (attribute == null)
				//TODO: Maybe exception it?
				Log.Error("Attempting to attach delegate to button but this delegate cannot find the necessary attributes to link to");

			return buttonBuilder;
		}


		public static ComponentBuilder AddFocusChildren(this ComponentBuilder builder, string customId, IFocusable focus) =>
			builder.WithSelectMenu(FocusableUtility.SelectMenuToChildren(customId, focus));

		public static ComponentBuilder AddFocusParents(this ComponentBuilder builder, string customId, IFocusable focus) =>
			builder.WithSelectMenu(FocusableUtility.SelectMenuToParents(customId, focus));
	}
}
