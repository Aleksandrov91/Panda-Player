using Panda_Player.Models.PandaPlayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Panda_Player.Models.Manage.Admin
{
    public class DeleteUserViewModel
    {
        public string UserName { get; set; }

        public string FullName { get; set; }

        public int UploadedSongsCount { get; set; }

        public int PlaylistCount { get; set; }
    }
}