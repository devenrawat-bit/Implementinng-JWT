namespace Jwt18.Models
{
    public class TokenResponseDto
    {
        public string RefreshToken { get; set; }=string.Empty;  
        public string AccessToken { get; set; }=string.Empty;  
    }
}
