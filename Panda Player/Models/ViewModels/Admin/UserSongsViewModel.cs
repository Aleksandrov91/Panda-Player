using Panda_Player.Models.PandaPlayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Panda_Player.Models.ViewModels.Admin
{
    public class UserSongsViewModel
    {
        public int Id { get; set; }

        public string Artist { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string UploadDate { get; set; }

        public string Uploader { get; set; }

        public List<Song> Songs { get; set; }
    }
}