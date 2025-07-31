
using Azure.Core;
using UserAPI.DTOs;
using UserAPI.Models;

namespace UserAPI.Services
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsers();
        Task<User> CreateUser(CreateUserDTO user);

        Task<User> GetUser(Guid id);
        Task DeleteUser(Guid id);

        Task<TokenResponseDTO> LoginAsync(LoginUserDTO request);

        Task<TokenResponseDTO> RefreshTokenAsync(RefreshTokenRequestDTO request);

    }
}
