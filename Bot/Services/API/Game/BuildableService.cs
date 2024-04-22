using Core.Game.Entities.Buildables;
using Core.Game.Entities.Buildables.Buildings;

namespace SpaceDiscordBot.Services.API.Game
{
	internal class BuildableService(HttpService httpService) 
		: GenericApiObjectRetriever<string, BuildableFactory>(httpService, "game/buildings")
	{
		protected override Func<BuildableFactory, string> ObjectIdPredicate => factory => factory.Id;

		public new Task<HttpResult<List<BuildableFactory>>> GetAllAsync()
		{
			return base.GetAllAsync();
		}

		public new Task<HttpResult<BuildableFactory>> GetAsync(string id)
		{
			return base.GetAsync(id);
		}

		public async Task<BuildableFactory> AsFactory(Buildable buildable)
		{
			var result = await GetAsync(buildable.FactoryId);
			result.EnsureSuccess(nameof(BuildableService));

			return result.Value!;
		}
	}
}