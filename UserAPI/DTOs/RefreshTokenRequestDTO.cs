namespace UserAPI.DTOs
{
    public class RefreshTokenRequestDTO
    {
        public Guid UserId { get; set; }
        public required string RefreshToken { get; set; }

        public string Audience { get; set; }

    }
}
