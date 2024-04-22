using Core.Data.Discord;
using SpaceServer.Database.Discord;



//Followed https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-7.0&tabs=visual-studio
//as a guide

namespace SpaceServer.Services.Player
{
    internal class PlayerDiscordDataService(DiscordDataContext context) : GenericDbService<DiscordDataContext, ulong, PlayerDiscordData>(context)
	{

		public async Task<List<PlayerDiscordData>> GetAsync() => await base.GetListAsync(_context.PlayerDiscordData);

		public List<PlayerDiscordData> GetMatch(Func<PlayerDiscordData, bool> predicate) 
			=> _context.PlayerDiscordData.Where(predicate).ToList();


	}
}
