using Panda_Player.Models.PandaPlayer;
using System.Collections.Generic;

namespace Panda_Player.Models.ViewModels
{
    public class IndexViewModel
    {
        public List<Song> Songs { get; set; }

        public List<Playlist> Playlists { get; set; }

        public List<Playlist> UserPlaylists { get; set; }

        public Song LastAddedSong { get; set; }
    }
}