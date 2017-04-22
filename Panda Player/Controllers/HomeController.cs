using Panda_Player.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Panda_Player.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {

            var lastSongs = db.Songs.OrderByDescending(s => s.UploadDate).Take(6).ToList();

            var playlists = db.Playlists.OrderByDescending(a => a.DateCreated)
                .Where(p => p.IsPublic)
                .Take(6)
                .ToList();


            ViewBag.Songs = lastSongs;
            ViewBag.Playlists = playlists;

            return View();
        }

        public ActionResult List(string id)
        {
            var db = new ApplicationDbContext();

            ViewBag.CurrentPage = 1;

            switch (id)
            {
                case "Songs":
                    var allSongs = db.Songs.OrderBy(s => s.Id).ToList();
                    ViewBag.LastPage = Math.Ceiling((decimal)allSongs.Count() / 5);
                    var currentPageSongs = allSongs.Take(5).ToList();
                    return View("ListSongs", currentPageSongs);
                case "Playlists":
                    var allPlaylists = db.Playlists.Where(p => p.IsPublic).OrderBy(p => p.Id).ToList();
                    ViewBag.LastPage = Math.Ceiling((decimal)allPlaylists.Count() / 5);
                    var currentPagePlaylists = allPlaylists.Take(5).ToList();
                    return View("ListPlaylists", currentPagePlaylists);
                default:
                    return HttpNotFound();
            }
        }

        [HttpPost]
        public ActionResult List(string id, int CurrentPage, int LastPage)
        {
            var db = new ApplicationDbContext();

            ViewBag.CurrentPage = CurrentPage;
            ViewBag.LastPage = LastPage;

            switch (id)
            {
                case "Songs":
                    var allSongs = db.Songs.OrderBy(s => s.Id).ToList();
                    ViewBag.LastPage = Math.Ceiling((decimal)allSongs.Count() / 5);
                    var currentPageSongs = allSongs.Skip((CurrentPage - 1) * 5).Take(5).ToList();
                    return PartialView("SongPartial", currentPageSongs);
                case "Playlists":
                    var allPlaylists = db.Playlists.Where(p => p.IsPublic).OrderBy(p => p.Id).ToList();
                    ViewBag.LastPage = Math.Ceiling((decimal)allPlaylists.Count() / 5);
                    var currentPagePlaylists = allPlaylists.Skip((CurrentPage - 1) * 5).Take(5).ToList();
                    return PartialView("PlaylistPartial", currentPagePlaylists);
                default:
                    return HttpNotFound();
            }
        }
    }
}