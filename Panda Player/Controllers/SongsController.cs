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
using Panda_Player.Models.Manage.Admin;
using System.IO;
using Panda_Player.Models.ViewModels;

namespace Panda_Player.Controllers
{
    [Authorize]
    public class SongsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        // GET: Songs
        public ActionResult MySongs()
        {
            var currentUser = this.User.Identity.GetUserId();
            var userSongs = db.Songs
                .Where(s => s.Uploader.Id == currentUser)
                .OrderByDescending(d => d.UploadDate)
                .Include(u => u.Uploader)
                .Include(s => s.Tags)
                .ToList();

            var playlists = db.Playlists
                .Where(a => a.Creator.Id == currentUser)
                .ToList();

            var songsPerPage = userSongs.Take(5).ToList();

            var lastPage = Math.Ceiling((decimal)userSongs.Count() / 5);

            var model = new ListAllSongsViewModel
            {
                Songs = songsPerPage,
                UserPlaylists = playlists,
                LastPage = lastPage
            };

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult MySongs(ListAllSongsViewModel model)
        {
            var currentUser = this.User.Identity.GetUserId();
            var userSongs = db.Songs
                .Where(s => s.Uploader.Id == currentUser)
                .OrderByDescending(d => d.UploadDate)
                .Include(u => u.Uploader)
                .Include(s => s.Tags)
                .ToList();

            var playlists = db.Playlists
                .Where(a => a.Creator.Id == currentUser)
                .ToList();

            var currentPageSongs = userSongs.Skip((model.CurrentPage - 1) * 5).Take(5).ToList();
            model.Songs = currentPageSongs;
            model.UserPlaylists = playlists;

            return PartialView("SongPartial", model);
        }

        // GET: Songs/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                this.AddNotification("Song does not exist", NotificationType.ERROR);
                return RedirectToAction("MySongs");
            }

            Song song = db.Songs
                .Where(s => s.Id == id)
                .Include(s => s.Uploader)
                .Include(s => s.Tags)
                .First();

            if (song == null)
            {
                this.AddNotification("Song does not exist", NotificationType.ERROR);
                return RedirectToAction("MySongs");
            }

            SongDetailsModel currSong = new SongDetailsModel();
            currSong.Id = song.Id;
            currSong.Artist = song.Artist;
            currSong.Title = song.Title;
            currSong.Genre = db.Genres.Where(g => g.Id == song.GenreId).Select(g => g.Name).First();
            currSong.Description = song.Description;
            currSong.UploadDate = song.UploadDate;
            currSong.Uploader = song.Uploader;
            currSong.Tags = song.Tags.ToList();

