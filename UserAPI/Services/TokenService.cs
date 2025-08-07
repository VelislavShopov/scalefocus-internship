using BCrypt.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using UserAPI.DTOs;
using UserAPI.Exceptions;
using UserAPI.Helpers;
using UserAPI.Models;
using UserAPI.Repositories;

namespace UserAPI.Services
{
    public class TokenService : ITokenService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenRepository _tokenRepository;
        private readonly IConfiguration _configuration;
        private readonly IJWTHelper _jwtHelper;

        public TokenService(IUserRepository userRepository,ITokenRepository tokenRepository, IConfiguration configuration, IJWTHelper jWTHelper)
        {
            _tokenRepository = tokenRepository;
            _userRepository = userRepository;
            _configuration = configuration;
            _jwtHelper = jWTHelper;
        }

        public async Task<TokenResponseDTO> CreateAccessAndRefreshTokenResponse(User user, string audience)
        {
            return new TokenResponseDTO {
                AccessToken = _jwtHelper.CreateAccessToken(user, audience).Result,
                RefreshToken = await GenerateAndSaveRefreshtokenAsync(user)
            };
        }

        public async Task<TokenResponseDTO> GetTokenResponse(RefreshTokenRequestDTO request)
        {
            //проверяваме дали е валиден -> взимаме userId -> правим access token + new refresh token
            // -> трябва да се логнеш 

            var refreshToken = await _tokenRepository.GetRefreshToken(request);

            if (refreshToken == null)
            {
                throw new TokenException("The user doesn't exist or doesn't have a refresh token.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.RefreshToken, refreshToken.RefreshTokenValue))
            {
                throw new TokenException("Invalid refresh token.");
            }

            if (refreshToken.ExpiryTime < DateTime.UtcNow)
            {
                _tokenRepository.DeleteRefreshToken(refreshToken);
                throw new TokenException("The token has expired.");
            }

            var user = await _userRepository.GetUser(refreshToken.UserId);

            var accessToken = await _jwtHelper.CreateAccessToken(user,request.Audience);

            var response = new TokenResponseDTO()
            {
                RefreshToken = request.RefreshToken,
                AccessToken = accessToken
            };

            return response;

        }

        private async Task<string> GenerateAndSaveRefreshtokenAsync(User user)
        {

            var currentRefreshToken = await _tokenRepository.GetRefreshTokenByUserId(user.Id);

            if (currentRefreshToken != null)
            {
                await _tokenRepository.DeleteRefreshToken(currentRefreshToken);
            }

            var refreshToken = _jwtHelper.GenerateRefreshtoken();

            var newRefreshToken = new RefreshToken()
            {
                UserId = user.Id,
                RefreshTokenValue = refreshToken.hashedToken,
                ExpiryTime = DateTime.UtcNow.AddDays(7)
            };

            await _tokenRepository.AddRefreshToken(newRefreshToken);

            return refreshToken.token;

        }
    }
}
