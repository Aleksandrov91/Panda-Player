using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Panda_Player.Models.PandaPlayer
{
    public class Playlist
    {
        public Playlist()
        {
            this.Songs = new HashSet<Song>();
        }
        
        [Key]
        public int Id { get; set; }

        [Required]
        public string PlaylistName { get; set; }

        [Required]
        public bool IsPublic { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        public ICollection<Song> Songs { get; set; }

        public virtual ApplicationUser Creator { get; set; }

        public bool IsCreator(string name)
        {
            return this.Creator.UserName.Equals(name);
        }
    }
}