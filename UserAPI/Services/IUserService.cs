
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
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

        Task<string> ChangeUsername(string oldUsername, string newUsername, string email, string password);

        Task<bool> ForgotPassword(string email, string baseUrl);
        Task<bool> ResetPassword(string token, string newPassword);


    }
}
