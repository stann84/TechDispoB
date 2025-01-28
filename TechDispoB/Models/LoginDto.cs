using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TechDispoB.Models
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        [JsonPropertyName("email")]
        public string? Email { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        [JsonPropertyName("password")]
        public string? Password { get; set; }

        [Display(Name = "Se rappeler de moi ")]
        [JsonPropertyName("rememberMe")]
        public bool RememberMe { get; set; } = true;

    }
}
