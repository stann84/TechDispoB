using System.ComponentModel.DataAnnotations;

namespace TechDispoB.Models
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [Display(Name = "Se rappeler de moi ")]
        public bool? RememberMe { get; set; } = true;

    }
}
