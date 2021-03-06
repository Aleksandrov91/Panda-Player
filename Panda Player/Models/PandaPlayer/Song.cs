﻿using Panda_Player.Models.Manage.Admin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Panda_Player.Models.PandaPlayer
{
    public class Song
    {
        private ICollection<Tag> tags;

        public Song()
        {
            this.Playlists = new HashSet<Playlist>();
            this.tags = new HashSet<Tag>();
            this.UploadDate = DateTime.Now;
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

        public virtual ICollection<Playlist> Playlists { get; set; }

        public DateTime UploadDate { get; set; }

        public string UploaderId { get; set; }

        public virtual ApplicationUser Uploader { get; set; }

        public bool IsUploader(string name)
        {
            return this.Uploader.UserName.Equals(name);
        }

        [Required]
        [Display(Name = "Genre *")]
        public int GenreId { get; set; }

        public virtual Genre Genre { get; set; }

        public virtual ICollection<Tag> Tags
        {
            get { return this.tags; }
            set { this.tags = value; }
        }
    }
}