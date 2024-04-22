//Imports
using Discord.WebSocket;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog.Sinks.SystemConsole.Themes;
using Serilog;
using SpaceDiscordBot.http.Services;
using SpaceDiscordBot.Services.API;
using SpaceDiscordBot.Services.API.Discord;
using SpaceDiscordBot.Services.API.Game;
using SpaceDiscordBot.Services.Discord;
using SpaceDiscordBot.Settings;
using Discord.Interactions;
using SpaceDiscordBot.Services.API.Utility;
using System.Runtime.CompilerServices;

//And thus, we begin
Log.Information("[SPACE_DISCORD_BOT]:	Starting application");

var builder = WebApplication.CreateBuilder(args);

//Default configuration for this server
var Configuration = builder.Configuration;
		//.SetBasePath(Directory.GetCurrentDirectory())
		//.AddJsonFile("appsettings.json")
		//.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
		//.AddUserSecrets<Program>()
		//.Build();

//Assigning the static logger reference
Log.Logger = new LoggerConfiguration()
	.ReadFrom.Configuration(Configuration)
	.WriteTo.Console(theme: AnsiConsoleTheme.Code)
	.CreateLogger();

//Use Serilog for the app's logging provider
builder.Host.UseSerilog();

var services = builder.Services;

//Config for the bot
var config = new DiscordSocketConfig()
{
	
};

//TODO: Add services to startup class file
//TODO: Make startup class file
services
	//Discord Services
	.AddSingleton(config)
	.AddSingleton<DiscordClientService>()

	.AddSingleton<ModuleService>()
	.AddSingleton<ServiceBundle>()

	.AddSingleton<ChannelService>()
	.AddSingleton<GuildService>()

	.AddSingleton<NotificationService>()


	//Api
	.AddSingleton<HttpService>()
	.AddSingleton<PlayerGameDataService>()
	.AddSingleton<PlayerDiscordDataService>()
	.AddSingleton<CompanyDiscordDataService>()
	.AddSingleton<ResourceService>()

	.AddSingleton<ActionService>()

	.AddSingleton<NotificationService>()
	.AddSingleton<NamingService>()

	.AddSingleton(builder =>
	{
		var service = builder.GetRequiredService<DiscordClientService>();
		return new InteractionService(service.GetClient());
	});
;

services
	.Configure<ApiSettings>(Configuration.GetSection("ApiSettings"))
	.Configure<BotSettings>(Configuration.GetSection("BotSettings"))
;

services.AddControllers();

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}


//Get the client at the start to auto build the bot
DiscordClientService client = app.Services.GetRequiredService<DiscordClientService>();
ServiceBundle serviceBundle = app.Services.GetRequiredService<ServiceBundle>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

//flush the log files
Log.Information("[SPACE_DISCORD_BOT]:	Closing application");
Log.CloseAndFlush();

return 0;