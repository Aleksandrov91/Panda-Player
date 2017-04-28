using Panda_Player.Models.PandaPlayer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Panda_Player.Models.ViewModels.Admin
{
    public class UserPlaylistsViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Playlist Name")]
        public string PlaylistName { get; set; }

        [Display(Name = "Is Public")]
        public bool IsPublic { get; set; }

        public ApplicationUser Creator { get; set; }

        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }

        public ICollection<Playlist> PlaylistsList { get; set; }
    }
}