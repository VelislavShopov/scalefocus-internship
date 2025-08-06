using System.ComponentModel.DataAnnotations;

namespace UserAPI.Models
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }
        public string RefreshTokenValue { get; set; }

        public DateTime ExpiryTime { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; }
    }
}
