using Microsoft.EntityFrameworkCore;
using UserAPI.Models;
namespace UserAPI.Repositories
{
    public class UserRepository : UserDbContext, IUserRepository
    {
        public UserRepository(DbContextOptions<UserDbContext> options) : base(options) { }

        public async Task<List<User>> GetAllUsers()
        {
            return await Users.ToListAsync();
        }
    }
}
