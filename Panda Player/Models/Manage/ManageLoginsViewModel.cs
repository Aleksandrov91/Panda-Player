using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Panda_Player.Models.ViewModels;
using System.Collections.Generic;

namespace Panda_Player.Models.Manage
{
    public class ManageLoginsViewModel : BaseViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }
}