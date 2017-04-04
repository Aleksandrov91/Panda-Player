using System.ComponentModel.DataAnnotations;

namespace Panda_Player.Models.Identity
{
    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}