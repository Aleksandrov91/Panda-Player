using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [ForeignKey("Songs")]
        public virtual ICollection<Song> Songs { get; set; }

        public virtual ApplicationUser Author { get; set; }
    }
}