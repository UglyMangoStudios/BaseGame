using Discord;
using Microsoft.AspNetCore.Mvc.Rendering;
using Serilog;
using SpaceCore.Game.Components;
using SpaceCore.Game.Entities.Buildables;
using SpaceCore.Game.Space;
using SpaceCore.Game.Space.Base;
using SpaceCore.Game.Space.Bodies;
//using SpaceCore.Game.Space.Bodies.Components;

namespace SpaceDiscordBot.Utilities.Game
{
	internal static class FocusableUtility
	{

		public static string WritePath(this IFocusable focusable)
		{
			IFocusable? parent = focusable.GetType().GetProperty("Parent")?.GetValue(focusable) as IFocusable;
			return parent is not null 
				? parent.WritePath() + "/" + focusable.FocusId 
				: focusable.FocusId;
		}



		public static SelectMenuBuilder SelectMenuToChildren(string customId, IFocusable focusable, ushort maxValues = 5)
		{
			var builder = new SelectMenuBuilder().WithCustomId(customId);

			var children = focusable.GetChildren().Take(25);
			maxValues = ushort.Clamp(maxValues, 1, (ushort) children.Count());

			builder.WithMaxValues(maxValues);
			builder.WithPlaceholder($"Select up to {maxValues} children");

			foreach(var child in children)
			{
				string id = child.FocusId;

				string? name, description;
				IEmote? emote = FocusableUtility.ParseToEmote(child);

				if (child is CosmicEntity entity)
				{
					name = entity.Name;
					description = entity.Description;
				} else if (child is Buildable buildable)
				{
					name = buildable.Name;
					description = "TODO: Building description here";
				} else
				{
					name = "Child " + id;
					description = "Unknown entity.";
				}

				builder.AddOption(
					$"{name} ({id})",
					id,
					description,
					emote
				);
			}


			return builder;
		}


		public static SelectMenuBuilder SelectMenuToParents(string customId, IFocusable focusable, ushort maxValues = 5)
		{
			var builder = new SelectMenuBuilder().WithCustomId(customId);

			var parents = focusable.GetParents().Take(25);
			maxValues = ushort.Clamp(maxValues, 1, (ushort) parents.Count());

			builder.WithMaxValues(maxValues);
			builder.WithPlaceholder($"Select up to {maxValues} parents");

			foreach(var parent in parents) 
			{
				string id = parent.FocusId;

				Console.WriteLine(id);

				string? name, description;
				IEmote? emote = FocusableUtility.ParseToEmote(parent);

				if (parent is CosmicEntity entity)
				{
					name = entity.Name;
					description = entity.Description;
				}
				else if (parent is Buildable buildable)
				{
					name = buildable.Name;
					description = "TODO: Building description here";
				}
				else
				{
					name = "Child " + id;
					description = "Unknown entity.";
				}

				builder.AddOption(
					$"{name} ({id})",
					id,
					description,
					emote
				);
			}

			return builder;
		}


		public static IEmote ParseToEmote(IFocusable f)
		{
			return f switch
			{
				CosmicSystem => Emoji.Parse("🌌"),
				Star => Emoji.Parse("☀️"),
				Planet => Emoji.Parse("🌍"),
				Moon => Emoji.Parse("🌙"),
				Buildable => Emoji.Parse("🏢"),
				_ => Emoji.Parse("❓")
			};
		}


		/// <summary>
		/// Potentially retrieves a focus object in reference to a root using a string.
		/// 
		/// <br/> The character <c>.</c> acts as the same current object delimiter
		/// <br/> The characters <c>..</c> acts as the parent delimiter
		/// <br/> All other characters are perceived as the id for a child
		/// </summary>
		/// <param name="root">The reference node to begin path traversal</param>
		/// <param name="path">The path to travel along</param>
		/// <returns>The found object or <c>null</c> if not found</returns>
		public static IFocusable? FromPath(this IFocusable? root, string path)
		{
			if (path == string.Empty) return root;


			var split = path.Split('/')
				.Select(s => s.Trim())
				.Where(s => s != string.Empty);

			IFocusable? current = root;

			foreach (string child in split)
			{
				if (current is null) return null;
				else if (child == "." || current.FocusId == child) continue;
				else if (child == "..")
				{
					current = current.GetType().GetProperty("Parent")?.GetValue(current) as IFocusable;
					continue;
				}


				current = current.GetChild(child);
			}

			return current;
		}
	}
}
