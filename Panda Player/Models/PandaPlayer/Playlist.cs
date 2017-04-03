using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Panda_Player.Models.PandaPlayer
{
    public class Playlist
    {
        public Playlist()
        {
            Users = new HashSet<ApplicationUser>();
        }
        
        [Key]
        public int Id { get; set; }

        [Required]
        public string PlaylistName { get; set; }

        [Required]
        public bool IsPublic { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }
    }
}