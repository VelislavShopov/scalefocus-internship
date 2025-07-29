
using UserAPI.Models;
using UserAPI.DTOs;

namespace UserAPI.Services
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsers();
        Task<User> CreateUser(CreateUserDTO user);

        Task<User> GetUser(Guid id);
        Task DeleteUser(Guid id);

        Task<bool> LoginAsync(LoginUserDTO request);

    }
}
