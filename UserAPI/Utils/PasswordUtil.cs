

using System.Security.Cryptography;
using System.Text;

namespace UserAPI.Utils
{
    public static class PasswordHasher
    {
        public static void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            var rng = RandomNumberGenerator.Create();
            salt = new byte[32];
            rng.GetBytes(salt);

            var sha256 = SHA256.Create();
            var combined = salt.Concat(Encoding.UTF8.GetBytes(password)).ToArray();
            hash = sha256.ComputeHash(combined);
        }

        public static bool VerifyPasswordHash(string password, byte[] hash, byte[] salt)
        {
            var sha256 = SHA256.Create();
            var combined = salt.Concat(Encoding.UTF8.GetBytes(password)).ToArray();
            var hashToCheck = sha256.ComputeHash(combined);

            return hashToCheck.SequenceEqual(hash);
        }
    }
}
