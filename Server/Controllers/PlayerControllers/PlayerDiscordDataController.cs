using Microsoft.AspNetCore.Mvc;
using SpaceCore.Data.Discord;
using SpaceServer.Services.Player;

namespace SpaceServer.Controllers.PlayerControllers
{

	[ApiController]
	[Route("api/discord/player")]
	internal class PlayerDiscordDataController : InternalController
	{
		private readonly PlayerDiscordDataService _playerDiscordDataService;

		public PlayerDiscordDataController(PlayerDiscordDataService playerDiscordDataService)
		{
			_playerDiscordDataService = playerDiscordDataService;
		}


		[HttpGet]
		public async Task<List<PlayerDiscordData>> Get() =>
			await _playerDiscordDataService.GetAsync();

		[HttpGet("{userId:ulong}")]
		public async Task<ActionResult<PlayerDiscordData>> Get(ulong userId)
		{
			var playerData = await _playerDiscordDataService.GetAsync(userId);

			if (playerData is null)
				return NotFound();

			return playerData;
		}

		[HttpGet("company/{guildId:ulong}")]
		public List<PlayerDiscordData> GetWithinCompany(ulong guildId)
		{
			return _playerDiscordDataService.GetMatch(player => player.GuildId == guildId);
		}



		[HttpPost]
		public async Task<IActionResult> Post(PlayerDiscordData playerDiscordData)
		{
			await _playerDiscordDataService.CreateAsync(playerDiscordData);
			return CreatedAtAction(nameof(Get), new { id = playerDiscordData.GuildId }, playerDiscordData);
		}


		[HttpPut("{user_id:ulong}")]
		public async Task<IActionResult> Put(ulong user_id, PlayerDiscordData playerDiscordData)
		{
			var existing = await _playerDiscordDataService.GetAsync(playerDiscordData.UserId);
			if (existing is null)
				return NotFound();

			await _playerDiscordDataService.UpdateAsync(user_id, playerDiscordData);

			return CreatedAtAction(nameof(Get), new { id = playerDiscordData.GuildId }, playerDiscordData);
		}

		[HttpDelete("{user_id:ulong}")]
		public async Task<IActionResult> Delete(ulong user_id)
		{
			var existing = await _playerDiscordDataService.GetAsync(user_id);
			if (existing is null)
				return NotFound();

			await _playerDiscordDataService.RemoveAsync(existing);
			return NoContent();
		}
	}
}
