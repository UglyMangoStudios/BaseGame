using Discord;
using Discord.Rest;
using Discord.WebSocket;
using SpaceCore.Data.Discord;
using SpaceCore.Types;
using SpaceDiscordBot.Services.API.Discord;
using SpaceDiscordBot.Frameworks.Exceptions;
using SpaceDiscordBot.Modules;

namespace SpaceDiscordBot.Services.Discord
{

    /// <summary>
    /// Static class that manages the behavior of guilds with their companies. 
    /// </summary>
    internal class GuildService(ChannelService channelService)
    {
        //Strings that surround categories that designated bot controlled categories
        private const string CATEGORY_PREFIX = "◈";
        private const string CATEGORY_SUFFIX = "◈";


        private ChannelService _channelService { get; } = channelService;


        /// <summary>
        /// Checks to see if a category's name is managed by this bot.
        /// </summary>
        /// <param name="category">The physical category</param>
        /// <returns>True if it matches</returns>
        public bool CategoryMatchesBotManagement(SocketCategoryChannel? category)
        {
            if (category is null) return false;

            string categoryName = category.Name;

            return categoryName[..CATEGORY_PREFIX.Length] == CATEGORY_PREFIX; //TODO: Check the ending sequence too
        }

        /// <summary>
        /// Deletes all of the threads that lives in a channel. Assigns async operations to a <see cref="MultiTask"/> field
        /// </summary>
        /// <param name="channel">The target channel</param>
        /// <returns>A <see cref="MultiTask"/> that may need to be awaited</returns>
        public async Task DeleteThreadsFromChannel(SocketGuild guild, ITextChannel channel)
        {
            MultiTask multiTask = new();

			foreach (var thread in guild.ThreadChannels.Where(thread => thread.ParentChannel == channel))
			{
                try
                {
					multiTask += thread.DeleteAsync();
				} catch (Exception e)
                {
                    //TODO: Log
                    continue;
                }
                
			}

            await multiTask.WaitAll();
        }

        /// <summary>
        /// Deletes company roles, channels, threads, and other features that are managed by this application
        /// </summary>
        /// <param name="guild">The target guild object to access and modify</param>
        /// <param name="companyDiscordData">The data object of this guild</param>
        /// <returns>An async task to await</returns>
        public async Task CleanCompany(SocketGuild guild, CompanyDiscordData companyDiscordData)
        { 
            MultiTask multiTask = new();

            //Gets existing categories and deletes any that match the preset 
            var categories = guild.CategoryChannels;
            foreach (var category in categories)
            {
                if (CategoryMatchesBotManagement(category))
                {
                    foreach (var channel in category.Channels)
                    {
                        if (channel is ITextChannel textChannel)
                        {
                            multiTask += DeleteThreadsFromChannel(guild, textChannel);
                        }
                        multiTask += channel.DeleteAsync();
                    }
                    multiTask += category.DeleteAsync();
                }
            }

            //TODO: Delete the roles, too

            await multiTask.WaitAll();
        }

        /// <summary>
        /// Takes the guild and company data and build categories and channels that match a pre-set template. 
        /// <br/> Note: Does not save the company's data.
        /// </summary>
        /// <param name="guild">The physical guild to manipulate</param>
        /// <param name="data">The company data to assign data</param>
        public async Task CleanAndDeployCompany(SocketGuild guild, CompanyDiscordData companyDiscordData)
        {
            MultiTask multiTask = new();

            multiTask += CleanCompany(guild, companyDiscordData);
            multiTask += DeployCompany(guild, companyDiscordData);

            await multiTask.WaitAll();
        }

