using Microsoft.AspNet.Identity;
using Panda_Player.Models;
using Panda_Player.Models.PandaPlayer;
using Panda_Player.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Panda_Player.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var userPlaylists = LoggedUser();

            var lastAddedSongs = db.Songs
                .OrderByDescending(s => s.UploadDate)
                .Take(6)
                .ToList();

            var lastAddedPlaylists = db.Playlists
                .OrderByDescending(a => a.DateCreated)
                .Where(p => p.IsPublic)
                .Take(6).ToList();

            var result = new IndexViewModel
            {

                Playlists = lastAddedPlaylists,
                Songs = lastAddedSongs,
                UserPlaylists = userPlaylists
            };

            return View(result);
        }

        public ActionResult List(string id)
        {
            switch (id)
            {
                case "Songs":
                    var userPlaylists = LoggedUser();
                    var allSongs = db.Songs.OrderBy(s => s.Id).ToList();
                    var songsLastPage = Math.Ceiling((decimal)allSongs.Count() / 5);
                    var currentPageSongs = allSongs.Take(5).ToList();

                    var songsModel = new ListAllSongsViewModel
                    {
                        Songs = currentPageSongs,
                        UserPlaylists = userPlaylists,
                        CurrentPage = 1,
                        LastPage = songsLastPage
                    };

                    return View("ListSongs", songsModel);

                case "Playlists":
                    var allPlaylists = db.Playlists.Where(p => p.IsPublic).OrderBy(p => p.Id).ToList();
                    var lastPlaylistsPage = Math.Ceiling((decimal)allPlaylists.Count() / 5);
                    var currentPagePlaylists = allPlaylists.Take(5).ToList();

                    var playlistsModel = new ListAllPlaylistsViewModel
                    {
                        Playlists = currentPagePlaylists,
                        CurrentPage = 1,
                        LastPage = lastPlaylistsPage
                    };

                    return View("ListPlaylists", playlistsModel);

                default:
                    return HttpNotFound();
            }
        }

        [HttpPost]
        public ActionResult ListSongs(ListAllSongsViewModel songsModel)
        {
            var allSongs = db.Songs.OrderBy(s => s.Id).ToList();
            var currentPageSongs = allSongs.Skip((songsModel.CurrentPage - 1) * 5).Take(5).ToList();
            songsModel.Songs = currentPageSongs;
            songsModel.UserPlaylists = LoggedUser();
            return PartialView("SongPartial", songsModel);
        }

        [HttpPost]
        public ActionResult ListPlaylists(ListAllPlaylistsViewModel playlistsModel)
        {
            var allPlaylists = db.Playlists.Where(p => p.IsPublic).OrderBy(p => p.Id).ToList();
            var currentPagePlaylists = allPlaylists.Skip((playlistsModel.CurrentPage - 1) * 5).Take(5).ToList();
            playlistsModel.Playlists = currentPagePlaylists;
            return PartialView("PlaylistPartial", playlistsModel);
        }

        public ActionResult Search(string query)
        {
            var songs = db.Songs;

            var result = songs
                .AsQueryable()
                .Where(song => song.Artist.ToLower().Contains(query.ToLower()) ||
                    song.Title.ToLower().Contains(query.ToLower()))
                    .ToList();

            var model = new ListAllSongsViewModel
            {
                Songs = result,
                UserPlaylists = LoggedUser(),
                CurrentPage = 1,
                LastPage = 1
            };

            if (query == string.Empty)
            {
                model.Songs = songs.ToList();
            }

            return PartialView("SongPartial", model);
        }

        public List<Playlist> LoggedUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var currentUser = this.User.Identity.GetUserId();
                var userPlaylists = db.Playlists
                .Where(a => a.Creator.Id == currentUser)
                .ToList();
                return userPlaylists ?? new List<Playlist>();
            }

            return new List<Playlist>();
        }
    }
}