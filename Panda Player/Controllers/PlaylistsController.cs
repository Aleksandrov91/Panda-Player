using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Panda_Player.Models;
using Panda_Player.Models.PandaPlayer;
using Microsoft.AspNet.Identity;
using Panda_Player.Extensions;
using System.Text;
using Panda_Player.Models.ViewModels;
using System;

namespace Panda_Player.Controllers
{
    public class PlaylistsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Playlists
        [Authorize]
        [HttpGet]
        public ActionResult MyPlaylists()
        {
            var currentUserId = this.User.Identity.GetUserId();
            var myPlaylists = db.Playlists.Where(u => u.Creator.Id == currentUserId).ToList();
            var playlistsPerPage = myPlaylists.Take(5).ToList();

            var lastPage = Math.Ceiling((decimal)myPlaylists.Count() / 5);
            var model = new ListAllPlaylistsViewModel
            {
                Playlists = playlistsPerPage,
                LastPage = lastPage
            };

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult MyPlaylists(ListAllPlaylistsViewModel model)
        {
            var currentUserId = this.User.Identity.GetUserId();
            var myPlaylists = db.Playlists.Where(u => u.Creator.Id == currentUserId).ToList();
            var currentPagePlaylists = myPlaylists.Skip((model.CurrentPage - 1) * 5).Take(5).ToList();
            model.Playlists = currentPagePlaylists;

            return PartialView("PlaylistPartial", model);
        }
        // GET: Playlists/Details
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                this.AddNotification("Invalid playlist id!", NotificationType.ERROR);
                return RedirectToAction("Index");
            }

            Playlist playlist = db.Playlists.Include(a => a.Songs).FirstOrDefault(a => a.Id == id);

            //ConvertToM3u(playlist);

            if (playlist == null)
            {
                this.AddNotification("Invalid playlist id!", NotificationType.ERROR);
                return RedirectToAction("Index");
            }

            if (!playlist.IsPublic)
            {
                if (!IsAuthorizedToOperate(playlist))
                {
                    this.AddNotification("Playlist is not public!", NotificationType.ERROR);
                    return RedirectToAction("Index");
                }
            }

