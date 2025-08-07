using System.ComponentModel.DataAnnotations;

namespace UserAPI.DTOs
{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

    
    }
}
