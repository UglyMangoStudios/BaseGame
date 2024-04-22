using System.Data;
using Core.Data.Discord;
using Core.Types;
using SpaceDiscordBot.Frameworks.Attributes;
using Serilog;
using System.Reflection;
using Discord.Interactions;
using SpaceDiscordBot.Modules;
using SpaceDiscordBot.Services.API.Discord;
using Discord;
using SpaceDiscordBot.Utilities;
using SpaceDiscordBot.Frameworks.Exceptions;
using SpaceDiscordBot.Frameworks.Extensions;
using Discord.WebSocket;

namespace SpaceDiscordBot.Services.Discord
{

    internal class ModuleService
	{
		private Dictionary<ModuleScope, List<ModuleInfo>> RegisteredModules = [];

		//Services
		private DiscordClientService DiscordClientService { get; }
		private InteractionService InteractionService { get; }
		private IServiceProvider ServiceProvider { get; }

		private CompanyDiscordDataService CompanyDiscordDataService { get; }

		private ILogger Logger { get; }


		public ModuleService(InteractionService interactionService, CompanyDiscordDataService companyDiscordDataService, DiscordClientService discordClientService, IServiceProvider serviceProvider)
		{
			Logger = Log.ForContext<ModuleService>();

			DiscordClientService = discordClientService;
			InteractionService = interactionService;
			ServiceProvider = serviceProvider;

			CompanyDiscordDataService = companyDiscordDataService;

			var client = DiscordClientService.GetClient();

			//Auto populate the dict with empty lists for each scope in the enum.
			Enum.GetValues<ModuleScope>().ToList().ForEach(scope => RegisteredModules[scope] = []);

			//Subscribe any interactions to the bot
			client.InteractionCreated += async (interaction) =>
			{
				SocketInteractionContext context = new(DiscordClientService.GetClient(), interaction);
				var result = await InteractionService.ExecuteCommandAsync(context, serviceProvider);
			};

			//When the bot is ready, register all modules
			client.Ready += RegisterModules;
			interactionService.InteractionExecuted += InteractionExecuted;
		}

		private static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
		{
			while (toCheck != null && toCheck != typeof(object))
			{
				var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
				if (generic == cur)
				{
					return true;
				}
				toCheck = toCheck.BaseType;
			}
			return false;
		}

		/// <summary>
		/// Comb through the assembly and collect all modules
		/// </summary>
		private async Task CollectModules()
		{
			MultiTask multiTask = new();

			Logger.Information("Beginning Module Collection....");

			var filtered = GetType().Assembly
				.GetTypes()
				.Where(t => t.IsClass && !t.IsAbstract && IsSubclassOfRawGeneric(typeof(InteractionModuleBase<>), t));

			ushort count = 0;
			foreach (Type type in filtered)
			{
				//Limit the object
				if (type.GetCustomAttribute<LimitRuntimeAttribute>() is LimitRuntimeAttribute limit)
				{
					string? runtime = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

					if (runtime == "Development" && limit.Runtime != Runtime.Development) continue;
					else if (runtime == "Production" && limit.Runtime != Runtime.Production) continue;
				}

				var registerModule = type.GetCustomAttribute<RegisterModuleAttribute>();
				if (registerModule is null) continue;

				ModuleScope moduleScope = registerModule.ModuleScope;

				multiTask.Run(async () =>
				{
					var moduleInfo = await InteractionService.AddModuleAsync(type, ServiceProvider);
					RegisteredModules[moduleScope].Add(moduleInfo);
				});

				count++;
			}

			Logger.Information("Found and registered {count} modules.", count);
			await multiTask.WaitAll();
		}


		public async Task RegisterModules()
		{
			await CollectModules();

			MultiTask multiTask = new();

			//Gather relevant guilds
			var centralGuild = DiscordClientService.GetCentralGuild();
			var guilds = DiscordClientService.GetClient().Guilds;

			//Gather relevant modules
			var centralModules = RegisteredModules[ModuleScope.HQ];
			var globalModules = RegisteredModules[ModuleScope.Global];
			var guildModules = RegisteredModules[ModuleScope.Guilds];

			multiTask += InteractionService.AddModulesGloballyAsync(true, [.. globalModules]);
			multiTask += InteractionService.AddModulesToGuildAsync(centralGuild, true, [.. centralModules, .. guildModules]);

			foreach (var guild in guilds)
				multiTask += UpdateGuildModules(guild.Id);

			await multiTask.WaitAll();
		}


		public async Task UpdateGuildModules(ulong guildId)
		{
			//Attempt to retrieve the guild's data
			CompanyDiscordData? data = (await CompanyDiscordDataService.GetAsync(guildId)).Value;

			bool isEstablished = data?.IsEstablished() == true; //Checks for NULL and truth at the same time

			//Depending on the guild's state, retrieve the appropriate modules
			//Add global guild commands too
			List<ModuleInfo> list = [
				.. RegisteredModules[ModuleScope.Guilds],
				.. RegisteredModules[ModuleScope.Company],
				.. RegisteredModules[isEstablished ? ModuleScope.EstablishedCompany : ModuleScope.PreEstablishedCompany]
			];

			//Add the modules to the guild
			await InteractionService.AddModulesToGuildAsync(guildId, true, list.ToArray());
		}


		private async Task InteractionExecuted(ICommandInfo commandInfo, IInteractionContext context, IResult result)
		{
			var interaction = context.Interaction;

			if (result.IsSuccess)
				Logger.Information("Interaction {info} successfully executed", commandInfo.Name);
			else
				Logger.Information("Interaction {info} failed to execute", commandInfo?.Name);


			if (!result.IsSuccess && result is ExecuteResult execute)
			{ 
				EmbedBuilder embed = execute.Error switch
				{
					InteractionCommandError.UnmetPrecondition => EmbedHelper.CreateApologistWarning("Unmet Conditions", result.ErrorReason),
					InteractionCommandError.UnknownCommand => EmbedHelper.CreateApologistWarning("", "That's an unknown command..."),
					InteractionCommandError.BadArgs => EmbedHelper.CreateApologistWarning("", "Invalid number or arguments"),

					InteractionCommandError.Exception when execute.Exception.InnerException is EmbedException e => e.Embed.ToEmbedBuilder(),
					InteractionCommandError.Exception when execute.Exception.InnerException is Exception e => e.ToEmbedBuilder(),

					InteractionCommandError.Unsuccessful => EmbedHelper.CreateAlertEmbed("Could not perform command.", result.ErrorReason),
					_ => EmbedHelper.CreateAlertEmbed("An unhandled error occurred.", result.ErrorReason),
				};

				//

				if (interaction is SocketMessageComponent message)
				{
					await message.Channel.SendMessageAsync(embed: embed.Build());
				} else await context.User.SendMessageAsync(embed: embed.Build());


				Logger.Error("An interaction found an error. The code {code} resulted with the message {message}", result.Error, result.ErrorReason);
			}
		}
	}
}
