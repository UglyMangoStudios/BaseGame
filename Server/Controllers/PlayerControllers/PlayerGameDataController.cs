using Microsoft.AspNetCore.Mvc;
using SpaceCore.Data.Saves;
using SpaceServer.Services.Game.Generation;
using SpaceServer.Services.Player;

namespace SpaceServer.Controllers.PlayerControllers
{

	[ApiController]
	[Route("api/game/player")]
	internal class PlayerGameDataController : InternalController
	{
		private PlayerGameDataService _playerGameDataService { get; }

		private SystemGenerationService _systemGenerationService { get; }

		public PlayerGameDataController(PlayerGameDataService playerGameDataService, SystemGenerationService systemGenerationService)
		{
			_playerGameDataService = playerGameDataService;
			_systemGenerationService = systemGenerationService;
		}


		[HttpGet("{userId:ulong}")]
		public async Task<ActionResult<PlayerGameData>> Get(ulong userId)
		{
			//var playerData = _playerGameDataService.GetByProxy(p => p.UserId == userId);
			var playerData = await _playerGameDataService.GetAsync(userId);

			if (playerData is null)
				return NotFound();


			return playerData;
		}


		[HttpPost("{user_id:ulong}")]
		public async Task<IActionResult> Post(ulong user_id)
		{
			var playerGameData = new PlayerGameData(user_id); 

			//Auto-generate the home system
			playerGameData.HomeSystem = _systemGenerationService.GenerateHomeSystem(playerGameData.Seed);

			await _playerGameDataService.CreateAsync(playerGameData);
			return CreatedAtAction(nameof(Post), new { id = playerGameData.UserId }, playerGameData);
		}


		[HttpPut("{user_id:ulong}")]
		public async Task<IActionResult> Put(ulong user_id, PlayerGameData playerGameData)
		{
			var existing = await _playerGameDataService.GetAsync(playerGameData.UserId);
			if (existing is null)
				return NotFound();

			await _playerGameDataService.UpdateAsync(user_id, playerGameData);

			return CreatedAtAction(nameof(Get), new { id = playerGameData.UserId }, playerGameData);
		}


		[HttpDelete("{user_id:ulong}")]
		public async Task<IActionResult> Delete(ulong user_id)
		{
			var existing = await _playerGameDataService.GetAsync(user_id);
			if (existing is null)
				return NotFound();

			await _playerGameDataService.RemoveAsync(existing);
			return NoContent();
		}
	}
}
