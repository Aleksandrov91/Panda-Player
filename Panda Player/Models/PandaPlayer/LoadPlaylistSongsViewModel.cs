using System.Collections.Generic;

namespace Panda_Player.Models.PandaPlayer
{
    public class LoadPlaylistSongsViewModel
    {
        public Playlist Playlist { get; set; }

        public string Author { get; set; }

        public string Title { get; set; }

        public List<Song> PlaylistSongs { get; set; }

        public Playlist playlist { get; set; }

    }
}