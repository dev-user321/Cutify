using System.ComponentModel.DataAnnotations;

namespace Cutify.ViewModels
{
    public class ResetPasswordVM
    {
        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage = "Password must be same" )]
        public string ConfirmPassword { get; set; }

    }
}
