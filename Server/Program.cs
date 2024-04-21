using Microsoft.AspNetCore.Components.Forms.Mapping;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using SpaceServer.Controllers.Config;
using SpaceServer.Controllers.Formatters;
using SpaceServer.Controllers.RouteConstraints;
using SpaceServer.Database;
using SpaceServer.Database.Base;
using SpaceServer.Database.Discord;
using SpaceServer.Database.Game;
using SpaceServer.Services.Company;
using SpaceServer.Services.Game;
using SpaceServer.Services.Game.Generation;
using SpaceServer.Services.Player;
using SpaceServer.Services.Utilization;
using SpaceServer.Settings;
using SpaceServer.Settings.Binder;
using SpaceServer.Settings.Contexts;
using SwaggerThemes;
using Swashbuckle.AspNetCore.Swagger;

//Default configuration for this server
var Configuration = new ConfigurationBuilder()
		.SetBasePath(Directory.GetCurrentDirectory())
		.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
		.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
		.Build();

//Assigning the static logger reference

LoggingService loggingService = new();

Log.Logger = new LoggerConfiguration()
	.ReadFrom.Configuration(Configuration)
	.WriteTo.Console(theme: AnsiConsoleTheme.Code)
	.WriteTo.File("/logs/.log", rollingInterval: RollingInterval.Day)
	.WriteTo.Sink(loggingService)
	.CreateLogger();


Log.Information("[SPACE_SERVER]: Starting application");

var builder = WebApplication.CreateBuilder(args);

//Add Serilog to the application
builder.Host.UseSerilog();

var services = builder.Services;

// Add services to the container.

services
	.AddScoped<IConfigurationRoot>(_ => Configuration)

	//Datacontexts
	.AddScoped<GameSavesContext>()
	.AddScoped<GameDataContext>()
	.AddScoped<DiscordDataContext>()
	.AddScoped<UtilizationContext>()
	.AddScoped<ContextInitializer>()

	// Logging
	.AddSingleton(loggingService)
	
	//Player data
	.AddScoped<PlayerDiscordDataService>()
	.AddScoped<PlayerGameDataService>()
	
	//Company data
	.AddScoped<CompanyDiscordDataService>()

	//Game data
	.AddScoped<ResourceService>()
	.AddScoped<RecipeService>()
	.AddScoped<RecipeBuildingService>()


	.AddScoped<NamingService>()


	.AddScoped<SystemGenerationService>()
	
	;


// TODO: Automate using reflections
services
	.Configure<GameDataSettings>(Configuration.GetSection("GameDataSettings"))
	.Configure<UtilizationDataSettings>(Configuration.GetSection("UtilizationDataSettings"))
	.Configure<ContextInitializerSettings>(Configuration.GetSection("ContextInitializerSettings"))
	.Configure<SwaggerSettings>(Configuration.GetSection("SwaggerSettings"))

	.Configure<RouteOptions>(options =>
	options.ConstraintMap.Add(UlongRouteConstraint.UlongRouteConstraintName, typeof(UlongRouteConstraint))
);

//Thanks to:
//https://stackoverflow.com/questions/41824957/can-i-make-an-asp-net-core-controller-internal
services.AddMvc().ConfigureApplicationPartManager(manager =>
{
	manager.FeatureProviders.Add(new CustomControllerFeatureProvider());
});


services.AddControllers(options =>	
{
	options.InputFormatters.Add(new ByteArrayInputFormatter());
	options.InputFormatters.Add(new TextPlainInputFormatter());
}).AddNewtonsoftJson(setup =>
{
	var settings = setup.SerializerSettings;
	settings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All;
	settings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
	settings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.All;
	settings.SerializationBinder = new EntityFrameworkSerializationBinder();

});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var provider = scope.ServiceProvider;
var initializer = provider.GetRequiredService<ContextInitializer>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	//Development database 
	app.UseSwagger();

	var swaggerSettings = scope.ServiceProvider.GetService<IOptions<SwaggerSettings>>()?.Value;
	if (swaggerSettings is not null)
	{
		var theme = swaggerSettings.GetTheme();
		string? customCSS = swaggerSettings.CustomCSS;
		app.UseSwaggerThemes(theme, customCSS);
	}

	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

//flush the log files
Log.Information("[SPACE_SERVER]: Closing application");
Log.CloseAndFlush();

Console.ReadLine();

return 0;

