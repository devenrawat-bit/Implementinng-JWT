namespace Jwt18.Models
{
    public class RefreshTokenRequestDto
    {
        public Guid UserID {  get; set; }
        public string RefreshToken { get; set; }=string.Empty;  
    }
}
