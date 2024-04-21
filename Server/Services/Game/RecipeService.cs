using SpaceCore.Game.Components;
using SpaceServer.Database.Game;

namespace SpaceServer.Services.Game
{
    internal class RecipeService(GameDataContext context) : GenericDbService<GameDataContext, string, Recipe>(context)
	{

		public async Task<List<Recipe>> GetAsync() => await base.GetListAsync(_context.Recipes);
	}
}
