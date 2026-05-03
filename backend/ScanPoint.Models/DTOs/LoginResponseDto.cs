namespace ScanPoint.Models.DTOs
{
    public class LoginResponseDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }  // ✅ për mesazhin "llogaria joaktive"
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string? Role { get; set; }
    }
}