        /// <summary>
        /// The main method that creates the various categories and channels required. Does not POST data.
        /// </summary>
        /// <param name="guild">The physical guild</param>
        /// <param name="companyDiscordData">The <see cref="CompanyData"/> object that gets edited with each new channel.</param>
        /// <returns></returns>
        public async Task DeployCompany(SocketGuild guild, CompanyDiscordData companyDiscordData)
        {
            //Combines all of the async tasks into one to return.
            //TODO: Handle all of these methods within a nonblocking thread.
            MultiTask multiTask = new();

            //Gets the localized name of the category and creates a new category with that name
            string companyCategoryName = $"{CATEGORY_PREFIX} Company Channels {CATEGORY_SUFFIX}";

            //The newly created category channel
            RestCategoryChannel companyCategoryChannel = await guild.CreateCategoryChannelAsync(companyCategoryName);
            companyDiscordData.Channels.CompanyCenterCategory = companyCategoryChannel.Id;

            //Create the player games channel
            string playerGamesCategoryName = $"{CATEGORY_PREFIX} Player Channels {CATEGORY_SUFFIX}";

            RestCategoryChannel playersCategoryChannel = await guild.CreateCategoryChannelAsync(playerGamesCategoryName);
            multiTask += playersCategoryChannel.ModifyAsync(properties =>
            {
                //Ensures the players category is right below the company center category 
                properties.Position = companyCategoryChannel.Position + 1;
            });

            companyDiscordData.Channels.PlayerGamesCategory = playersCategoryChannel.Id;

            //Iterates through all of the company center channels and assigns the data
            foreach (ChannelScope scope in _channelService.GetCompanyChannelScopes())
            {
                multiTask += Task.Run(async () =>
                {
                    var channelData = _channelService.GetChannelData(scope);
                    var textChannel = await guild.CreateTextChannelAsync(channelData.Name, tcp =>
                    {
                        tcp.CategoryId = companyCategoryChannel.Id;
                    });

                    ulong channelId = textChannel.Id;
                    //For each channel, it has a specific preset type. ASSign the id to the corresponding type within 

                    _channelService.SetCompanyChannelId(scope, companyDiscordData, channelId);
                });

            }

            await multiTask.WaitAll();
        }

        /// <summary>
        /// Deploys a player to a target company. This means player channels are created and permissions set so a player can play
        /// </summary>
        /// <param name="guild">The guild the user is joining</param>
        /// <param name="user">The user object that represents the client in Discord</param>
        /// <param name="playerData">The user's data</param>
        /// <returns>An async task to await. If awaited, a boolean representing the success status is returned.</returns>
        public async Task DeployPlayerToCompany(SocketGuild guild, SocketGuildUser user, CompanyDiscordData companyDiscordData, PlayerDiscordData playerDiscordData)
        {
            if (user.IsBot)
                throw new EmbedException(LogSeverity.Critical, nameof(DeployPlayerToCompany), "User is a bot. Refusing deployment");

            MultiTask multiTask = new();

            var lobbyChannelData = _channelService.GetChannelData(ChannelScope.PlayerLobbyChannel);

            string playerName = user.Username;
            string channelName = string.Format(lobbyChannelData.Name, playerName);

            RestTextChannel publicLobbyChannel = await guild.CreateTextChannelAsync(channelName, tcp => { tcp.CategoryId = companyDiscordData.Channels.PlayerGamesCategory; });

			playerDiscordData.Channels = new()
			{
				LobbyChannelId = publicLobbyChannel.Id
			};

			//Iterate through each of the player channels. Create these channels as threads.
			//TODO: Find a way to properly order everything
			foreach (ChannelScope scope in _channelService.GetPlayerThreadScopes())
            {
                var channelData = _channelService.GetChannelData(scope);
                multiTask.Run(async () =>
                {
                    RestThreadChannel threadChannel = await publicLobbyChannel.CreateThreadAsync(
					    channelData.Name,
                        ThreadType.PrivateThread,
                        autoArchiveDuration: ThreadArchiveDuration.OneWeek,
                        invitable: false
                    );

					//Lock the channel if the player only has reading rights  
					await threadChannel.ModifyAsync(properties =>
					{
						properties.Locked = !channelData.UserWrite;
					});

					//TODO: Only add the user once they unlock the channel
					await threadChannel.AddUserAsync(user);

                    //Assign the channel's id to the corresponding id in the player's channel data
                    ulong channelId = threadChannel.Id;
                    _channelService.SetPlayerChannelId(scope, playerDiscordData, channelId);
                });
            }

            //Auto register the player
            companyDiscordData.RegisteredPlayers.Add(user.Id);
            playerDiscordData.GuildId = guild.Id;

            await multiTask.WaitAll();
        }
	}
}
