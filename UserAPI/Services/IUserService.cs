
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

        Task<User?> GetUser(Guid id);
        Task DeleteUser(Guid id, Guid loggedUserId);

        Task<User> LoginAsync(LoginUserDTO request);

        Task<string> ChangeUsername(string oldUsername, string newUsername, string email, string password);

        Task ForgotPassword(string email);
        Task ResetPassword(string token,string email, string newPassword);


    }
}
