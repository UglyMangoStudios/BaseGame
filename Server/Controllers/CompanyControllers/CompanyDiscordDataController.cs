using Microsoft.AspNetCore.Mvc;
using Core.Data.Discord;
using SpaceServer.Services.Company;

namespace SpaceServer.Controllers.CompanyControllers
{

	[ApiController]
    [Route("api/discord/company")]
    internal class CompanyDiscordDataController : InternalController
    {
        private readonly CompanyDiscordDataService _companyDiscordDataService;

        public CompanyDiscordDataController(CompanyDiscordDataService companyDiscordDataService)
        {
            _companyDiscordDataService = companyDiscordDataService;
        }


        [HttpGet]
        public async Task<List<CompanyDiscordData>> Get() =>
            await _companyDiscordDataService.GetAsync();

        [HttpGet("{userId:ulong}")]
        public async Task<ActionResult<CompanyDiscordData>> Get(ulong userId)
        {
            var playerData = await _companyDiscordDataService.GetAsync(userId);

            if (playerData is null)
                return NotFound();

            return playerData;
        }

        [HttpPost]
        public async Task<IActionResult> Post(CompanyDiscordData companyDiscordData)
        {
            await _companyDiscordDataService.CreateAsync(companyDiscordData);
            return CreatedAtAction(nameof(Get), new { id = companyDiscordData.GuildId }, companyDiscordData);
        }

        [HttpPut("{guild_id:ulong}")]
        public async Task<IActionResult> Put(ulong guild_id, CompanyDiscordData companyDiscordData)
        {
            var existing = await _companyDiscordDataService.GetAsync(guild_id);
            if (existing is null)
                return NotFound();

            await _companyDiscordDataService.UpdateAsync(guild_id, companyDiscordData);

            return CreatedAtAction(nameof(Get), new { id = companyDiscordData.GuildId }, companyDiscordData);
        }

        [HttpDelete("{guild_id:ulong}")]
        public async Task<IActionResult> Delete(ulong guild_id)
        {
            var existing = await _companyDiscordDataService.GetAsync(guild_id);
            if (existing is null)
                return NotFound();

            await _companyDiscordDataService.RemoveAsync(existing);
            return NoContent();
        }
    }
}