            return View(playlist);
        }

        // GET: Playlists/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Playlists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PlaylistName,IsPublic")] Playlist playlist)
        {
            if (ModelState.IsValid)
            {
                string authorId = this.User.Identity.GetUserId();
                var author = db.Users.Find(authorId);

                playlist.Creator = author;

                db.Playlists.Add(playlist);
                db.SaveChanges();

                return RedirectToAction("MyPlaylists");
            }

            return View(playlist);
        }

        // GET: Playlists/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                this.AddNotification("Invalid playlist id!", NotificationType.ERROR);
                return RedirectToAction("Index");
            }

            Playlist playlist = db.Playlists.Find(id);

            if (playlist == null)
            {
                this.AddNotification("Invalid playlist id!", NotificationType.ERROR);
                return RedirectToAction("Index");
            }

            if (!IsAuthorizedToOperate(playlist))
            {
                this.AddNotification("You are not authorized to edit this playlist!", NotificationType.ERROR);
                return RedirectToAction("Index");
            }


            var model = new PlaylistViewModel
            {
                Id = playlist.Id,
                PlaylistName = playlist.PlaylistName,
                IsPublic = playlist.IsPublic
            };

            return PartialView(model);
        }

        // POST: Playlists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PlaylistViewModel model)
        {
            if (ModelState.IsValid)
            {
                var playlistToEdit = db.Playlists.Find(model.Id);

                playlistToEdit.PlaylistName = model.PlaylistName;
                playlistToEdit.IsPublic = model.IsPublic;

                db.Entry(playlistToEdit).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("MyPlaylists");
            }

            return View(model);
        }

        // GET: Playlists/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                this.AddNotification("Invalid playlist id!", NotificationType.ERROR);
                return RedirectToAction("Index");
            }


            Playlist playlist = db.Playlists.Find(id);

            if (playlist == null)
            {
                this.AddNotification("Invalid playlist id!", NotificationType.ERROR);
                return RedirectToAction("Index");
            }

            if (!IsAuthorizedToOperate(playlist))
            {
                this.AddNotification("You are not authorized to delete this playlist!", NotificationType.ERROR);
                return RedirectToAction("Index");
            }

            return View(playlist);
        }

        // POST: Playlists/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            Playlist playlist = db.Playlists.Find(id);
            db.Playlists.Remove(playlist);
            db.SaveChanges();

            this.AddNotification("The Playlist has been deleted successfully.", NotificationType.SUCCESS);
            return Json(new { Success = true });
        }

        public ActionResult LoadPlaylist(int? id)
        {

            var playlist = db.Playlists.Include(song => song.Songs).FirstOrDefault(a => a.Id == id);
            var playlistSongs = playlist.Songs.ToList();

            var result = new StringBuilder();

            result.AppendLine("#EXTM3U");
            result.AppendLine("");


            foreach (var song in playlistSongs)
            {
                var formattedSong = $"#EXTINF:1,{song.Artist} - {song.Title}";
                var songPath = song.SongPath;

                result.AppendLine(formattedSong);
                result.AppendLine($"http://localhost:4522{songPath}");
            }

            var directoryPath = HttpContext.Server.MapPath("~/Uploads/Playlists");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string uploadDir = Server.MapPath("~/Uploads/Playlists/");
            var myPlayList = $@"{uploadDir}currentPlaylist.m3u";

            if (!System.IO.File.Exists($"{myPlayList}"))
            {
                System.IO.File.Create($"{myPlayList}");
            }

            if (!System.IO.File.Exists($"{myPlayList}"))
            {
                System.IO.File.Create($"{myPlayList}");
            }

            System.IO.File.WriteAllText(myPlayList, result.ToString());

            var model = new LoadPlaylistSongsViewModel
            {
                Playlist = playlist,
                PlaylistSongs = playlistSongs
            };

            return PartialView("LoadPlaylist");
        }

        //private void ConvertToM3u(Playlist playlist)
        //{
        //    var result = new StringBuilder();

        //    result.AppendLine("#EXTM3U");
        //    result.AppendLine("");

        //    var playlistSongs = playlist.Songs.ToList();

        //    foreach (var song in playlistSongs)
        //    {
        //        var formattedSong = $"#EXTINF:1,{song.Artist} - {song.Title}";
        //        var songPath = song.SongPath;

        //        result.AppendLine(formattedSong);
        //        result.AppendLine($"http://localhost:4522{songPath}");
        //    }

        //    var directoryPath = HttpContext.Server.MapPath("~/Uploads/Playlists");

        //    if (!Directory.Exists(directoryPath))
        //    {
        //        Directory.CreateDirectory(directoryPath);
        //    }

        //    string uploadDir = Server.MapPath("~/Uploads/Playlists/");
        //    var myPlayList = $@"{uploadDir}currentPlaylist.m3u";

        //    if (!System.IO.File.Exists($"{myPlayList}"))
        //    {
        //        System.IO.File.Create($"{myPlayList}");
        //    }

        //    if (!System.IO.File.Exists($"{myPlayList}"))
        //    {
        //        System.IO.File.Create($"{myPlayList}");
        //    }

        //    System.IO.File.WriteAllText(myPlayList, result.ToString());
        //}

        private void ConvertToM3u(Playlist playlist)
        {
            var result = new StringBuilder();

            result.AppendLine("#EXTM3U");
            result.AppendLine("");

            var playlistSongs = playlist.Songs.ToList();

            foreach (var song in playlistSongs)
            {
                var formattedSong = $"#EXTINF:1,{song.Artist} - {song.Title}";
                var songPath = song.SongPath;

                result.AppendLine(formattedSong);
                result.AppendLine($"http://localhost:4522{songPath}");
            }

            string uploadDir = Server.MapPath("~/");
            var myPlayList = $@"{uploadDir}Uploads/Playlists/currentPlaylist.m3u";
    
            if (!System.IO.File.Exists($"{myPlayList}"))
            {
                System.IO.File.Create($"{myPlayList}");
            }

            System.IO.File.WriteAllText(myPlayList, result.ToString());
        }

        public ActionResult DeleteFromPlaylist(int songId, int playlistId)
        {
            var playlist = db.Playlists.Include(s => s.Songs).FirstOrDefault(p => p.Id == playlistId);
            var song = playlist.Songs.FirstOrDefault(s => s.Id == songId);
            playlist.Songs.Remove(song);
            db.SaveChanges();
            
            return RedirectToAction("Index");
        }

        private bool IsAuthorizedToOperate(Playlist playlist)
        {
            bool isAdmin = this.User.IsInRole("Admin");
            bool isCreator = playlist.IsCreator(this.User.Identity.Name);

            return isAdmin || isCreator;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
