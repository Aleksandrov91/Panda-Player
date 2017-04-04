using System;
using System.ComponentModel.DataAnnotations;

namespace Panda_Player.Models.PandaPlayer
{
    public class Song
    {
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

        public Playlist Playlist { get; set; }

        [Required]
        public DateTime UploadDate { get; set; }

        public string UploaderId { get; set; }

        public virtual ApplicationUser Uploader { get; set; }

    }
}