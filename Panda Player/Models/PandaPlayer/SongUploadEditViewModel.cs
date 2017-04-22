using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Panda_Player.Models.PandaPlayer
{
    public class SongUploadEditViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Artist *")]
        public string Artist { get; set; }

        [Required]
        [Display(Name = "Title *")]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public string UploaderId { get; set; }

        //[Display(Name = "Category *")]
        //public int Category { get; set; }

        [Display(Name = "Song Path *")]
        public string SongPath { get; set; }

        [Required]
        public DateTime UploadDate { get; set; }

        public ICollection<Song> Songs { get; set; }

        public int GenreId { get; set; }

        //public int GenreId { get; set; }

        public HttpPostedFileBase File { get; set; }

        public string Tags { get; set; }
    }
}