using Panda_Player.Models.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace Panda_Player.Models.Manage
{
    public class VerifyPhoneNumberViewModel : BaseViewModel
    {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }
}