using Microsoft.EntityFrameworkCore;
using Server.DbContexts;
using Server.Models;

namespace Server.Services
{
    public class MessageService : IMessageService
    {
        private readonly MessangerDataContext _dbContext;

        public MessageService(MessangerDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Message> GetMessageAsync(Guid messageId)
        {
            return await _dbContext.Messages.FindAsync(messageId);
        }

        public async Task<List<Message>> GetMessagesAsync(Guid senderId, Guid receiverId)
        {
            return await _dbContext.Messages
                .Where(m => (m.SenderId == senderId && m.ReceiverId == receiverId)
                    || (m.SenderId == receiverId && m.ReceiverId == senderId))
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
        }

        public async Task<Message> SendMessageAsync(Guid senderId, Guid receiverId, string text)
        {
            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Text = text,
                Timestamp = DateTime.UtcNow
            };

            await _dbContext.Messages.AddAsync(message);
            await _dbContext.SaveChangesAsync();

            return message;
        }
    }
}
