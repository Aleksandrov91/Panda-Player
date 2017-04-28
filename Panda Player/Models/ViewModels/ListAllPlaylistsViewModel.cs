using Panda_Player.Models.PandaPlayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Panda_Player.Models.ViewModels
{
    public class ListAllPlaylistsViewModel : BaseViewModel
    {
        public ListAllPlaylistsViewModel()
        {
            this.CurrentPage = 1;
        }
        public List<Playlist> Playlists { get; set; }

        public int CurrentPage { get; set; }

        public decimal LastPage { get; set; }
    }
}