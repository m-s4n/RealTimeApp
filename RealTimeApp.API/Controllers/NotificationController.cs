using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RealTimeApp.API.SignalR.Hubs;

namespace RealTimeApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        //hangi hub'ı kullanıcaz türü verilir
        private readonly IHubContext<MyHub> _hubContext;

        public NotificationController(IHubContext<MyHub> hubContext)
        {
            _hubContext = hubContext;
        }


        // Hub dışından mesaj bildirim gönderebiliriz.
        [HttpGet("{teamCount}")]
        public async Task SetTeamCount(int teamCount)
        {
            MyHub.TeamCount = teamCount;
            await _hubContext.Clients.All.SendAsync("Notify", $"Arkadaşlar takım {teamCount} kişi olacaktır.");
        }
    }
}
