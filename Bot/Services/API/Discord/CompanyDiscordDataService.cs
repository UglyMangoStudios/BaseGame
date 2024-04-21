using SpaceCore.Data.Discord;

namespace SpaceDiscordBot.Services.API.Discord
{
	internal class CompanyDiscordDataService(HttpService httpService) 
		: GenericApiObjectRetriever<ulong, CompanyDiscordData>(httpService, "discord/company")
	{
		protected override Func<CompanyDiscordData, ulong> ObjectIdPredicate => data => data.GuildId;

		public new Task<HttpResult<bool>> DeleteAsync(ulong id)
		{
			return base.DeleteAsync(id);
		}

		public new Task<HttpResult<List<CompanyDiscordData>>> GetAllAsync()
		{
			return base.GetAllAsync();
		}

		public new Task<HttpResult<CompanyDiscordData>> GetAsync(ulong id)
		{
			return base.GetAsync(id);
		}

		public new Task<HttpResult<CompanyDiscordData>> CreateAsync(CompanyDiscordData @object)
		{
			return base.CreateAsync(@object);
		}

		public new Task<HttpResult<CompanyDiscordData>> UpdateAsync(ulong id, CompanyDiscordData @object)
		{
			return base.UpdateAsync(id, @object);
		}
	}
}
