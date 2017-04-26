using Microsoft.AspNet.Identity;
using Panda_Player.Extensions;
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

            var lastAddedSong = lastAddedSongs[lastAddedSongs.Count - 1];

            var indexModel = new IndexViewModel
            {

                Playlists = lastAddedPlaylists,
                Songs = lastAddedSongs,
                UserPlaylists = userPlaylists,
                LastAddedSong = lastAddedSong
            };

            
            return View(indexModel);
        }

        [HttpGet]
        public ActionResult List(string id)
        {
            switch (id)
            {
                case "Songs":
                    var allSongs = db.Songs.OrderBy(s => s.Id).ToList();
                    var songsTotalPages = Math.Ceiling((decimal)allSongs.Count() / 6);
                    var currentPageSongs = allSongs.Take(6).ToList();

                    var songsModel = new ListAllSongsViewModel
                    {
                        Songs = currentPageSongs,
                        UserPlaylists = LoggedUser(),
                        LastPage = songsTotalPages
                    };

                    return View("ListSongs", songsModel);

                case "Playlists":
                    var allPlaylists = db.Playlists.Where(p => p.IsPublic).OrderBy(p => p.Id).ToList();
                    var playlistsTotalPages = Math.Ceiling((decimal)allPlaylists.Count() / 6);
                    var currentPagePlaylists = allPlaylists.Take(6).ToList();

                    var playlistsModel = new ListAllPlaylistsViewModel
                    {
                        Playlists = currentPagePlaylists,
                        LastPage = playlistsTotalPages
                    };

                    return View("ListPlaylists", playlistsModel);

                default:
                    this.AddNotification("Somethings get wrong", NotificationType.WARNING);
                    return null;
            }
        }

        [HttpPost]
        public ActionResult ListSongs(ListAllSongsViewModel songsModel)
        {
            var allSongs = db.Songs.OrderBy(s => s.Id).ToList();
            var currentPageSongs = allSongs.Skip((songsModel.CurrentPage - 1) * 6).Take(6).ToList();

            songsModel.Songs = currentPageSongs;
            songsModel.UserPlaylists = LoggedUser();

            return PartialView("SongPartial", songsModel);
        }

        [HttpPost]
        public ActionResult ListPlaylists(ListAllPlaylistsViewModel playlistsModel)
        {
            var allPlaylists = db.Playlists.Where(p => p.IsPublic).OrderBy(p => p.Id).ToList();
            var currentPagePlaylists = allPlaylists.Skip((playlistsModel.CurrentPage - 1) * 6).Take(6).ToList();

            playlistsModel.Playlists = currentPagePlaylists;

            return PartialView("PlaylistPartial", playlistsModel);
        }

        public ActionResult Search(string query, string type)
        {
            switch (type)
            {
                case "songs":
                    var allSongs = db.Songs;

                    var songResult = allSongs
                        .AsQueryable()
                        .Where(song => song.Artist.ToLower().Contains(query.ToLower()) ||
                            song.Title.ToLower().Contains(query.ToLower()))
                            .ToList();

                    var songsPerPage = songResult.Take(6).ToList();
                    var songsTotalPages = Math.Ceiling((decimal)songResult.Count() / 6);

                    var songModel = new ListAllSongsViewModel
                    {
                        Songs = songsPerPage,
                        UserPlaylists = LoggedUser(),
                        LastPage = songsTotalPages,
                    };

                    if (query == string.Empty)
                    {
                        songModel.Songs = allSongs.ToList();
                        songModel.LastPage = Math.Ceiling((decimal)allSongs.Count() / 6);
                    }

                    return PartialView("SearchSongs", songModel);

                case "playlists":
                    var publicPlaylists = db.Playlists.Where(playlist => playlist.IsPublic).ToList();

                    var playlistResult = publicPlaylists
                        .AsQueryable()
                        .Where(playlist => playlist.PlaylistName.ToLower().Contains(query.ToLower()))
                        .ToList();

                    var playlistsPerPage = playlistResult.Take(6).ToList();
                    var playlistsTotalPages = Math.Ceiling((decimal)playlistResult.Count() / 6);

                    var playlistModel = new ListAllPlaylistsViewModel
                    {
                        Playlists = playlistsPerPage,
                        LastPage = playlistsTotalPages,
                    };

                    if (query == string.Empty)
                    {
                        playlistModel.Playlists = publicPlaylists;
                        playlistModel.LastPage = Math.Ceiling((decimal)publicPlaylists.Count() / 6);
                    }

                    return PartialView("SearchPlaylists", playlistModel);

                default:

                    return HttpNotFound();
            }  
        }

        [HttpPost]
        public ActionResult SearchSongs(ListAllSongsViewModel model)
        {
            return null;
        }

        [HttpPost]
        public ActionResult SearchPlaylists(string query, ListAllPlaylistsViewModel playlistsModel)
        {
            var publicPlaylists = db.Playlists.Where(playlist => playlist.IsPublic).ToList();

            var playlistResult = publicPlaylists
                .AsQueryable()
                .Where(playlist => playlist.PlaylistName.ToLower().Contains(query.ToLower()))
                .ToList();

            var currentPagePlaylists = publicPlaylists.Skip((playlistsModel.CurrentPage - 1) * 6).Take(6).ToList();
            playlistsModel.Playlists = currentPagePlaylists;
            return PartialView("PlaylistPartial", playlistsModel);

        }
        
        [HttpGet]
        public ActionResult Genres()
        {
            var allSongs = db.Songs.ToList();
            var allGenres = db.Genres.ToList();
            var songsTotalPages = Math.Ceiling((decimal)allSongs.Count() / 6);

            var songsModel = new ListAllSongsViewModel
            {
                Songs = allSongs,
                UserPlaylists = LoggedUser(),
                LastPage = songsTotalPages
            };

            ViewBag.Genres = allGenres;

            return View(songsModel);
        }

        [HttpPost]
        public ActionResult Genres(int id)
        {
            var selectedGenre = db.Genres.Select(i => i.Id).Contains(id);
            if (!selectedGenre)
            {
                return HttpNotFound();
            }

            var songsInGenre = db.Songs.Where(g => g.GenreId == id).ToList();
            var lastSongPage = Math.Ceiling((decimal)songsInGenre.Count() / 6);

            var model = new ListAllSongsViewModel
            {
                Songs = songsInGenre,
                UserPlaylists = LoggedUser(),
                LastPage = lastSongPage
            };

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