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
                    var songsPerPage = songResult.Take(5).ToList();
                    var lastSongPage = Math.Ceiling((decimal)songResult.Count() / 5);
                    var songModel = new ListAllSongsViewModel
                    {
                        Songs = songsPerPage,
                        UserPlaylists = LoggedUser(),
                        LastPage = lastSongPage
                    };

                    if (query == string.Empty)
                    {
                        songModel.Songs = allSongs.ToList();
                        songModel.LastPage = Math.Ceiling((decimal)allSongs.Count() / 5);
                    }

                    return PartialView("SearchSongs", songModel);

                case "playlists":
                    var publicPlaylists = db.Playlists.Where(playlist => playlist.IsPublic).ToList();

                    var playlistResult = publicPlaylists
                        .AsQueryable()
                        .Where(playlist => playlist.PlaylistName.ToLower().Contains(query.ToLower()))
                        .ToList();

                    var playlistsPerPage = playlistResult.Take(5).ToList();
                    var lastPlaylistPage = Math.Ceiling((decimal)playlistResult.Count() / 5);

                    var playlistModel = new ListAllPlaylistsViewModel
                    {
                        Playlists = playlistsPerPage,
                        LastPage = lastPlaylistPage
                    };

                    if (query == string.Empty)
                    {
                        playlistModel.Playlists = publicPlaylists;
                        playlistModel.LastPage = Math.Ceiling((decimal)publicPlaylists.Count() / 5);
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

        public ActionResult SearchPlaylists(string query, ListAllPlaylistsViewModel playlistsModel)
        {
            var publicPlaylists = db.Playlists.Where(playlist => playlist.IsPublic).ToList();

            var playlistResult = publicPlaylists
                .AsQueryable()
                .Where(playlist => playlist.PlaylistName.ToLower().Contains(query.ToLower()))
                .ToList();

            var currentPagePlaylists = publicPlaylists.Skip((playlistsModel.CurrentPage - 1) * 5).Take(5).ToList();
            playlistsModel.Playlists = currentPagePlaylists;
            return PartialView("PlaylistPartial", playlistsModel);

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