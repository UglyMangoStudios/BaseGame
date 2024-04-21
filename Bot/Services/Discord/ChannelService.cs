using Discord;
using Discord.WebSocket;
using SpaceCore.Data.Discord;
using SpaceDiscordBot.Frameworks.Exceptions;
using SpaceDiscordBot.Modules;
using SpaceDiscordBot.Services.Discord;

namespace SpaceDiscordBot.Services.API.Discord
{
    internal class ChannelService
	{
		private DiscordClientService _discordClientService { get; }

		private DiscordSocketClient _client { get; }


		public ChannelService(DiscordClientService discordClientService)
		{
			_discordClientService = discordClientService;
			_client = _discordClientService.GetClient();
		}


		public SocketTextChannel? GetTextChannel(SocketGuild guild, ulong id) => guild.GetTextChannel(id);

		public SocketTextChannel GetTextChannelEnforced(SocketGuild guild, ulong id) => GetTextChannel(guild, id) ?? throw new EmbedException(LogSeverity.Warning,
			nameof(GetTextChannelEnforced), "Was NULL.");


		public SocketThreadChannel? GetThread(SocketGuild guild, ulong id) => guild.GetThreadChannel(id);


		public SocketChannel GetThreadEnforced(SocketGuild guild, ulong id) => GetThread(guild, id) ?? throw new EmbedException(LogSeverity.Warning,
			nameof(GetThreadEnforced), "Was NULL.");



		public void SetCompanyChannelId(ChannelScope scope, CompanyDiscordData companyDiscordData, ulong id)
		{
			var channels = companyDiscordData.Channels;
			switch (scope)
			{
				case ChannelScope.AnnouncementChannel:
					channels.AnnouncementChannelId = id;
					break;
				case ChannelScope.PublicConsoleChannel:
					channels.CompanyConsoleChannelId = id;
					break;
				case ChannelScope.AdminConsoleChannel:
					channels.AdminConsoleChannelId = id;
					break;
				case ChannelScope.DataChannel:
					channels.CompanyDataChannelId = id;
					break;
				default: throw new
						EmbedException(LogSeverity.Error, nameof(SetCompanyChannelId), $"Channel {scope} out of scope for the company.");
			}
		}

		public ulong GetCompanyChannelId(ChannelScope scope, CompanyDiscordData compayDiscordData)
		{
			var channels = compayDiscordData.Channels;
			return scope switch
			{
				ChannelScope.AnnouncementChannel => channels.AnnouncementChannelId,
				ChannelScope.PublicConsoleChannel => channels.CompanyConsoleChannelId,
				ChannelScope.AdminConsoleChannel => channels.AdminConsoleChannelId,
				ChannelScope.DataChannel => channels.CompanyDataChannelId,
				_ => 0
			};
		}


		public SocketTextChannel? GetCompanyTextChannel(ChannelScope scope, SocketGuild guild, CompanyDiscordData companyDiscordData)
		{
			return GetTextChannel(guild, GetCompanyChannelId(scope, companyDiscordData));
		}


		public void SetPlayerChannelId(ChannelScope scope, PlayerDiscordData playerDiscordData, ulong id)
		{
			var channels = playerDiscordData.Channels;
			switch (scope)
			{
				case ChannelScope.PlayerLobbyChannel:
					channels.LobbyChannelId = id;
					break;
				case ChannelScope.NotificationsThread:
					channels.NotificationThreadId = id;
					break;
				case ChannelScope.AdventureThread:
					channels.AdventureThreadId = id;
					break;
				case ChannelScope.MissionsThread:
					channels.MissionsThreadId = id;
					break;
				case ChannelScope.HomeSystemThread:
					channels.HomeSystemThreadId = id;
					break;
				case ChannelScope.ColonySystemsThread:
					channels.ColonizedSystemsThreadId = id;
					break;
				case ChannelScope.OutpostThread:
					channels.OutpostsThreadId = id;
					break;
				default: throw new 
						EmbedException(LogSeverity.Error, nameof(SetPlayerChannelId), $"Channel {scope} out of scope for players.");
			}
		}


