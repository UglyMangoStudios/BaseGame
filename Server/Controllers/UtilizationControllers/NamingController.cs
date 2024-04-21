using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ObjectPool;
using SpaceServer.Services.Utilization;

namespace SpaceServer.Controllers.UtilizationControllers
{

	[ApiController]
	[Route("api/util/naming")]
	internal class NamingController(NamingService namingService) : InternalController
	{
		private NamingService NamingService { get; } = namingService;

		[HttpGet]
		public string[] Get() => NamingService.GetNames();


		[HttpGet("random")]
		public string GetRandom() => NamingService.RandomName();


		[HttpGet("random/{count:int}")]
		public string[] GetRandom(int count) => NamingService.RandomNames(count);


		[HttpPost]
		public async Task<IActionResult> PostName(string name)
		{
			await NamingService.AddName(name);
			return CreatedAtAction(nameof(PostName), name);
		}


		[HttpDelete("{name}")]
		public async Task<IActionResult> RemoveName(string name)
		{
			bool find = NamingService.NameExists(name);
			if (!find)
				return NotFound();

			await NamingService.RemoveName(name);
			return NoContent();
		}

	}
}
