using Panda_Player.Extensions;
using Panda_Player.Models;
using Panda_Player.Models.Manage.Admin;
using Panda_Player.Models.PandaPlayer;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Panda_Player.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class GenreController : Controller
    {
        // GET: Genre
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: Genre/List
        public ActionResult List()
        {
            var db = new ApplicationDbContext();

            var genres = db.Genres.OrderBy(c => c.Name).ToList();

            return View(genres);
        }

        // GET: Genre/Create
        public ActionResult Create()
        {
            return View();
        }

        // Post: Genre/List
        [HttpPost]
        public ActionResult Create(Genre genre)
        {
            if (genre.Name == null)
            {
                this.AddNotification("Name is empty!", NotificationType.ERROR);
                return View(genre);
            }

            var db = new ApplicationDbContext();

            var currGenres = new List<Genre>();
            
            foreach (var gen in db.Genres)
            {
                currGenres.Add(gen);
            }

            var genreExists = CheckIfGenreExists(currGenres, genre.Name.ToString());

            if(ModelState.IsValid && !genreExists)
            {
                db.Genres.Add(genre);
                db.SaveChanges();

                this.AddNotification($"New genre '{genre.Name}' created!", NotificationType.SUCCESS);
                return RedirectToAction("List");
            }

            this.AddNotification($"Genre with name '{genre.Name}' already exists!", NotificationType.ERROR);
            return View(genre);
        }

        // GET: Genre/Edit
        public ActionResult Edit (int? id)
        {
            if (id == null)
            {
                this.AddNotification("No id specified!", NotificationType.ERROR);
                return RedirectToAction("List");
            }

            var db = new ApplicationDbContext();

            var genre = db.Genres.Where(g => g.Id == id).First();

            if (genre == null)
            {
                this.AddNotification("Invalid genre!", NotificationType.ERROR);
                return RedirectToAction("List");
            }

            return View(genre);
        }

        // POST: Genre/Edit
        [HttpPost]
        public ActionResult Edit(Genre genre)
        {
            if (genre.Name == null)
            {
                this.AddNotification("Name is empty!", NotificationType.ERROR);
                return View(genre);
            }

            var db = new ApplicationDbContext();

            var currGenres = new List<Genre>();

            foreach (var gen in db.Genres)
            {
                currGenres.Add(gen);
            }

            var genreExists = CheckIfGenreExists(currGenres, genre.Name.ToString());

            if (ModelState.IsValid && !genreExists)
            {
                var updateGenreId = db.Genres.First(i => i.Id == genre.Id);
                updateGenreId.Name = genre.Name;
                db.SaveChanges();

                this.AddNotification("Genre edited!", NotificationType.SUCCESS);
                return RedirectToAction("List");
            }

            this.AddNotification($"Genre with name '{genre.Name}' already exists!", NotificationType.ERROR);
            return View(genre);
        }

        //GET: Genre/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                this.AddNotification("No id specified!", NotificationType.ERROR);
                return RedirectToAction("List");
            }

            var db = new ApplicationDbContext();

            var genre = db.Genres.Where(g => g.Id == id).First();

            if (genre == null)
            {
                this.AddNotification("Invalid genre ID!", NotificationType.ERROR);
                return RedirectToAction("List");
            }

            this.AddNotification($"Deleting genre {genre.Name} will delete all songs in it!", NotificationType.WARNING);
            return View(genre);
        }

        // POST: Genre/Delete
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                this.AddNotification("No id specified!", NotificationType.ERROR);
                return RedirectToAction("List");
            }

            var db = new ApplicationDbContext();

            var genre = db.Genres.FirstOrDefault(g => g.Id == id);

            if (genre == null)
            {
                this.AddNotification("Invalid genre ID!", NotificationType.ERROR);
                return RedirectToAction("List");
            }

            var songs = db.Songs.ToList();
            
            var songsToRemove = new List<Song>();

            foreach (var song in songs.Where(s => s.GenreId == id))
            {
                songsToRemove.Add(song);
            }

            db.Genres.Remove(genre);
            db.SaveChanges();

            this.AddNotification($"Genre {genre.Name} successfully deleted.", NotificationType.INFO);
            return RedirectToAction("List");
        }

        private bool CheckIfGenreExists (List<Genre> genres, string genre)
        {
            foreach (var gen in genres)
            {
                if (gen.Name == genre)
                {
                    return true;
                }
            }

            return false;
        }
    }
}