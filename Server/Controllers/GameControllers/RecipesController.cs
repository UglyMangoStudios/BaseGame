using Microsoft.AspNetCore.Mvc;
using SpaceCore.Game.Components;
using SpaceServer.Services.Game;

namespace SpaceServer.Controllers.GameControllers
{

	[ApiController]
	[Route("api/game/recipes")]
	internal class RecipesController : InternalController
	{

		private readonly RecipeService _recipeService;

		public RecipesController(RecipeService recipeService)
		{
			_recipeService = recipeService;
		}


		[HttpGet]
		public async Task<List<Recipe>> Get() =>
			await _recipeService.GetAsync();


		[HttpGet("{id}")]
		public async Task<ActionResult<Recipe>> Get(string id)
		{
			var building = await _recipeService.GetAsync(id);

			if (building is null)
				return NotFound();

			return building;
		}

		[HttpPost]
		public async Task<IActionResult> Post(Recipe recipe)
		{
			await _recipeService.CreateAsync(recipe);
			return CreatedAtAction(nameof(Post), new { id = recipe.Id }, recipe);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Put(string id, Recipe recipe)
		{
			var existing = await _recipeService.GetAsync(id);
			if (existing is null)
				return NotFound();

			await _recipeService.UpdateAsync(id, recipe);

			return CreatedAtAction(nameof(Put), new { id = recipe.Id }, recipe);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(string id)
		{
			var existing = await _recipeService.GetAsync(id);
			if (existing is null)
				return NotFound();

			await _recipeService.RemoveAsync(existing);
			return NoContent();
		}

	}
}
