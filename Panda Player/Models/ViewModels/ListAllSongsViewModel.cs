using Panda_Player.Models.PandaPlayer;
using System.Collections.Generic;

namespace Panda_Player.Models.ViewModels
{
    public class ListAllSongsViewModel
    {
        public ListAllSongsViewModel()
        {
            this.CurrentPage = 1;
        }
        public List<Song> Songs { get; set; }

        public List<Playlist> UserPlaylists { get; set; }
        public int CurrentPage { get; set; }
        public decimal LastPage { get; set; }
    }
}