using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.DbContexts
{
    public class MessangerDataContext : DbContext
    {
        public MessangerDataContext(DbContextOptions options) : base(options)
        {

        }

        /// <summary>
        /// Пользователи
        /// </summary>
        DbSet<User> Users { get; set; }

        /// <summary>
        /// Сообщения
        /// </summary>
        DbSet<Message> Messages { get; set; }
    }
}
