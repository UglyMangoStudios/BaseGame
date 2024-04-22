using Core.Game.Components;
using SpaceServer.Database.Game;

namespace SpaceServer.Services.Game
{
    internal sealed class ResourceService(GameDataContext context) : GenericDbService<GameDataContext, string, Resource>(context)
    {

		public async Task<List<Resource>> GetAsync() => await base.GetListAsync(_context.Resources);

		public Resource? Get(string id) =>
			_context.Find<Resource>(id);

	}
}
