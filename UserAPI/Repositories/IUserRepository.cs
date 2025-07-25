using UserAPI.Models;

namespace UserAPI.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUsers();
    }
}
