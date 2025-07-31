using Microsoft.AspNetCore.DataProtection;
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
namespace UserAPI.Repositories
{
    public class UserRepository : UserDbContext, IUserRepository
    {

        private readonly IConfiguration _configuration;

        public UserRepository(DbContextOptions<UserDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await Users.ToListAsync();
        }

        public async Task CreateUser(User user)
        {
            await Users.AddAsync(user);
            var userRole = await Roles.FirstAsync(x => x.Name == "user");
            userRole.Users.Add(user);
            await SaveChangesAsync();
        }

        public async Task DeleteUser(Guid id)
        {
            var user = await Users.FindAsync(id);
            Users.Remove(user);
            await SaveChangesAsync();

        }

        public async Task<User> GetUser(Guid id)
        {
            var user = await Users.FindAsync(id);

            if (user == null)
            {
                throw new Exception();
            }

            return user;
        }

        public async Task<User> GetUserByUsername(string username)
        {
            var user = await Users.FirstOrDefaultAsync(u => u.Username == username);
            return user;
        }

        //Нови методи за генериране на token-и.

        public async Task<TokenResponseDTO> CreatetokenResponse(User user)
        {
            return new TokenResponseDTO { AccessToken = CreateToken(user).Result, RefreshToken = await GenerateAndSaveRefreshtokenAsync(user) };
        }

        private async Task<string> CreateToken(User user)
        {
            // loading the roles and if there is an admin role it is what is send in the token, can be changed later!
            await Users.Entry(user).Collection(x => x.Roles).LoadAsync();

            var role = "user";

            if (user.Roles.Any(x=> x.Name == "admin"))
            {
                role = "admin";
            }
            

            var claims = new List<Claim>
            {

                new Claim(ClaimTypes.Name,user.Username),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Role,role)
            };


            var key = new SymmetricSecurityKey(Convert.FromBase64String(_configuration.GetValue<string>("AppSettings:Token")));

            var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(

                issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
                audience: _configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds

            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

        }

        private string GenerateRefreshtoken()
        {

            var randomNumber = new byte[32];

            using var rng = RandomNumberGenerator.Create();

            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);

        }

        private async Task<string> GenerateAndSaveRefreshtokenAsync(User user)
        {

            var refreshToken = GenerateRefreshtoken();

            user.RefreshToken = refreshToken;

            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await SaveChangesAsync();

            return refreshToken;

        }

        private async Task<User?> ValidateRefreshtokenAsync(Guid userId, string refreshToken)
        {

            var user = await Users.FindAsync(userId);

            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {

                return null;

            }

            return user;

        }

        public async Task<TokenResponseDTO> RefreshTokenAsync(RefreshTokenRequestDTO request)
        {
            var user = await ValidateRefreshtokenAsync(request.UserId, request.RefreshToken);

            if (user is null)
            {
                return null;

            }

            return await CreatetokenResponse(user);



        }

    }
}
