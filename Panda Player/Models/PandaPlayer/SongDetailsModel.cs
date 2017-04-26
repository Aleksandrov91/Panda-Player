using Panda_Player.Models.Manage.Admin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Panda_Player.Models.PandaPlayer
{
    public class SongDetailsModel
    {
        public int Id { get; set; }

        public string Artist { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string SongPath { get; set; }

        public ICollection<Playlist> Playlists { get; set; }

        public DateTime UploadDate { get; set; }

        public string UploaderId { get; set; }

        public virtual ApplicationUser Uploader { get; set; }

        public int GenreId { get; set; }

        public string Genre { get; set; }

        public ICollection<Tag> Tags { get; set; }
    }
}