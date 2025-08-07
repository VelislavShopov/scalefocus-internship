using UserAPI.Models;

namespace UserAPI.Helpers
{
    public interface IJWTHelper
    {
        Guid GetCurrentUserId();

        Task<string> CreateAccessToken(User user,string audience);

        (string token, string hashedToken) GenerateRefreshtoken();
    }
}
