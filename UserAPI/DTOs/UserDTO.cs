namespace UserAPI.DTOs
{
    public class UserDTO
    {
        //Да добавим username? -Георги Станков
        public string Username { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginUserDTO
    {
        // може да използваме така
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
