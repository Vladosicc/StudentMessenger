using Server.Models;

namespace Server.Services
{
    /// <summary>
    /// Интерфейс для сервиса пользователей с методами для создания, поиска и получения пользователей
    /// </summary>
    public interface IUserService
    {
        public Task<User> CreateUserAsync(string name);

        public Task<User> GetUserAsync(Guid userId);

        public Task<User> GetUserByNameAsync(string name);
    }
}
