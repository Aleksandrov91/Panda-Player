using Panda_Player.Models.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace Panda_Player.Models.Manage
{
    public class AddPhoneNumberViewModel : BaseViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Number { get; set; }
    }
}