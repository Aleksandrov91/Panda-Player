using Panda_Player.Models.PandaPlayer;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Panda_Player.Models.Manage.Admin
{
    public class Genre
    {
        public Genre()
        {
            this.Songs = new HashSet<Song>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<Song> Songs { get; set; }
    }
}