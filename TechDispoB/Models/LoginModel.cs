using System.ComponentModel.DataAnnotations;

namespace TechDispoB.Models
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty; 

        [Display(Name = "Se rappeler de moi ")]
        public bool RememberMe { get; set; } = true; 
    }

}
