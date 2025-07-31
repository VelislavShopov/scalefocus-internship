using Azure.Core;
using UserAPI.DTOs;
using UserAPI.Models;

namespace UserAPI.Services
{
    public interface ITokenService
    {
        Task<TokenResponseDTO> CreatetokenResponse(User user);

        Task<string> CreateToken(User user);

        Task<TokenResponseDTO> GetTokenResponse(RefreshTokenRequestDTO request);
    }
}
