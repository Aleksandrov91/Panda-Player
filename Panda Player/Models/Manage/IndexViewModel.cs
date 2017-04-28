using Microsoft.AspNet.Identity;
using Panda_Player.Models.ViewModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Panda_Player.Models.Manage
{
    public class IndexViewModel : BaseViewModel
    {
        public bool HasPassword { get; set; }
        public bool HasProfilePic { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactor { get; set; }
        public bool BrowserRemembered { get; set; }

        [Display(Name = "File")]
        public string ImagePath { get; set; }

        public HttpPostedFileBase File { get; set; }
    }
}