using Microsoft.AspNetCore.Mvc;
using SpaceDiscordBot.http.Modals;
using SpaceDiscordBot.http.Services;

namespace SpaceDiscordBot.http.Controllers
{

    [ApiController]
    [Route("api/discord/notify")]
    internal class NotificationController : Controller
	{
        private readonly NotificationService _notificationService;

        public NotificationController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }


        [HttpPost]
        public async Task<bool> PostNotification(NotifyBundle bundle) =>
            await _notificationService.NotifyAsync(bundle);


    }
}
