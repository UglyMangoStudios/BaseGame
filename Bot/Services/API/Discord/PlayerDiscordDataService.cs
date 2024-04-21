using SpaceCore.Data.Discord;

namespace SpaceDiscordBot.Services.API.Discord
{
	internal class PlayerDiscordDataService(HttpService httpService) 
		: GenericApiObjectRetriever<ulong, PlayerDiscordData>(httpService, "discord/player")
	{
		protected override Func<PlayerDiscordData, ulong> ObjectIdPredicate => data => data.UserId;

		public new Task<HttpResult<bool>> DeleteAsync(ulong id)
		{
			return base.DeleteAsync(id);
		}

		public new Task<HttpResult<PlayerDiscordData>> GetAsync(ulong id)
		{
			return base.GetAsync(id);
		}

		public new Task<HttpResult<PlayerDiscordData>> CreateAsync(PlayerDiscordData @object)
		{
			return base.CreateAsync(@object);
		}

		public new Task<HttpResult<PlayerDiscordData>> UpdateAsync(ulong id, PlayerDiscordData @object)
		{
			return base.UpdateAsync(id, @object);
		}

		public Task<HttpResult<List<PlayerDiscordData>>> GetForCompany(ulong guildId)
		{
			string path = HttpService.AppendPath(ApiPath, "company", guildId);
			return HttpService.GetAsync<List<PlayerDiscordData>>(path);
		}
	}
}
