using Panda_Player.Models.ViewModels;
using System.Collections.Generic;

namespace Panda_Player.Models.Manage
{
    public class ConfigureTwoFactorViewModel : BaseViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    }
}