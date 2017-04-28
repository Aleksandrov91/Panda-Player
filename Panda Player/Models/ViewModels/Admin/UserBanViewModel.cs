using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Panda_Player.Models.ViewModels.Admin
{
    public class UserBanViewModel
    {
        public string Id { get; set; }

        [Display(Name ="Email")]
        public string UserName { get; set; }

        [Display(Name = "Name")]
        public string FullName { get; set; }

        [Display(Name = "Total Songs")]
        public int UploadedSongsCount { get; set; }

        [Display(Name = "Total Playlists")]
        public int PlaylistCount { get; set; }

        [Display(Name = "Select ban end date")]
        public DateTime BanEndDate { get; set; }
    }
}