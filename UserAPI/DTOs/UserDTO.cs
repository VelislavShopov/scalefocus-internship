namespace UserAPI.DTOs
{
    public class CreateUserDTO
    {
        public string Username { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }

        // Втора парола за потвърждение

        public string ConfirmPassword { get; set; }
    }

    public class LoginUserDTO
    {
        // може да използваме така
        public string Username { get; set; }
        public string Password { get; set; }

    }

}
