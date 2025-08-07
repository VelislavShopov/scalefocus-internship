using System.ComponentModel.DataAnnotations;

namespace UserAPI.DTOs
{
    public class ResetPasswordDTO
    {
        [Required]
        public string? newPassword {  get; set; }

        [Compare("newPassword", ErrorMessage = "Passwords do not match")]
        public string? confirmPassword { get; set; }

        [Required]
        public string Token { get; set; }
    }
}
