using SpaceCore.Game.Entities.Buildables.Buildings;
using SpaceServer.Database.Game;

namespace SpaceServer.Services.Game
{
    internal class RecipeBuildingService(GameDataContext context) : GenericDbService<GameDataContext, string, RecipeBuildingFactory>(context)
	{

		public async Task<List<RecipeBuildingFactory>> GetAsync() => await base.GetListAsync(_context.RecipeBuildingFactories);
	}
}
