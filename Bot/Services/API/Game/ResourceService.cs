using SpaceCore.Game.Components;

namespace SpaceDiscordBot.Services.API.Game
{
	internal class ResourceService(HttpService httpService) 
        : GenericApiObjectRetriever<string, Resource>(httpService, "game/resource")
    {
        protected override Func<Resource, string> ObjectIdPredicate => resource => resource.Id;

		public new Task<HttpResult<List<Resource>>> GetAllAsync()
        {
            return base.GetAllAsync();
        }

        public new Task<HttpResult<Resource>> GetAsync(string id)
        {
            return base.GetAsync(id);
        }
    }
}
