using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Panda_Player.Models;
using Panda_Player.Models.PandaPlayer;
using Microsoft.AspNet.Identity;
using Panda_Player.Extensions;
using System;

namespace Panda_Player.Controllers
{
    public class PlaylistsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Playlists
        public ActionResult Index()
        {
            var currentUser = this.User.Identity.GetUserId();
            var myPlaylists = db.Playlists.Where(u => u.Author.Id == currentUser).ToList();
            return View(myPlaylists);
        }

        // GET: Playlists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Playlist playlist = db.Playlists.Include(a => a.Songs).FirstOrDefault(a => a.Id == id);
            if (playlist == null)
            {
                return HttpNotFound();
            }

            return View(playlist);
        }

        // GET: Playlists/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Playlists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PlaylistName,IsPublic")] Playlist playlist)
        {
            if (ModelState.IsValid)
            {
                string authorId = this.User.Identity.GetUserId();
                var author = db.Users.Find(authorId);

                playlist.Author = author;
                playlist.DateCreated = DateTime.Now;

                db.Playlists.Add(playlist);
                db.SaveChanges();
                return RedirectToAction("MySongs", "Songs");
            }

            return View(playlist);
        }

        // GET: Playlists/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Playlist playlist = db.Playlists.Find(id);
            if (playlist == null)
            {
                return HttpNotFound();
            }
            return View(playlist);
        }

        // POST: Playlists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PlaylistName,IsPublic")] Playlist playlist)
        {
            if (ModelState.IsValid)
            {
                db.Entry(playlist).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(playlist);
        }

        // GET: Playlists/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Playlist playlist = db.Playlists.Find(id);
            if (playlist == null)
            {
                return HttpNotFound();
            }
            return View(playlist);
        }

        // POST: Playlists/Delete/5
        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            var result = false;
            Playlist playlist = db.Playlists.Find(id);
            db.Playlists.Remove(playlist);
            db.SaveChanges();
            result = true;

            this.AddNotification("The Playlist has been deleted successfully.", NotificationType.SUCCESS);
            return Json(new { Success = true });
        }

        public ActionResult DeleteFromPlaylist(int songId, int playlistId)
        {
            var playlist = db.Playlists.Include(s => s.Songs).FirstOrDefault(p => p.Id == playlistId);
            var song = playlist.Songs.FirstOrDefault(s => s.Id == songId);
            playlist.Songs.Remove(song);
            db.SaveChanges();
            
            return RedirectToAction("Index");
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
