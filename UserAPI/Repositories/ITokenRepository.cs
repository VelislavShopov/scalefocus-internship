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

        Task CreatePasswordResetToken(PasswordResetToken newPasswordResetToken);

        Task<PasswordResetToken?> GetPasswordResetTokenByValue(string token,string email);

        Task DeletePasswordResetToken(PasswordResetToken prt);
    }
}
