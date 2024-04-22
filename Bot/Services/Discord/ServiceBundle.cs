using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Core.Data.Discord;
using Core.Data.Saves;
using SpaceDiscordBot.Frameworks.Exceptions;
using SpaceDiscordBot.Services.API.Discord;
using SpaceDiscordBot.Services.API.Game;
using SpaceDiscordBot.Services.API.Utility;
using System.Net;

namespace SpaceDiscordBot.Services.Discord
{
	internal class ServiceBundle(
			DiscordClientService discordClientService, 
			ModuleService moduleService,

			ResourceService resourceService,
			PlayerGameDataService playerGameDataService, 
			CompanyDiscordDataService companyDiscordDataService,
			PlayerDiscordDataService playerDiscordDataService,
			ChannelService channelService,
			GuildService guildService,

			ActionService actionService,
			NamingService namingService
		)
	{

		public DiscordClientService DiscordClientService { get; } = discordClientService;
		public ModuleService ModuleService { get; } = moduleService;

		public ResourceService ResourceService { get; } = resourceService;
		public PlayerGameDataService PlayerGameDataService { get; } = playerGameDataService;
		public CompanyDiscordDataService CompanyDiscordDataService { get; } = companyDiscordDataService;
		public PlayerDiscordDataService PlayerDiscordDataService { get; } = playerDiscordDataService;

		public InteractionService InteractionService { get; } = new(discordClientService.GetClient().Rest);

		public ChannelService ChannelService { get; } = channelService;
		public GuildService GuildService { get; } = guildService;

		public ActionService ActionService { get; } = actionService;
		public NamingService NamingService { get; } = namingService;

		#region Helper Service Accessor Methods

		public DiscordSocketClient GetClient() => DiscordClientService.GetClient();



		public async Task<PlayerGameData> GetPlayerGameDataAsync(ulong userId)
		{
			var result = await PlayerGameDataService.GetAsync(userId);

			if (result.Success)
				return result.Value!;

			else if (!result.Success && result.Code != HttpStatusCode.NotFound)
				throw new EmbedException(LogSeverity.Warning, nameof(GetPlayerGameDataAsync),
					"Could not retrieve player game data.",
					$"Status code: {result.Code}");

			else
			{
				result = await PlayerGameDataService.CreateAsync(userId);

				Console.WriteLine("Code: " + result.Code);

				result.EnsureSuccess(nameof(GetPlayerGameDataAsync), message: "Could not post player game data.");

				return result.Value!;
			}
		}

		public async Task<PlayerDiscordData> GetPlayerDiscordDataAsync(ulong userId)
		{
			var result = await PlayerDiscordDataService.GetAsync(userId);

			if (result.Success)
				return result.Value!;

			else if (!result.Success && result.Code != HttpStatusCode.NotFound)
				throw new EmbedException(LogSeverity.Warning, nameof(GetPlayerDiscordDataAsync),
					"Could not retrieve player Discord data.",
					$"Status code: {result.Code}");

			else
			{
				result = await PlayerDiscordDataService.CreateAsync(new PlayerDiscordData(userId));
				result.EnsureSuccess(nameof(GetPlayerDiscordDataAsync), message: "Could not post player Discord data.");

				return result.Value!;
			}
		}

		public async Task<CompanyDiscordData> EnsureCompanyDiscordDataAsync(ulong? guildId)
		{
			if (guildId == null)
				throw new EmbedException(LogSeverity.Warning, nameof(EnsureCompanyDiscordDataAsync),
					"Could not retrieve company Discord data because could not retrieve guild id.");

			ulong id = (ulong)guildId;

			var result = await CompanyDiscordDataService.GetAsync(id);

			if (result.Success)
				return result.Value!;

			else
				throw new EmbedException(LogSeverity.Warning, nameof(EnsureCompanyDiscordDataAsync),
					"Could not retrieve company Discord data.",
					$"Status code: {result.Code}");
		}

		public async Task<CompanyDiscordData?> GetCompanyDiscordDataAsync(ulong? guildId)
		{
			if (guildId == null) return null;

			ulong id = (ulong)guildId;

			var result = await CompanyDiscordDataService.GetAsync(id);

			if (result.Success)
				return result.Value!;

			else if (!result.Success && result.Code != HttpStatusCode.NotFound)
				throw new EmbedException(LogSeverity.Warning, nameof(GetCompanyDiscordDataAsync),
					"Could not retrieve company Discord data.",
					$"Status code: {result.Code}");

			else return null;
		}

		#endregion
	}
}
