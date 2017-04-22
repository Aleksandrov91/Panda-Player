using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Panda_Player.Models;
using Panda_Player.Models.PandaPlayer;
using System.Web;
using System;
using Microsoft.AspNet.Identity;
using Panda_Player.Extensions;
using System.IO;

namespace Panda_Player.Controllers
{
    [Authorize]
    public class SongsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Songs
        public ActionResult MySongs()
        {
            var currentUser = this.User.Identity.GetUserId();
            var userSongs = db.Songs.Where(s => s.Uploader.Id == currentUser).Include(u => u.Uploader).ToList();

            var playlists = db.Playlists.Where(a => a.Author.Id == currentUser).ToList();

            ViewBag.Playlists = playlists;

            return View(userSongs);
        }

        // GET: Songs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                this.AddNotification("Song does not exist", NotificationType.ERROR);
                return RedirectToAction("MySongs");
            }
            Song song = db.Songs.Find(id);
            if (song == null)
            {
                this.AddNotification("Song does not exist", NotificationType.ERROR);
                return RedirectToAction("MySongs");
            }
            return View(song);
        }

        // GET: Songs/Upload
        public ActionResult Upload()
        {
            return PartialView();
        }

        // POST: Songs/Upload
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Upload(Song song, HttpPostedFileBase file)
        {
            if (ModelState.IsValid && file != null)
            {
                var validTypes = new[]
                {
                    "audio/mpeg",
                    "audio/mp3",
                    "audio/wav",
                    "audio/flac",
                    "audio/wv"
                };

                if (validTypes.Contains(file.ContentType))
                {
                    var currentUser = this.User.Identity.GetUserId();
                    var songsPath = "~/Uploads/";
                    var mappedPath = HttpContext.Server.MapPath(songsPath);
                    var uploadFilename = Path.GetFileName(file.FileName);
                    var randomHash = Guid.NewGuid().ToString().Substring(0, 6);

                    var fileName = randomHash + "_" + uploadFilename;


                    var absoluteFilePath = mappedPath + fileName;

                    var currentSong = new Song
                    {
                        Artist = song.Artist,
                        Title = song.Title,
                        Description = song.Description,
                        UploaderId = currentUser,
                        SongPath = $"/Uploads/{fileName}",
                        UploadDate = DateTime.Now
                    };

                    if (!Directory.Exists(mappedPath))
                    {
                        Directory.CreateDirectory(mappedPath);
                    }

                    file.SaveAs(absoluteFilePath);

                    db.Songs.Add(currentSong);
                    db.SaveChanges();

                    this.AddNotification("The song has been upload successfully.", NotificationType.SUCCESS);
                    return RedirectToAction("Index", "Home");
                }

                this.AddNotification("The File must be only mp3 or wav.", NotificationType.ERROR);
                return View(song);
            }

            this.AddNotification("The file cannot be null", NotificationType.ERROR);
            return View(song);
        }

        // GET: Songs/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                this.AddNotification("Song does not exist", NotificationType.ERROR);
                return RedirectToAction("MySongs");
            }

            Song song = db.Songs.Find(id);

            if (!IsAuthorizedToOperate(song))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            if (song == null)
            {
                this.AddNotification("Song does not exist", NotificationType.ERROR);
                return RedirectToAction("MySongs");
            }
            return View(song);
        }

        // POST: Songs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Song song)
        {
            var currentSong = db.Songs.FirstOrDefault(s => s.Id == song.Id);
            if (currentSong == null)
            {
                this.AddNotification("Song does not exist", NotificationType.ERROR);
                return RedirectToAction("MySongs");
            }

            currentSong.Artist = song.Artist;
            currentSong.Title = song.Title;
            currentSong.Description = song.Description;

            db.SaveChanges();

            this.AddNotification("The Song has been updated successfully.", NotificationType.SUCCESS);
            return RedirectToAction($"Details/{currentSong.Id}");
        }

        // GET: Songs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                this.AddNotification("Song does not exist", NotificationType.ERROR);
                return RedirectToAction("MySongs");
            }
            Song song = db.Songs.Find(id);

            if (!IsAuthorizedToOperate(song))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            if (song == null)
            {
                this.AddNotification("Song does not exist", NotificationType.ERROR);
                return RedirectToAction("MySongs");
            }
            return View(song);
        }

        // POST: Songs/Delete/5
        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {            
            string uploadDir = Server.MapPath("~/");
            Song song = db.Songs.Find(id);

            string songPath = song.SongPath;

            var absolutePath = uploadDir + songPath;

            System.IO.File.Delete(absolutePath);
            db.Songs.Remove(song);
            db.SaveChanges();

            this.AddNotification("Song has been deleted successfully.", NotificationType.SUCCESS);
            return Json(new { Success = true });
        }

        public ActionResult AddSongToPlaylist(int songId, int playlistId)
        {
            var playlist = db.Playlists.Find(playlistId);
            var song = db.Songs.Find(songId);

            playlist.Songs.Add(song);

            db.SaveChanges();

            this.AddNotification($"Song has been added to {playlist.PlaylistName} Playlist.", NotificationType.SUCCESS);
            return null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool IsAuthorizedToOperate(Song song)
        {
            bool isAdmin = this.User.IsInRole("Admin");
            bool isUploader = song.IsUploader(this.User.Identity.Name);

            return isAdmin || isUploader;
        }
    }
}
