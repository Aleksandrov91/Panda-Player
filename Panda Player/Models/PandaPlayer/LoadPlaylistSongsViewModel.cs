using System.Collections.Generic;

namespace Panda_Player.Models.PandaPlayer
{
    public class LoadPlaylistSongsViewModel
    {
        public Playlist Playlist { get; set; }

        public string PlaylistName { get; set; }

        public List<Song> PlaylistSongs { get; set; }

    }
}