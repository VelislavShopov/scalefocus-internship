using EmailService;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserAPI.DTOs;
using UserAPI.Models;
using UserAPI.Repositories;

namespace UserAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        private readonly IConfiguration _configuration;

        private readonly IEmailSender _emailSender;

        private readonly ITokenRepository _tokenRepository;

        public UserService(IUserRepository userRepository, IConfiguration configuration, IEmailSender emailSender, ITokenRepository tokenRepository)
        {
            _userRepository = userRepository;
            _tokenRepository = tokenRepository;
            _configuration = configuration;
            _emailSender = emailSender;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _userRepository.GetAllUsers();
        }


        public async Task<User?> GetUser(Guid id)
        {
            return await _userRepository.GetUser(id);
        }

        // Create метод направен от Никола Гочев
        public async Task<User> CreateUser(CreateUserDTO user)
        {
            if (user == null) 
            {
                throw new ArgumentNullException("Please provide credentials.");
            }

            if (user.Password != user.ConfirmPassword)
            {
                throw new ArgumentException("Password and Confirm Password do not match.");
            }
              
              var existingEmail = await _userRepository.GetUserByEmail(user.Email);
            if (existingEmail != null)
            {
                throw new ArgumentException("A user with this email already exists.");
            }

            var existingUsername = await _userRepository.GetUserByUsername(user.Username);
            if (existingUsername != null)
            {
                throw new ArgumentException("A user with this username already exists.");
            }
            
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            string hashedPassword = new PasswordHasher<User>().HashPassword(newUser, user.Password);

            newUser.PasswordHash = hashedPassword;

            await _userRepository.CreateUser(newUser);
            return newUser;
        }

        public async Task DeleteUser(Guid id, Guid loggedUserId)
        {
            if (id != loggedUserId)
            {
                throw new UnauthorizedAccessException();
            }

            await _userRepository.DeleteUser(id);

        }

        public async Task<User> LoginAsync(LoginUserDTO request)
        {
            var user = await _userRepository.GetUserByUsername(request.Username);

            if (user == null)
            {
                throw new KeyNotFoundException();
            }

            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password)
                == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException();
            }

            return user;
        }

        public async Task<string> ChangeUsername(string oldUsername, string newUsername, string email, string password)
        {
            var user = await _userRepository.GetUserByUsername(oldUsername);
            if (user == null) return "User with the old username not found.";

            if (!string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
                return "Email does not match our records.";

            var passwordVerificationResult = new PasswordHasher<User>()
                .VerifyHashedPassword(user, user.PasswordHash, password);
            if (passwordVerificationResult == PasswordVerificationResult.Failed)
                return "Incorrect password.";

            var existingUser = await _userRepository.GetUserByUsername(newUsername);
            if (existingUser != null)
                return "New username is already taken.";

            user.Username = newUsername;
            await _userRepository.UpdateUser(user); 

            return "Username successfully changed.";
        }

        // Забравена парола
        public async Task<bool> ForgotPassword(string email, string baseUrl)
        {
            var user = await _userRepository.GetUserByEmail(email);
            if (user == null)
                return false;

            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var token = Convert.ToBase64String(tokenBytes);

            var passwordResetToken = new PasswordResetToken
            {
                UserId = user.Id,
                PasswordResetTokenValue = token,
                ExpiryTime = DateTime.UtcNow.AddMinutes(5)
            };
            
            await _tokenRepository.CreatePasswordResetToken(passwordResetToken);

            var callbackUrl = $"{baseUrl}?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";

            var message = new Message(
                new[] { user.Email },
                "Password Reset",
                $"Please reset your password using this link: {callbackUrl}"
            );
            await _emailSender.SendEmailAsync(message);

            return true;
        }

        // Ресет на парола
        public async Task<bool> ResetPassword(string token, string newPassword)
        {
            var decodedToken = Uri.UnescapeDataString(token);

            var passwordResetToken = await _tokenRepository.GetPasswordResetTokenByValue(decodedToken);

            var user = await GetUser(passwordResetToken.UserId);
            if (user == null)
                return false;

            var passwordHasher = new PasswordHasher<User>();
            var hashedPassword = passwordHasher.HashPassword(user, newPassword);

            user.PasswordHash = hashedPassword;

            _tokenRepository.DeletePasswordResetToken(passwordResetToken);

            await _userRepository.UpdateUser(user);

            return true;
        }
    }
}


