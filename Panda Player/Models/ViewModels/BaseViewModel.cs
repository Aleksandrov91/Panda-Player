using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Panda_Player.Models.ViewModels
{
    public class BaseViewModel
    {
        public BaseViewModel()
        {
            this.Path = string.Empty;
        }
        public string Path { get; set; }
    }
}