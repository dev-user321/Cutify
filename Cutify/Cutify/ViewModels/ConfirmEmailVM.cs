using System.ComponentModel.DataAnnotations;

namespace Cutify.ViewModels
{
    public class ConfirmEmailVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
