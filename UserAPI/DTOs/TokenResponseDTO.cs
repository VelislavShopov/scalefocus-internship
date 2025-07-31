namespace UserAPI.DTOs
{
    //Ново DTO. Да се вкара в базата.-Георги Станков.

    public class TokenResponseDTO
    {

        public required string AccessToken {  get; set; }   

        public required string RefreshToken { get; set; }

    }
}
