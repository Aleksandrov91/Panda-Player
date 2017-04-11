using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Panda_Player.Models;
using Panda_Player.Models.PandaPlayer;
using Microsoft.AspNet.Identity;

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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Playlist playlist = db.Playlists.Find(id);
            db.Playlists.Remove(playlist);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult AddSongs(int? id)
        {
            var songs = db.Songs.OrderByDescending(s => s.UploadDate).ToList();
            return View(songs);
        }

        [HttpPost]
        public ActionResult AddSongs()
        {
            //int playlistId = playlist.Id;

            return View();
        }

        [HttpPost]
        public ActionResult DeleteFromPlaylist(int id)
        {

            return View();
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
