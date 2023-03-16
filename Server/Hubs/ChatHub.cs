using Microsoft.AspNetCore.SignalR;
using Server.Services;

namespace Server.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IUserService _userService;
        private readonly IMessageService _messageService;

        public ChatHub(IUserService userService, IMessageService messageService)
        {
            _userService = userService;
            _messageService = messageService;
        }

        public async Task SendMessage(Guid receiverId, string text)
        {
            var senderName = Context.User.Identity.Name;
            var sender = await _userService.GetUserByNameAsync(senderName);
            var receiver = await _userService.GetUserAsync(receiverId);

            if (sender == null || receiver == null)
            {
                return;
            }

            var message = await _messageService.SendMessageAsync(sender.Id, receiver.Id, text);

            await Clients.User(receiver.Name).SendAsync("ReceiveMessage", message);
        }

        public async Task GetMessages(Guid receiverId)
        {
            var senderName = Context.User.Identity.Name;
            var sender = await _userService.GetUserByNameAsync(senderName);
            var receiver = await _userService.GetUserAsync(receiverId);

            if (sender == null || receiver == null)
            {
                return;
            }

            var messages = await _messageService.GetMessagesAsync(sender.Id, receiver.Id);

            await Clients.User(sender.Name).SendAsync("ReceiveMessages", messages);
        }
    }
}
