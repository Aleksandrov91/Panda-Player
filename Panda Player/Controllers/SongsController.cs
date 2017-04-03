using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Panda_Player.Models;
using Panda_Player.Models.PandaPlayer;
using System.Web;
using System;
using System.IO;
using Microsoft.AspNet.Identity;

namespace Panda_Player.Controllers
{
    [Authorize]
    public class SongsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Songs
        public ActionResult Index()
        {
            var currentUser = this.User.Identity.GetUserId();
            var userSongs = db.Songs.Where(s => s.UploaderId == currentUser).ToList();

            return View(userSongs);
        }

        // GET: Songs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Song song = db.Songs.Find(id);
            if (song == null)
            {
                return HttpNotFound();
            }
            return View(song);
        }

        // GET: Songs/Upload
        public ActionResult Upload()
        {
            return View();
        }

        // POST: Songs/Upload
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Upload(Song song, HttpPostedFileBase file)
        {
            if (ModelState.IsValid && file != null)
            {
                if (file.ContentType == "audio/mpeg" || file.ContentType == "audio/wav")
                {
                    var curentUser = this.User.Identity.GetUserId();
                    var songsPath = "~/Uploads/";
                    var mappedPath = HttpContext.Server.MapPath(songsPath);
                    var fileName = Path.GetFileName(file.FileName);
                    var hash = curentUser.Substring(0, 6);

                    var currentSong = new Song
                    {
                        Author = song.Author,
                        Name = song.Name,
                        Description = song.Description,
                        UploaderId = curentUser,
                        SongPath = mappedPath + hash + "_" + fileName,
                        UploadDate = DateTime.Now
                    };

                    file.SaveAs(mappedPath + fileName);

                    db.Songs.Add(currentSong);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }                

                ViewBag.Error = "The File must be only mp3 or wav";
                return View(song);
            }

            return View(song);
        }

        // GET: Songs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Song song = db.Songs.Find(id);
            if (song == null)
            {
                return HttpNotFound();
            }
            return View(song);
        }

        // POST: Songs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,SongPath,Date")] Song song)
        {
            if (ModelState.IsValid)
            {
                db.Entry(song).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(song);
        }

        // GET: Songs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Song song = db.Songs.Find(id);
            if (song == null)
            {
                return HttpNotFound();
            }
            return View(song);
        }

        // POST: Songs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Song song = db.Songs.Find(id);
            db.Songs.Remove(song);
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
