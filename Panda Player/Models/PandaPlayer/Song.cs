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
        public string Author { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public string SongPath { get; set; }

        [Required]
        public DateTime UploadDate { get; set; }

        public string UploaderId { get; set; }

        public virtual ApplicationUser Uploader { get; set; }
    }
}