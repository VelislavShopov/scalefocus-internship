using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using UserAPI.Exceptions;
using UserAPI.Models;
using UserAPI.Repositories;

namespace UserAPI.Helpers
{
    public class JWTHelper : IJWTHelper
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;

        public JWTHelper(IHttpContextAccessor contextAccessor, IConfiguration configuration,IUserRepository userRepository) 
        {
            _contextAccessor = contextAccessor;
            _configuration = configuration;
            _userRepository = userRepository;
        }

        public Guid GetCurrentUserId()
        {
            var userIdString = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            Guid userId = Guid.Parse(userIdString);

            return userId;
        }

        public async Task<string> CreateAccessToken(User user, string audience)
        {
            // loading the roles and if there is an admin role it is what is send in the token, can be changed later!
            var roles = await _userRepository.GetRolesForUser(user);

            var role = "user";

            if (roles.Any(x => x.RoleId == 1))
            {
                role = "admin";
            }

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Username),
                new Claim(ClaimTypes.Role,role)
            };

            var key = new SymmetricSecurityKey(Convert.FromBase64String(_configuration.GetValue<string>("AppSettings:Token")));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            if (!_configuration.GetSection("AppSettings:Audience").Get<List<string>>().Contains(audience))
            {
                throw new TokenException("Audience not valid.");
            }

            var tokenDescriptor = new JwtSecurityToken(

                issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds

            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
         
        public (string token, string hashedToken) GenerateRefreshtoken()
        {
            var randomNumber = new byte[32];

            using var rng = RandomNumberGenerator.Create();

            rng.GetBytes(randomNumber);

            var token = Convert.ToBase64String(randomNumber);

            var hashedToken = BCrypt.Net.BCrypt.HashPassword(token);

            return (token, hashedToken);
        }
    }
}
