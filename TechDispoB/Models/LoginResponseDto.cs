using System.Text.Json.Serialization;

namespace TechDispoB.Models
{
    public class LoginResponseDto
    {
        [JsonPropertyName("token")]
        public string? Token { get; set; }
        [JsonPropertyName("user")]
        public UserDto? User { get; set; }
    }
}
