using Azure.Core;
using UserAPI.DTOs;
using UserAPI.Models;

namespace UserAPI.Services
{
    public interface ITokenService
    {
        Task<TokenResponseDTO> CreateAccessAndRefreshTokenResponse(User user,string audience);

        Task<TokenResponseDTO> GetTokenResponse(RefreshTokenRequestDTO request);
    }
}
