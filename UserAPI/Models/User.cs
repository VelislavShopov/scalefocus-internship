using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace UserAPI.Models
{
    public class User
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; }=string.Empty;

        [Required]
        public string LastName { get; set; }=string.Empty;

        [Required]
        public string Email { get; set; }=string.Empty;

        public string PasswordHash { get; set; }= string.Empty;


        public List<Role> Roles { get; } = [];

        public string? PasswordResetToken { get; set; }
      
        public DateTime? PasswordResetTokenExpires { get; set; }

        public RefreshToken? RefreshToken { get; set; }
    }
}