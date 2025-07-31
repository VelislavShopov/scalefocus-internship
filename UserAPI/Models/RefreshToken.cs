using System.ComponentModel.DataAnnotations;

namespace UserAPI.Models
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }
        public string Token { get; set; }

        public DateTime RefreshTokenExpiryTime { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; }
    }
}
