using UserAPI.DTOs;
using UserAPI.Models;

namespace UserAPI.Repositories
{
    public interface ITokenRepository
    {

        Task<RefreshToken?> GetRefreshToken(RefreshTokenRequestDTO request);

        Task<RefreshToken?> GetRefreshTokenByUserId(Guid userId);

        Task DeleteRefreshToken(RefreshToken refreshToken);

        Task AddRefreshToken(RefreshToken newRefreshToken);

    }
}
