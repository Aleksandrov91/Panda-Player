using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Panda_Player.Models.PandaPlayer
{
    public class Tag
    {
        private ICollection<Song> songs;

        public Tag()
        {
            this.songs = new HashSet<Song>();
        }

        [Key]
        public int id { get; set; }

        [Index(IsUnique = true)]
        [StringLength(10)]
        public string Name { get; set; }

        public virtual ICollection<Song> Songs
        {
            get { return this.songs; }
            set { this.songs = value; }
        }
    }
}