using Microsoft.EntityFrameworkCore;
using UserAPI.DTOs;
using UserAPI.Models;

namespace UserAPI.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IConfiguration _configuration;
        private readonly UserDbContext _context;

        public TokenRepository(UserDbContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<RefreshToken?> GetRefreshToken(RefreshTokenRequestDTO request)
        {
            var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(x=>x.UserId == request.UserId);
            return refreshToken;

        }

        public async Task<RefreshToken?> GetRefreshTokenByUserId(Guid userId)
        {

            return await _context.RefreshTokens.FirstOrDefaultAsync(x => userId == x.UserId);
        }

        public async Task DeleteRefreshToken(RefreshToken token)
        {
            _context.RefreshTokens.Remove(token);
            await _context.SaveChangesAsync();
        }

        public async Task AddRefreshToken(RefreshToken token)
        {
            await _context.RefreshTokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task CreatePasswordResetToken(PasswordResetToken token)
        {
            await _context.PasswordResetTokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }
        public async Task<PasswordResetToken?> GetPasswordResetTokenByValue(string token)
        {
            var passwordresettoken = await _context.PasswordResetTokens.FirstOrDefaultAsync(x => x.PasswordResetTokenValue == token);
            return passwordresettoken;
        }

        public async Task DeletePasswordResetToken(PasswordResetToken passwordresettoken)
        {
            _context.PasswordResetTokens.Remove(passwordresettoken);
            await _context.SaveChangesAsync();
        }

    }
}
