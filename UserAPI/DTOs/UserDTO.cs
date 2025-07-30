namespace UserAPI.DTOs
{
    public class CreateUserDTO
    {

        //Добавен е string.empty на всеки параметър с цел защита от грешки при изпълнение.-Георги Станков
        //Има нужда от обновяване на базата данни.
        public string Username { get; set; }=string.Empty;

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // Втора парола за потвърждение

        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class LoginUserDTO
    {
        // може да използваме така
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

    }

}