		public ulong GetPlayerThreadId(ChannelScope scope, PlayerDiscordData playerDiscordData)
		{
			var channels = playerDiscordData.Channels;
			return scope switch
			{
				ChannelScope.NotificationsThread => channels.NotificationThreadId,
				ChannelScope.AdventureThread => channels.AdventureThreadId,
				ChannelScope.MissionsThread => channels.MissionsThreadId,
				ChannelScope.HomeSystemThread => channels.HomeSystemThreadId,
				ChannelScope.ColonySystemsThread => channels.ColonizedSystemsThreadId,
				ChannelScope.OutpostThread => channels.OutpostsThreadId,
				_ => 0
			};
		}

		public SocketThreadChannel? GetPlayerThread(ChannelScope scope, SocketGuild guild, PlayerDiscordData playerDiscordData)
		{
			return GetThread(guild, GetPlayerThreadId(scope, playerDiscordData));
		}

		public ChannelScope? FindCompanyChannelScope(ulong id, CompanyDiscordData data)
		{
			foreach (var scope in GetCompanyChannelScopes())
			{
				if (GetCompanyChannelId(scope, data) == id)
					return scope;
			}

			return null;
		}

		public ChannelScope? FindPlayerThreadScope(ulong id, PlayerDiscordData data)
		{
			foreach(var scope in GetPlayerThreadScopes())
			{
				if (GetPlayerThreadId(scope, data) == id)
					return scope;
            }

			return null;
		}

		public ChannelScope? FindPlayerChannelScope(ulong id, PlayerDiscordData data)
		{
			foreach (var scope in GetPlayerChannelScopes())
			{
				if (GetPlayerChannelId(scope, data) == id)
					return scope;
			}

			return null;
		}



		public ulong GetPlayerChannelId(ChannelScope scope, PlayerDiscordData playerDiscordData)
		{
			var channels = playerDiscordData.Channels;
			return scope switch
			{
				ChannelScope.PlayerLobbyChannel => channels.LobbyChannelId,
				_ => 0
			};
		}

		public SocketChannel? GetPlayerTextChannel(ChannelScope scope, SocketGuild guild, PlayerDiscordData playerDiscordData)
		{
			return GetTextChannel(guild, GetPlayerChannelId(scope, playerDiscordData));
		}



		public ChannelScope[] GetCompanyChannelScopes() =>
			[
				ChannelScope.AnnouncementChannel,
				ChannelScope.PublicConsoleChannel,
				ChannelScope.AdminConsoleChannel,
				ChannelScope.DataChannel
			];

		public ChannelScope[] GetPlayerChannelScopes() =>
			[
				ChannelScope.PlayerLobbyChannel
			];

		public ChannelScope[] GetPlayerThreadScopes() => 
			[
				ChannelScope.NotificationsThread,
				ChannelScope.AdventureThread,
				ChannelScope.MissionsThread,
				ChannelScope.HomeSystemThread,
				ChannelScope.ColonySystemsThread,
				ChannelScope.OutpostThread
			];




		public ChannelData GetChannelData(ChannelScope scope)
		{
			return scope switch
			{
				ChannelScope.AnnouncementChannel => new ChannelData("📢announcements", UserWrite: false),
				ChannelScope.PublicConsoleChannel => new ChannelData("📱console"),
				ChannelScope.AdminConsoleChannel => new ChannelData("🖥️admin"),
				ChannelScope.DataChannel => new ChannelData("📰statistics"),

				ChannelScope.PlayerLobbyChannel => new ChannelData("{0}'s-public-lobby"),

				ChannelScope.NotificationsThread => new ChannelData("🔔 Notifications", UserWrite: false),
				ChannelScope.AdventureThread => new ChannelData("🌍 Adventures"),
				ChannelScope.MissionsThread => new ChannelData("📡 Missions"),
				ChannelScope.HomeSystemThread => new ChannelData("⭐ Home System"),
				ChannelScope.ColonySystemsThread => new ChannelData("🌙 Colony Systems"),
				ChannelScope.OutpostThread => new ChannelData("🏢 Outposts"),


				_ => throw new EmbedException(LogSeverity.Error, nameof(GetChannelData), $"Scope {scope} not yet configured.")
			};
		}

		public record class ChannelData(string Name, bool UserRead = true, bool UserWrite = true);
	}
}
