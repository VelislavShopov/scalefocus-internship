using System.ComponentModel.DataAnnotations;

namespace UserAPI.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        //Да добавим username? -Георги Станков

        //Добавен е string.empty на всеки параметър с цел защита от грешки при изпълнение.-Георги Станков

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; }=string.Empty;

        [Required]
        public string LastName { get; set; }=string.Empty;

        [Required]
        public string Email { get; set; }=string.Empty;

        public string PasswordHash { get; set; }= string.Empty;


        //Добавих роли, за да работят токените.-Георги Станков

        [Required]

        public string Role {  get; set; }=string.Empty;

        //Добавих refresh token
        public string? RefreshToken { get; set; }

        public DateTime RefreshTokenExpiryTime { get; set; }


    }
}