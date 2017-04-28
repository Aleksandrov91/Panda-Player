using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Panda_Player.Models;
using Panda_Player.Models.PandaPlayer;
using Microsoft.AspNet.Identity;
using Panda_Player.Extensions;
using System.Text;
using Panda_Player.Models.ViewModels;
using System;
using System.IO;
using System.Collections.Generic;

namespace Panda_Player.Controllers
{
    public class PlaylistsController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Playlists
        [Authorize]
        [HttpGet]
        public ActionResult MyPlaylists()
        {
            var currentUserId = this.User.Identity.GetUserId();
            var userPlaylists = db.Playlists.Where(u => u.Creator.Id == currentUserId).OrderByDescending(p => p.DateCreated).ToList();
            var playlistsPerPage = userPlaylists.Take(6).ToList();

            var lastPage = Math.Ceiling((decimal)userPlaylists.Count() / 6);

            var playlistsModel = new ListAllPlaylistsViewModel
            {
                Playlists = playlistsPerPage,
                LastPage = lastPage
            };

            return PartialView(playlistsModel);
        }

        [HttpPost]
        public ActionResult MyPlaylists(ListAllPlaylistsViewModel playlistsModel)
        {
            var currentUserId = this.User.Identity.GetUserId();
            var userPlaylists = db.Playlists.Where(u => u.Creator.Id == currentUserId).OrderByDescending(p => p.DateCreated).ToList();
            var currentPagePlaylists = userPlaylists.Skip((playlistsModel.CurrentPage - 1) * 6).Take(6).ToList();

            playlistsModel.Playlists = currentPagePlaylists;

            return PartialView("PlaylistPartial", playlistsModel);
        }

        // GET: Playlists/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                this.AddNotification("Invalid playlist id!", NotificationType.ERROR);
                return RedirectToAction("MyPlaylists");
            }

            Playlist playlist = db.Playlists.Include(a => a.Songs).FirstOrDefault(a => a.Id == id);


            ConvertToM3u(playlist);

            if (playlist == null)
            {
                this.AddNotification("Invalid playlist id!", NotificationType.ERROR);
                return RedirectToAction("MyPlaylists");
            }

            if (!playlist.IsPublic)
            {
                if (!IsAuthorizedToOperate(playlist))
                {
                    this.AddNotification("Playlist is not public!", NotificationType.ERROR);
                    return RedirectToAction("MyPlaylists");
                }
            }

            var model = new PlaylistDetailsViewModel
            {
                Id = playlist.Id,
                Name = playlist.PlaylistName,
                CreatedDate = playlist.DateCreated,
                Creator = playlist.Creator.FullName,
                SongsInPlaylist = playlist.Songs
            };

            return View(model);
        }

        // GET: Playlists/Create
        [Authorize]
        public ActionResult Create()
        {
            var model = new PlaylistViewModel();

            return PartialView(model);
        }

        // POST: Playlists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PlaylistViewModel model)
        {
            if (ModelState.IsValid)
            {
                string creatorId = this.User.Identity.GetUserId();
                var creator = db.Users.Find(creatorId);

                var playlist = new Playlist
                {
                    PlaylistName = model.PlaylistName,
                    IsPublic = model.IsPublic,
                    Creator = creator,
                    DateCreated = DateTime.Now
                };

                db.Playlists.Add(playlist);
                db.SaveChanges();

                this.AddNotification($"Successfully created {model.PlaylistName} playlist", NotificationType.SUCCESS);
                return RedirectToAction("MyPlaylists");
            }

            this.AddNotification("Check your data and try again.", NotificationType.ERROR);
            return View(model);
        }

        // GET: Playlists/Edit/
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                this.AddNotification("Invalid playlist id!", NotificationType.ERROR);
                return RedirectToAction("MyPlaylists");
            }

            Playlist playlist = db.Playlists.Find(id);

            if (playlist == null)
            {
                this.AddNotification("Invalid playlist id!", NotificationType.ERROR);
                return RedirectToAction("MyPlaylists");
            }

            if (!IsAuthorizedToOperate(playlist))
            {
                this.AddNotification("You are not authorized to edit this playlist!", NotificationType.ERROR);
                return RedirectToAction("MyPlaylists");
            }

            var model = new PlaylistViewModel
            {
                Id = playlist.Id,
                PlaylistName = playlist.PlaylistName,
                IsPublic = playlist.IsPublic
            };

            return PartialView(model);
        }

        // POST: Playlists/Edit/
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

                if (playlistToEdit == null)
                {
                    this.AddNotification("Invalid playlist id!", NotificationType.ERROR);
                    return RedirectToAction("MyPlaylists");
                }

                playlistToEdit.PlaylistName = model.PlaylistName;
                playlistToEdit.IsPublic = model.IsPublic;

                db.Entry(playlistToEdit).State = EntityState.Modified;
                db.SaveChanges();

                this.AddNotification($"Succesfully edited {model.PlaylistName} playlist", NotificationType.INFO);
                return RedirectToAction("MyPlaylists");
            }

            return View(model);
        }

        // GET: Playlists/Delete/
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                this.AddNotification("Invalid playlist id!", NotificationType.ERROR);
                return RedirectToAction("MyPlaylists");
            }

            Playlist playlist = db.Playlists.Find(id);

            if (playlist == null)
            {
                this.AddNotification("Invalid playlist id!", NotificationType.ERROR);
                return RedirectToAction("MyPlaylists");
            }

            if (!IsAuthorizedToOperate(playlist))
            {
                this.AddNotification("You are not authorized to delete this playlist!", NotificationType.ERROR);
                return RedirectToAction("MyPlaylists");
            }

            return View(playlist);
        }

        // POST: Playlists/Delete/
        [HttpPost]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            Playlist playlist = db.Playlists.Find(id);

            if (playlist == null)
            {
                this.AddNotification("Invalid playlist id!", NotificationType.ERROR);
                return RedirectToAction("MyPlaylists");
            }

            db.Playlists.Remove(playlist);
            db.SaveChanges();

            this.AddNotification($"Playlist '{playlist.PlaylistName}' has been deleted.", NotificationType.INFO);
            return Json(new { Success = true, Url = "/Playlists/MyPlaylists" });
        }

        public void ConvertToM3u(Playlist playlist)
        {
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

            System.IO.File.WriteAllText(myPlayList, result.ToString());
        }

        public ActionResult DeleteFromPlaylist(int songId, int playlistId)
        {
            var playlist = db.Playlists.Include(s => s.Songs).FirstOrDefault(p => p.Id == playlistId);
            var song = playlist.Songs.FirstOrDefault(s => s.Id == songId);

            if (playlist == null)
            {
                this.AddNotification("Playlist not found!", NotificationType.ERROR);
                return RedirectToAction("MyPlaylists");
            }

            if(song == null)
            {
                this.AddNotification("Song not found!", NotificationType.ERROR);
                return RedirectToAction("MyPlaylists");
            }

            if (!IsAuthorizedToOperate(playlist))
            {
                this.AddNotification($"You have no permission to delete songs from {playlist.PlaylistName} playlist!", NotificationType.WARNING);
                return RedirectToAction("MyPlaylists");
            }

            playlist.Songs.Remove(song);
            db.SaveChanges();

            this.AddNotification($"{song.Artist} - {song.Title} has been successfully removed from {playlist.PlaylistName} playlist!", NotificationType.WARNING);

            return RedirectToAction("MyPlaylists");
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
