using Microsoft.EntityFrameworkCore;
using Server.DbContexts;
using Server.Models;

namespace Server.Services
{
    public class UserService
    {
        private readonly MessangerDataContext _dbContext;

        public UserService(MessangerDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> CreateUserAsync(string name)
        {
            var user = new User
            {
                Name = name
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return user;
        }

        public async Task<User> GetUserAsync(Guid userId)
        {
            return await _dbContext.Users.FindAsync(userId);
        }

        public async Task<User> GetUserByNameAsync(string name)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Name == name);
        }
    }
}
