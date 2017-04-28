using Panda_Player.Models.PandaPlayer;
using System;
using System.Collections.Generic;

namespace Panda_Player.Models.ViewModels
{
    public class PlaylistDetailsViewModel : BaseViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsPublic { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Creator { get; set; }

        public ICollection<Song> SongsInPlaylist { get; set; }
    }
}