using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using Serilog;
using SpaceDiscordBot.Frameworks.Extensions;
using SpaceDiscordBot.Settings;

namespace SpaceDiscordBot.Services.Discord
{
	internal class DiscordClientService
	{

		//Setting
		private DiscordSocketConfig _socketConfig { get; }
		private BotSettings _botSettings { get; }

		//Services

		//Active Service References
		private DiscordSocketClient Client { get; }
		public DiscordSocketClient GetClient() => Client;

		private Task ClientThread { get; }


		private SocketGuild _centralGuild { get; set; } = null!;
		public SocketGuild GetCentralGuild() => _centralGuild;
		

		public DiscordClientService(DiscordSocketConfig socketConfig, IOptions<BotSettings> botSettingsOptions)
		{
			//Settings
			_socketConfig = socketConfig;
			_botSettings = botSettingsOptions.Value;

			Client = new(socketConfig);

			if (_botSettings.Token is null || _botSettings.HQGuildId == 0)
			{
				Log.Fatal("The bot token and the central guild id is required within the Environment! This is a fatal exception.");
				Environment.Exit(1);
			}

			//Bot subscriptions to events
			Client.Log += this.BasicTaskLog;

			Client.Ready += async () =>
			{
				_centralGuild = Client.GetGuild(_botSettings.HQGuildId);
				if (_centralGuild is null)
				{
					Log.Fatal("Unable to find the central discord server from the provided id. This is a fatal exception.");
					Environment.Exit(1);
				}
			};

			ClientThread = Task.Run(async () =>
			{
				await Client.LoginAsync(TokenType.Bot, _botSettings.Token);
				await Client.StartAsync();
			});
		}



		/// <summary>
		/// Used by the bot. Subscribes to a logging event.
		/// </summary>
		public Task BasicTaskLog(LogMessage msg)
		{
			msg.ToConsole();
			return Task.CompletedTask;
		}

	}
}
