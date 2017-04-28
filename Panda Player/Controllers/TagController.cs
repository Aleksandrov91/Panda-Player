using Microsoft.AspNet.Identity;
using Panda_Player.Extensions;
using Panda_Player.Models;
using Panda_Player.Models.PandaPlayer;
using Panda_Player.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Panda_Player.Controllers
{
    public class TagController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }


        // GET: Tag
        [HttpGet]
        public ActionResult List(int? id)
        {
            if (id == null)
            {
                this.AddNotification("Song does not exist", NotificationType.ERROR);
                return RedirectToAction("MySongs");
            }

            using (var db = new ApplicationDbContext())
            {
                var songs = db.Tags
                     .Include(t => t.Songs.Select(s => s.Tags))
                     .Include(t => t.Songs.Select(s => s.Uploader))
                     .FirstOrDefault(t => t.id == id)
                     .Songs
                     .ToList();

                var currentUser = this.User.Identity.GetUserId();
                var userPlaylists = db.Playlists
                .Where(a => a.Creator.Id == currentUser)
                .ToList();

                var model = new ListAllSongsViewModel
                {
                    Songs = songs,
                    UserPlaylists = userPlaylists,
                };

                return View(model);
            }
        }
    }
}