            return View(currSong);
        }

        // GET: Songs/Upload
        public ActionResult Upload()
        {
            var model = new SongViewModel();
            model.Genre = db.Genres.OrderBy(c => c.Name).ToList();

            //return View(model);
            return PartialView(model);
        }

        // POST: Songs/Upload
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Upload(SongUploadViewModel song, HttpPostedFileBase file)
        {   
            if (file != null)
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

                    bool isGenreIdValid = ValidateGenre(song.Genre);

                    if (!isGenreIdValid)
                    {
                        this.AddNotification("Invalid genre selected.", NotificationType.ERROR);
                        return View(song);
                    }

                    var currentSong = new Song
                    {
                        Artist = song.Artist,
                        Title = song.Title,
                        Description = song.Description,
                        UploaderId = currentUser,
                        SongPath = $"/Uploads/{fileName}",
                        UploadDate = DateTime.Now,
                        GenreId = song.Genre,                      
                    };

                    this.SetSongTagsOnUpload(currentSong, song, db);

                    if (!Directory.Exists(mappedPath))
                    {
                        Directory.CreateDirectory(mappedPath);
                    }

                    file.SaveAs(absoluteFilePath);

                    db.Songs.Add(currentSong);
                    db.SaveChanges();

                    this.AddNotification("The song has been upload successfully.", NotificationType.SUCCESS);
                    return RedirectToAction("MySongs", "Songs");
                }

                this.AddNotification("The File must be only mp3 or wav.", NotificationType.ERROR);
                return RedirectToAction("Upload");
            }

            this.AddNotification("The file cannot be null", NotificationType.ERROR);
            return RedirectToAction("Upload");
        }

        // GET: Songs/Edit/{id}
        [Authorize]
        public ActionResult Edit(int? id)
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

            if (!IsAuthorizedToOperate(song))
            {
                this.AddNotification("You don't have permission to edit this song.", NotificationType.ERROR);
                return RedirectToAction("MySongs");
            }

            var viewModel = new SongViewModel();
            viewModel.Artist = song.Artist;
            viewModel.Title = song.Title;
            viewModel.Description = song.Description;
            viewModel.GenreId = song.GenreId;
            viewModel.Genre = db.Genres.OrderBy(g => g.Name).ToList();
            viewModel.Tags = string.Join(", ", song.Tags.Select(t => t.Name));

            return View(viewModel);
        }

        // POST: Songs/Edit/
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SongUploadEditViewModel song)
        {
            var currentSong = db.Songs.FirstOrDefault(s => s.Id == song.Id);
            if (currentSong == null)
            {
                this.AddNotification("Song does not exist", NotificationType.ERROR);
                return RedirectToAction("MySongs");
            }

            currentSong.Id = song.Id;
            currentSong.Artist = song.Artist;
            currentSong.Title = song.Title;
            currentSong.Description = song.Description;
            currentSong.GenreId = song.GenreId;
            this.SetSongTagsOnUpload(currentSong, song, db);

            if (currentSong.Artist == null || currentSong.Title == null)
            {
                this.AddNotification("Fill all requred fields, marked with \"*\"", NotificationType.ERROR);
                return RedirectToAction("Edit");
            }

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

            Song song = db.Songs
                .Where(s => s.Id == id)
                .Include(u => u.UploaderId)
                .Include(g => g.Genre)
                .First();

            if (song == null)
            {
                this.AddNotification("Song does not exist", NotificationType.ERROR);
                return RedirectToAction("MySongs");
            }

            if (!IsAuthorizedToOperate(song))
            {
                this.AddNotification("You are not allowed to delete this song!", NotificationType.ERROR);
                return RedirectToAction("MySongs");
            }

            ViewBag.TagString = string.Join("; ", song.Tags.OrderBy(t => t.Name));

            return View(song);
        }

        // POST: Songs/Delete/5
        [HttpPost]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                this.AddNotification("Song does not exist!", NotificationType.ERROR);
                return RedirectToAction("MySongs");
            }

            string uploadDir = Server.MapPath("~/");
            Song song = db.Songs.Find(id);

            if (song == null)
            {
                this.AddNotification("Song does not exist", NotificationType.ERROR);
                return RedirectToAction("MySongs");
            }

            if (!IsAuthorizedToOperate(song))
            {
                this.AddNotification("You are not allowed to delete this song!", NotificationType.ERROR);
                return RedirectToAction("MySongs");
            }

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

        private void SetSongTagsOnUpload(Song currentSong, SongUploadEditViewModel song, ApplicationDbContext db)
        {
            if (song.Tags == null)
            {
                song.Tags = string.Empty;
            }

            var tagsStringSplit = song.Tags.Split(" ,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .OrderBy(t => t)
                .Select(t => t.ToLower())
                .Distinct();

            currentSong.Tags.Clear();

            foreach (var tagString in tagsStringSplit)
            {
                Tag tag = db.Tags.FirstOrDefault(t => t.Name == tagString);

                if (tag == null)
                {
                    tag = new Tag() { Name = tagString };
                    db.Tags.Add(tag);
                }

                currentSong.Tags.Add(tag);
            }
        }

        private void SetSongTagsOnUpload(Song currentSong, SongUploadViewModel song, ApplicationDbContext db)
        {
            if (song.Tags == null)
            {
                song.Tags = string.Empty;
            }

            var tagsStringSplit = song.Tags.Split(" ,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .OrderBy(t => t)
                .Select(t => t.ToLower())
                .Distinct();

            currentSong.Tags.Clear();

            foreach (var tagString in tagsStringSplit)
            {
                Tag tag = db.Tags.FirstOrDefault(t => t.Name == tagString);

                if (tag == null)
                {
                    tag = new Tag() { Name = tagString };
                    db.Tags.Add(tag);
                }

                currentSong.Tags.Add(tag);
            }
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

        private bool ValidateGenre(int genreId)
        {
            var db = new ApplicationDbContext();

            var genres = db.Genres.ToList();

            foreach (var genre in genres)
            {
                if (genre.Id == genreId)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
