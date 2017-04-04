using System.ComponentModel.DataAnnotations;

namespace Panda_Player.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
