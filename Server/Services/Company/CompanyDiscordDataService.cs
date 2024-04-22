using Core.Data.Discord;
using SpaceServer.Database.Discord;

namespace SpaceServer.Services.Company
{
    internal class CompanyDiscordDataService(DiscordDataContext context) : GenericDbService<DiscordDataContext, ulong, CompanyDiscordData>(context)
    {
		public async Task<List<CompanyDiscordData>> GetAsync() => await base.GetListAsync(_context.CompanyDiscordData);


	}
}
