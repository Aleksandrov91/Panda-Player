using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Panda_Player.Models.PandaPlayer
{
    public class Song
    {
        public Song()
        {
            this.Playlists = new HashSet<Playlist>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string Artist { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public string SongPath { get; set; }

        public ICollection<Playlist> Playlists { get; set; }

        [Required]
        public DateTime UploadDate { get; set; }

        public string UploaderId { get; set; }

        public virtual ApplicationUser Uploader { get; set; }

        public bool IsUploader(string name)
        {
            return this.Uploader.UserName.Equals(name);
        }
    }
}