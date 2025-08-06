using Azure.Core;
using UserAPI.DTOs;
using UserAPI.Models;

namespace UserAPI.Services
{
    public interface ITokenService
    {
        Task<TokenResponseDTO> CreatetokenResponse(User user,string audience);

        Task<string> CreateToken(User user,string audience);

        Task<TokenResponseDTO> GetTokenResponse(RefreshTokenRequestDTO request);
    }
}
