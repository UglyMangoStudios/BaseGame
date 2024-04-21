using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Serilog;
using SpaceCore.Data.Discord;
using SpaceCore.Data.Saves;
using SpaceCore.Game.Components;
using SpaceCore.Types;
using SpaceDiscordBot.Frameworks.Exceptions;
using SpaceDiscordBot.Services.Discord;
using static SpaceDiscordBot.Services.Discord.ActionService;

namespace SpaceDiscordBot.Modules
{
	internal abstract class BaseModule: InteractionModuleBase<SocketInteractionContext>
	{
		protected ServiceBundle Services { get; }


		private PlayerGameData? playerGameData = null;
		private PlayerDiscordData? playerDiscordData = null;

		public BaseModule(ServiceBundle services)
		{
			Services = services;
		}


		protected ILogger Logger { get; } = Log.ForContext<BaseModule>();

		protected ulong UserId => Context.User.Id;
		protected ulong ChannelId => Context.Channel.Id;

		protected SocketGuild AsSocketGuild() =>
			Context.Guild as SocketGuild ?? 
			throw new NullReferenceException(nameof(Context.Guild));


		protected async Task RespondEmbedAsync(Embed embed, bool ephemeral = false) =>
			await RespondAsync(embed: embed, ephemeral: ephemeral);

		protected async Task RespondEmbedAsync(EmbedBuilder builder, bool ephemeral = false) =>
			await RespondEmbedAsync(builder.Build(), ephemeral);



		protected SocketGuildUser EnsureGuildUser(IUser user)
		{
			return user as SocketGuildUser ?? throw
				new EmbedException(LogSeverity.Error, nameof(EnsureGuildUser), "Required user to be guild user");
		}

		protected async Task<PlayerGameData> GetPlayerGameData()
		{
			if (playerGameData is null)
				playerGameData = await Services.GetPlayerGameDataAsync(UserId);

			return playerGameData;
		}

		protected async Task<PlayerDiscordData> GetPlayerDiscordData()
		{
			if (playerDiscordData is null)
				playerDiscordData = await Services.GetPlayerDiscordDataAsync(UserId);

			return playerDiscordData;
		}

		protected async Task<ActionPacket> GetActionPacket(bool defaultFocus = true)
		{
			PlayerGameData gameData = null!;
			PlayerDiscordData discordData = null!;

			//Assign both values at the same time
			MultiTask multiTask = new();

			multiTask.Run(async () => gameData = await GetPlayerGameData());
			multiTask.Run(async () => discordData = await GetPlayerDiscordData());

			await multiTask.WaitAll();


			var packet = Services.ActionService.GetPlayerPacket(UserId);

			if (defaultFocus)
			{
				ChannelScope? scope = Services.ChannelService.FindPlayerThreadScope(ChannelId, discordData);

				if (packet.GetChannelFocus(ChannelId) is null)
					packet.SetChannelFocus(ChannelId, await GetChannelDefaultFocus());

			}

			return packet;
		}

		protected async Task<IFocusable> GetChannelDefaultFocus()
		{
			var discordData = await GetPlayerDiscordData();
			var gameData = await GetPlayerGameData();

			ChannelScope? scope = Services.ChannelService.FindPlayerThreadScope(ChannelId, discordData);
			return scope switch
			{
				ChannelScope.HomeSystemThread => gameData.HomeSystem,
				ChannelScope.ColonySystemsThread => gameData.ColonySystems,
				ChannelScope.OutpostThread => gameData.OutpostSystems,
				_ => throw new InvalidOperationException($"Channel scope {scope} does not have a default focus."),
			};
		}

		protected string GetEmbedFooter()
		{
			var interaction = Context.Interaction as SocketMessageComponent;
			return interaction?.Message.Embeds.FirstOrDefault()?.Footer?.Text ?? string.Empty;
		}



		protected T Ensure<T>(string source, T? @object)
		{
			return @object ?? throw new EmbedException(LogSeverity.Error, source, "Nonnull requirement failed.");
		}


		protected SocketMessageComponent AsMessageInteraction()
		{
			return Context.Interaction as SocketMessageComponent 
				?? throw new InvalidOperationException(nameof(AsMessageInteraction));
		}


		protected async Task<bool> SaveGameData()
		{
			var gameData = await GetPlayerGameData();
			var response = await Services.PlayerGameDataService.UpdateAsync(gameData);
			return response.Success;
		}

	}
}
