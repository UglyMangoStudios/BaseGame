using Core.Data.Saves;

namespace SpaceDiscordBot.Services.API.Game
{
	internal class PlayerGameDataService(HttpService httpService)
		: GenericApiObjectRetriever<ulong, PlayerGameData>(httpService, "game/player")
    {
		protected override Func<PlayerGameData, ulong> ObjectIdPredicate => data => data.UserId;

		public new Task<HttpResult<bool>> DeleteAsync(ulong id)
        {
            return base.DeleteAsync(id);
        }

        public new Task<HttpResult<PlayerGameData>> GetAsync(ulong id)
        {
            return base.GetAsync(id);
        }

        public async Task<HttpResult<PlayerGameData>> CreateAsync(ulong userId)
        {
            string path = BuildPath(userId);
            return await HttpService.PostAsync<PlayerGameData>(path);
        }

		public async Task<HttpResult<PlayerGameData>> UpdateAsync(PlayerGameData @object)
		{
			return await base.UpdateAsync(@object.UserId, @object);
		}
	}
}
