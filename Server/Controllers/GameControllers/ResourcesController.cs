using Microsoft.AspNetCore.Mvc;
using Core.Game.Components;
using SpaceServer.Services.Game;

namespace SpaceServer.Controllers.GameControllers
{

	[ApiController]
    [Route("api/game/resource")]
    internal class ResourcesController : InternalController
    {
		private readonly ResourceService _resourceService;

		public ResourcesController(ResourceService resourceService)
		{
			_resourceService = resourceService;
		}


		[HttpGet]
		public async Task<List<Resource>> Get() =>
			await _resourceService.GetAsync();


		[HttpGet("{id}")]
		public async Task<ActionResult<Resource>> Get(string id)
		{
			var find = await _resourceService.GetAsync(id);

			if (find is null)
				return NotFound();

			return find;
		}

		[HttpPost]
		public async Task<IActionResult> Post(Resource resource)
		{
			await _resourceService.CreateAsync(resource);
			return CreatedAtAction(nameof(Post), new { id = resource.Id }, resource);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Put(string id, Resource resource)
		{
			var existing = await _resourceService.GetAsync(id);
			if (existing is null)
				return NotFound();

			await _resourceService.UpdateAsync(id, resource);

			return CreatedAtAction(nameof(Put), new { id = resource.Id }, resource);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(string id)
		{
			var existing = await _resourceService.GetAsync(id);
			if (existing is null)
				return NotFound();

			await _resourceService.RemoveAsync(existing);
			return NoContent();
		}
	}
}
