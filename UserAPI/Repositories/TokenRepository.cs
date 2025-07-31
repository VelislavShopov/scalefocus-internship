using Microsoft.EntityFrameworkCore;
using UserAPI.DTOs;
using UserAPI.Models;

namespace UserAPI.Repositories
{
    public class TokenRepository : UserDbContext, ITokenRepository
    {
        private readonly IConfiguration _configuration;

        public TokenRepository(DbContextOptions<UserDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        public async Task<RefreshToken?> GetRefreshToken(RefreshTokenRequestDTO request)
        {
            return await RefreshTokens.FirstOrDefaultAsync(x => x.Token == request.RefreshToken);

        }

        public async Task<RefreshToken?> GetRefreshTokenByUserId(Guid userId)
        {

            return await RefreshTokens.FirstOrDefaultAsync(x => userId == x.UserId);
        }

        public async Task DeleteRefreshToken(RefreshToken token)
        {
            RefreshTokens.Remove(token);
            await SaveChangesAsync();
        }

        public async Task AddRefreshToken(RefreshToken token)
        {
            await RefreshTokens.AddAsync(token);
            await SaveChangesAsync();
        }
    }
}
