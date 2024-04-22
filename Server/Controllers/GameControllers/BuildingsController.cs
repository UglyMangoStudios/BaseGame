using Microsoft.AspNetCore.Mvc;
using Core.Game.Entities.Buildables.Buildings;
using SpaceServer.Services.Game;

namespace SpaceServer.Controllers.GameControllers
{

	[ApiController]
	[Route("api/game/buildings")]
	internal class BuildingsController : InternalController
	{

		private readonly RecipeBuildingService _buildingsService;

		public BuildingsController(RecipeBuildingService buildingsService)
		{
			_buildingsService = buildingsService;
		}


		[HttpGet]
		public async Task<List<RecipeBuildingFactory>> Get() =>
			await _buildingsService.GetAsync();


		[HttpGet("{id}")]
		public async Task<ActionResult<RecipeBuildingFactory>> Get(string id)
		{
			var building = await _buildingsService.GetAsync(id);

			if (building is null)
				return NotFound();

			return building;
		}

		[HttpPost]
		public async Task<IActionResult> Post(RecipeBuildingFactory buildingBaseFactory)
		{
			await _buildingsService.CreateAsync(buildingBaseFactory);
			return CreatedAtAction(nameof(Post), new { id = buildingBaseFactory.Id }, buildingBaseFactory);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Put(string id, RecipeBuildingFactory buildingBaseFactory)
		{
			var existing = await _buildingsService.GetAsync(id);
			if (existing is null)
				return NotFound();

			await _buildingsService.UpdateAsync(id, buildingBaseFactory);

			return CreatedAtAction(nameof(Put), new { id = buildingBaseFactory.Id }, buildingBaseFactory);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(string id)
		{
			var existing = await _buildingsService.GetAsync(id);
			if (existing is null)
				return NotFound();

			await _buildingsService.RemoveAsync(existing);
			return NoContent();
		}

	}
}
