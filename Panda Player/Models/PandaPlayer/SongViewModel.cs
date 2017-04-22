using Panda_Player.Models.Manage.Admin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Panda_Player.Models.PandaPlayer
{
    public class SongViewModel
    {
        public SongViewModel()
        {
            
        }

        public int Id { get; set; }

        [Required]
        [Display(Name = "Artist *")]
        public string Artist { get; set; }

        [Required]
        [Display(Name = "Title *")]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Song Path *")]
        public string SongPath { get; set; }

        public string UploaderId { get; set; }

        [Required]
        public DateTime UploadDate { get; set; }

        public ICollection<Genre> Genre { get; set; }

        public int GenreId { get; set; }

        public HttpPostedFileBase File { get; set; }
    }
}