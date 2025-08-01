using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using UserAPI.DTOs;
using UserAPI.Models;
using UserAPI.Repositories;

namespace UserAPI.Services
{
    public class TokenService : ITokenService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenRepository _tokenRepository;

        private readonly IConfiguration _configuration;

        public TokenService(IUserRepository userRepository,ITokenRepository tokenRepository, IConfiguration configuration)
        {
            _tokenRepository = tokenRepository;
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<TokenResponseDTO> GetTokenResponse(RefreshTokenRequestDTO request)
        {
            //проверяваме дали е валиден -> взимаме userId -> правим access token + new refresh token
            // -> трябва да се логнеш 

            var refreshToken = await _tokenRepository.GetRefreshToken(request);

            if (refreshToken == null)
            {
                throw new Exception();
            }
            else if (refreshToken.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                throw new Exception();
            }

            var user = await _userRepository.GetUser(refreshToken.UserId);

            var accessToken = await CreateToken(user);

            var response = new TokenResponseDTO()
            {
                RefreshToken = request.RefreshToken,
                AccessToken = accessToken
            };

            return response;

        }

        private string GenerateRefreshtoken()
        {
            var randomNumber = new byte[32];

            using var rng = RandomNumberGenerator.Create();

            rng.GetBytes(randomNumber);


            return Convert.ToBase64String(randomNumber);

        }

        public async Task<string> CreateToken(User user)
        {
            // loading the roles and if there is an admin role it is what is send in the token, can be changed later!
            var roles = await _userRepository.GetRolesForUser(user);

            var role = "user";

            if (roles.Any(x => x.RoleId == 2))
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

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(

                issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
                audience: "events_api",
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds

            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

        }

        public async Task<TokenResponseDTO> CreatetokenResponse(User user)
        {
            return new TokenResponseDTO { AccessToken = CreateToken(user).Result, RefreshToken = await GenerateAndSaveRefreshtokenAsync(user) };
        }



        private async Task<string> GenerateAndSaveRefreshtokenAsync(User user)
        {

            var currentRefreshToken = await _tokenRepository.GetRefreshTokenByUserId(user.Id);

            if (currentRefreshToken != null)
            {
                await _tokenRepository.DeleteRefreshToken(currentRefreshToken);
            }


            var refreshToken = GenerateRefreshtoken();


            var newRefreshToken = new RefreshToken()
            {
                UserId = user.Id,
                Token = refreshToken,
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
            };

            await _tokenRepository.AddRefreshToken(newRefreshToken);

            return refreshToken;

        }

    }
}
