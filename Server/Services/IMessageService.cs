using Server.Models;

namespace Server.Services
{
    /// <summary>
    /// Интерфейс для сервиса сообщений с методами для отправки и получения сообщений
    /// </summary>
    public interface IMessageService
    {
        public Task<Message> GetMessageAsync(Guid messageId);

        public Task<List<Message>> GetMessagesAsync(Guid senderId, Guid receiverId);

        public Task<Message> SendMessageAsync(Guid senderId, Guid receiverId, string text);
    }
}
