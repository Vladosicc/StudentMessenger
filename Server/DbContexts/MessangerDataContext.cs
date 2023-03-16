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
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Сообщения
        /// </summary>
        public DbSet<Message> Messages { get; set; }
    }
}
