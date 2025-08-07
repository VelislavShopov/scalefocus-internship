using System.ComponentModel.DataAnnotations;

namespace UserAPI.Models
{
    public class PasswordResetToken
    {
        [Key]
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string? PasswordResetTokenValue { get; set; }
        public DateTime? PasswordResetTokenExpires { get; set; }

        public User User { get; set; } = null!;
    }
}
