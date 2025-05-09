using System.ComponentModel.DataAnnotations;

namespace Cutify.ViewModels
{
    public class RegisterVM
    {
        [Required]
        public IFormFile Image { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string Password { get; set; }
    }

}
