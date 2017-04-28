using System;
using System.ComponentModel.DataAnnotations;

namespace Panda_Player.Models.ViewModels.Admin
{
    public class UserDetailsViewModel
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }

        public string Roles { get; set; }

        public string LastLoginnDate { get; set; }

        public DateTime BanEndDate { get; set; }

        [Display(Name = "Total songs")]
        public int TotalSongs { get; set; }

        [Display(Name = "Total playlists")]
        public int TotalPlaylists { get; set; }
    }
}