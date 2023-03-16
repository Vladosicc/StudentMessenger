using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.Hubs;

namespace Server.Controllers
{
    public class ChatController : Controller
    {

        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string recipientId, string message)
        {
            var senderId = User.Identity.Name;

            // Сообщение может быть сохранено в базе данных, используя Entity Framework Core

            await _hubContext.Clients.User(recipientId).SendAsync("ReceiveMessage", senderId, message);

            return Ok();
        }
    }
}
