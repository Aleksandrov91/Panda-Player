using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Panda_Player.Extensions;
using Panda_Player.Models;
using Panda_Player.Models.Manage.Admin;
using Panda_Player.Models.PandaPlayer;
using Panda_Player.Models.ViewModels;
using Panda_Player.Models.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Panda_Player.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: User
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: User/List
        public ActionResult List(int page = 1, string search = "")
        {
            var db = new ApplicationDbContext();

            var usersPerPage = 5;

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            var userViewModels = new List<UserDetailsViewModel>();
            if(search == "")
            {
                userViewModels = db.Users
                .OrderBy(n => n.Email)
                .Skip((page - 1) * usersPerPage)
                .Take(usersPerPage)
                .ToArray()
                .Select(user => new UserDetailsViewModel()
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Roles = string.Join(", ", userManager.GetRoles(user.Id)),
                    LastLoginnDate = user.UserAccessControl.LastLogin.ToLongTimeString(),
                    BanEndDate = user.UserAccessControl.BanEndTime,
                    TotalSongs = user.UploadedSongs.Count,
                    TotalPlaylists = user.Playlists.Count,
                })
            .ToList();
            }
            else
            {
               userViewModels = db.Users
                .OrderBy(n => n.Email)
                .Where(u => u.FullName.ToLower().Contains(search.ToLower()))
                .Skip((page - 1) * usersPerPage)
                .Take(usersPerPage)
                .ToArray()
                .Select(user => new UserDetailsViewModel()
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Roles = string.Join(", ", userManager.GetRoles(user.Id)),
                    LastLoginnDate = user.UserAccessControl.LastLogin.ToLongTimeString(),
                    BanEndDate = user.UserAccessControl.BanEndTime,
                    TotalSongs = user.UploadedSongs.Count,
                    TotalPlaylists = user.Playlists.Count,
                })
            .ToList();
            }

            ViewBag.CurrPage = page;
            ViewBag.TotalPages = search == "" ? Math.Ceiling((double)db.Users.ToList().Count / usersPerPage) : Math.Ceiling((double)db.Users.Where(u => u.FullName.ToLower().Contains(search.ToLower())).ToList().Count() / usersPerPage);
            ViewBag.UsersPerPage = usersPerPage;
            ViewBag.TotalUsersFound = db.Users
                .ToList()
                .Where(u => u.FullName.ToLower().Contains(search.ToLower()))
                .OrderBy(n => n.Email)
                .ToList()
                .Count;
                
            //var users = db.Users.Include(r => r.Roles).Select(user => db.Roles.Where(role => user.Roles.Select(a => a.RoleId).ToArray().Contains(role.Id)).Select(a => a.Name).ToArray()).ToArray();

            return View(userViewModels);
        }

        // GET: User/Edit
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                this.AddNotification("No id specified!", NotificationType.ERROR);
                return Redirect("/User/List");
            }

            var db = new ApplicationDbContext();

            var user = db.Users.Find(id);

            if (user == null)
            {
                this.AddNotification("No user found!", NotificationType.ERROR);
                return Redirect("/User/List");
            }

            var viewModel = new EditUserViewModel();
            viewModel.User = user;
            viewModel.Roles = GetRoles(user, db);

            return View(viewModel);
        }

        // Post: User/Edit
        [HttpPost]
        public ActionResult Edit(string id, EditUserViewModel viewModel)
        {
            if (id == null)
            {
                this.AddNotification("No id specified!", NotificationType.ERROR);
                return Redirect("/User/List");
            }

            var db = new ApplicationDbContext();

            var user = db.Users.Find(id);

            if (user == null)
            {
                this.AddNotification("No user found!", NotificationType.ERROR);
                return Redirect("/User/List");
            }

            if (!string.IsNullOrEmpty(viewModel.Password))
            {
                var hasher = new PasswordHasher();
                var passwordHash = hasher.HashPassword(viewModel.Password);
                user.PasswordHash = passwordHash;
            }

            user.Email = viewModel.User.Email;
            user.FullName = viewModel.User.FullName;
            this.SetUserRole(viewModel, user, db);

            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            //If the user edits himself logout
            var currUser = User.Identity.Name.ToString();
            if (currUser == viewModel.User.Email)
            {

                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home");
            }

            this.AddNotification($"User '{viewModel.User.FullName}' edited!", NotificationType.SUCCESS);
            return Redirect("/User/List");
        }

        // Get User/Delete
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                this.AddNotification("No id specified!", NotificationType.ERROR);
                return Redirect("/User/List");
            }

            using (var db = new ApplicationDbContext())
            {
                var user = db.Users.Find(id);

                if (user == null)
                {
                    this.AddNotification("Invalid user id!", NotificationType.ERROR);
                    return Redirect("/User/List");
                }

                var viewModel = new DeleteUserViewModel();
                viewModel.UserName = db.Users.Where(u => u.Id == id).Select(u => u.UserName).FirstOrDefault();
                viewModel.FullName = db.Users.Where(u => u.Id == id).Select(u => u.FullName).FirstOrDefault();
                viewModel.UploadedSongsCount = user.UploadedSongs.Count;
                viewModel.PlaylistCount = user.Playlists.Count;

                this.AddNotification($"Deleting the user '{viewModel.FullName}' will delete all songs and playlists created by him!", NotificationType.WARNING);
                this.AddNotification($"Total songs '{viewModel.UploadedSongsCount}'!", NotificationType.INFO);
                this.AddNotification($"Total playlists '{viewModel.PlaylistCount}'!", NotificationType.INFO);

                return View(viewModel);
            }
        }

        // POST User/Delete
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeletePost(string id)
        {
            if (id == null)
            {
                this.AddNotification("Invalid user id!", NotificationType.ERROR);
                return Redirect("/User/List");
            }

            using (var db = new ApplicationDbContext())
            {
                var ids = db.Users.Select(i => i.Id).ToList();
                if (!ids.Contains(id))
                {
                    this.AddNotification("User not found!", NotificationType.ERROR);
                    return Redirect("/User/List");
                }

                var user = db.Users.Find(id);

                var userSongs = db.Songs.Where(s => s.Uploader.Id == user.Id);

                foreach (var song in userSongs)
                {
                    db.Songs.Remove(song);
                }

                var userPlaylists = db.Playlists.Where(p => p.Creator.Id == user.Id);

                foreach (var playlist in userPlaylists)
                {
                    db.Playlists.Remove(playlist);
                }

                db.Users.Remove(user);
                db.SaveChanges();

                this.AddNotification($"User '{user.FullName}' deleted!", NotificationType.INFO);
            }

            return Redirect("/User/List");
        }

        // GET: User/SongsEdit
        public ActionResult SongsEdit(string id, int page = 1, string search = "")
        {
            if (id == null)
            {
                this.AddNotification("Invalid user id!", NotificationType.ERROR);
                return Redirect("/User/List");
            }

            var songsPerPage = 5;

            using (var db = new ApplicationDbContext())
            {
                var ids = db.Users.Select(i => i.Id).ToList();
                if (!ids.Contains(id))
                {
                    this.AddNotification("User not found!", NotificationType.ERROR);
                    return Redirect("/User/List");
                }

                var user = db.Users.Find(id); // Find user

                var userSongs = new List<Song>();

                if (search == "")
                {
                    userSongs = db.Songs.Where(s => s.Uploader.Id == user.Id)
                    .OrderBy(s => s.UploadDate)
                    .Skip((page - 1) * songsPerPage)
                    .Take(songsPerPage)
                    .ToList();
                }
                else
                {
                   userSongs = db.Songs.Where(s => s.Uploader.Id == user.Id)
                    .OrderBy(s => s.UploadDate)
                    .Where(s => s.Artist.ToLower().Contains(search.ToLower()) 
                             || s.Title.ToLower().Contains(search.ToLower()))
                    .Skip((page - 1) * songsPerPage)
                    .Take(songsPerPage)
                    .ToList();
                }

                ViewBag.CurrPage = page;
                ViewBag.TotalPages = search == "" ? Math.Ceiling((double)db.Songs.ToList().Count / songsPerPage) : Math.Ceiling((double)userSongs.Count / songsPerPage);
                ViewBag.SongsPerPage = songsPerPage;
                ViewBag.UserId = user.Id;
                ViewBag.TotalSongsFound = db.Songs
                                            .Where(s => s.Uploader.Id == user.Id)
                                            .ToList()
                                            .Where(s => s.Artist.ToLower().Contains(search.ToLower())
                                                      || s.Title.ToLower().Contains(search.ToLower()))
                                            .ToList()
                                            .Count;

                var viewModel = new UserSongsViewModel();
                viewModel.Songs = userSongs;
                viewModel.Uploader = db.Users.Where(i => i.Id == id).Select(u => u.FullName).FirstOrDefault();

                if (userSongs.Count == 0)
                {
                    this.AddNotification($"User '{user.FullName}' has not uploaded any songs!", NotificationType.INFO);
                }

                return View(viewModel);
            }
        }

        // GET: User/SongToEdit
        public ActionResult SongToBeEdited(int? id)
        {
            if (id == null)
            {
                this.AddNotification("Song does not exist", NotificationType.ERROR);
                return RedirectToAction("SongsEdit");
            }

            Song song = db.Songs.Find(id);

            if (song == null)
            {
                this.AddNotification("Song does not exist", NotificationType.ERROR);
                return RedirectToAction("SongsEdit");
            }

            if (!IsAuthorizedToOperate(song))
            {
                this.AddNotification("You don't have permission to edit this song.", NotificationType.ERROR);
                return RedirectToAction("SongsEdit");
            }

            var viewModel = new SongViewModel();
            viewModel.Id = song.Id;
            viewModel.Artist = song.Artist;
            viewModel.Title = song.Title;
            viewModel.Description = song.Description;
            viewModel.GenreId = song.GenreId;
            viewModel.Genre = db.Genres.OrderBy(g => g.Name).ToList();
            viewModel.Tags = string.Join(", ", song.Tags.Select(t => t.Name));

            return View(viewModel);
        }

        // POST: User/SongToEdit/
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SongToBeEdited(SongUploadEditViewModel song)
        {
            var currentSong = db.Songs.FirstOrDefault(s => s.Id == song.Id);

            if (currentSong == null)
            {
                this.AddNotification("Song does not exist", NotificationType.ERROR);
                return RedirectToAction("SongsEdit");
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
                return RedirectToAction("SongsEdit");
            }

            db.SaveChanges();

            this.AddNotification($"Song '{song.Artist} - {song.Title}' was updated.", NotificationType.SUCCESS);

            return RedirectToAction($"SongDetails/{currentSong.Id}");
        }

        // GET: User/SongDetails
        public ActionResult SongDetails(int? id)
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

        // GET: User/PlaylistsList
        public ActionResult PlaylistsList(string id, string search = "", int page = 1)
        {
            if (id == null)
            {
                this.AddNotification("Invalid user id.", NotificationType.ERROR);
                return RedirectToAction("MySongs");
            }

            var playlistsPerPage = 5;

            var playlists = new List<Playlist>();
        
            if (search == "")
            {
                playlists = db.Playlists
                    .OrderBy(p => p.PlaylistName)
                    .Skip((page - 1) * playlistsPerPage)
                    .Take(playlistsPerPage)
                    .ToList();
            }
            else
            {
                playlists = db.Playlists
                    .Where(u => u.Creator.Id == id)
                    .ToArray()
                    .Where(p => p.PlaylistName.ToLower().Contains(search.ToLower()))
                    .OrderBy(p => p.PlaylistName)
                    .Skip((page - 1) * playlistsPerPage)
                    .Take(playlistsPerPage)
                    .ToList();
            }

            var viewModel = new UserPlaylistsViewModel();
            viewModel.PlaylistsList = playlists;
            ViewBag.PlaylistsPerPage = playlistsPerPage;
            ViewBag.UserId = id;
            ViewBag.CurrPage = page;
            ViewBag.TotalPages = search == "" ? Math.Ceiling((double)db.Playlists.Where(u => u.Creator.Id == id).ToList().Count / playlistsPerPage) :
                Math.Ceiling((double)db.Playlists.Where(u => u.Creator.Id == id).ToList().Where(p => p.PlaylistName.ToLower().Contains(search.ToLower())).ToList().Count / playlistsPerPage) ;
            ViewBag.TotalPlaylistsFound = db.Playlists.Where(u => u.Creator.Id == id).ToList().Where(p => p.PlaylistName.ToLower().Contains(search.ToLower())).ToList().Count;

            if (playlists.Count == 0)
            {
                var userName = db.Playlists.Where(u => u.Creator.Id == id).Select(n => n.Creator.FullName).FirstOrDefault();

                this.AddNotification($"User '{userName}' has not uploaded any playlsts!", NotificationType.INFO);
            }

            return View(viewModel);
        }

        // GET: User/PlaylistToBeEdited
        public ActionResult PlaylistToBeEdited (int? id)
        {
            if (id == null)
            {
                this.AddNotification("Invalid playlist id!", NotificationType.ERROR);
                return RedirectToAction("Index");
            }

            Playlist playlist = db.Playlists.Find(id);

            if (playlist == null)
            {
                this.AddNotification("Invalid playlist id!", NotificationType.ERROR);
                return RedirectToAction("Index");
            }

            if (!IsAuthorizedToOperate(playlist))
            {
                this.AddNotification("You are not authorized to edit this playlist!", NotificationType.ERROR);
                return RedirectToAction("Index");
            }

            var model = new PlaylistViewModel
            {
                Id = playlist.Id,
                PlaylistName = playlist.PlaylistName,
                IsPublic = playlist.IsPublic
            };

            return View(model);
        }

        // POST: User/PlaylistToBeEdited
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PlaylistToBeEdited(PlaylistViewModel model)
        {
            if (ModelState.IsValid)
            {
                var playlistToEdit = db.Playlists.Find(model.Id);

                playlistToEdit.PlaylistName = model.PlaylistName;
                playlistToEdit.IsPublic = model.IsPublic;

                db.Entry(playlistToEdit).State = EntityState.Modified;
                db.SaveChanges();

                this.AddNotification($"Plylist {model.PlaylistName} edited.", NotificationType.INFO);
                return RedirectToAction($"PlaylistDetails/{playlistToEdit.Id}");
            }

            return View(model);
        }

        // GET: User/PlaylistDetails
        [Authorize]
        public ActionResult PlaylistDetails(int? id)
        {
            if (id == null)
            {
                this.AddNotification("Invalid playlist id!", NotificationType.ERROR);
                return RedirectToAction("Index");
            }

            Playlist playlist = db.Playlists.Include(a => a.Songs).FirstOrDefault(a => a.Id == id);

            //ConvertToM3u(playlist);
            if (playlist == null)
            {
                this.AddNotification("Invalid playlist id!", NotificationType.ERROR);
                return RedirectToAction("Index");
            }

            if (!playlist.IsPublic)
            {
                if (!IsAuthorizedToOperate(playlist))
                {
                    this.AddNotification("Playlist is not public!", NotificationType.ERROR);
                    return RedirectToAction("Index");
                }
            }

            return View(playlist);
        }

        [HttpPost]
        public ActionResult DeleteFromPlaylist(int songId, int playlistId)
        {
            var playlist = db.Playlists.Include(s => s.Songs).FirstOrDefault(p => p.Id == playlistId);
            var song = playlist.Songs.FirstOrDefault(s => s.Id == songId);
            playlist.Songs.Remove(song);
            db.SaveChanges();

            this.AddNotification($"Song '{song.Artist} - {song.Title}' deleted from '{playlist.PlaylistName}' playlist.", NotificationType.INFO);

            return RedirectToAction($"PlaylistDetails/{playlistId}");
        }

        // Get User/BanUser
        public ActionResult BanUser(string id)
        {
            if (id == null)
            {
                this.AddNotification("No id specified!", NotificationType.ERROR);
                return Redirect("/User/List");
            }

            using (var db = new ApplicationDbContext())
            {
                var user = db.Users.Find(id);

                if (user == null)
                {
                    this.AddNotification("Invalid user id!", NotificationType.ERROR);
                    return Redirect("/User/List");
                }

                var viewModel = new UserBanViewModel();
                viewModel.Id = id;
                viewModel.UserName = db.Users.Where(u => u.Id == id).Select(u => u.UserName).FirstOrDefault();
                viewModel.FullName = db.Users.Where(u => u.Id == id).Select(u => u.FullName).FirstOrDefault();
                viewModel.UploadedSongsCount = user.UploadedSongs.Count;
                viewModel.PlaylistCount = user.Playlists.Count;
                viewModel.BanEndDate = user.UserAccessControl.BanEndTime;

                this.AddNotification("Select end date of ban.", NotificationType.INFO);
                this.AddNotification("To enable user set the ban date to 'Now'.", NotificationType.INFO);

                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                var userRoles = string.Join(", ", userManager.GetRoles(id));

                if (userRoles.Contains("Admin"))
                {
                    this.AddNotification("Selected user is 'Admin'", NotificationType.WARNING);
                }

                return View(viewModel);
            }
        }

        // POST User/BanUser
        [HttpPost]
        public ActionResult BanUser(UserBanViewModel userBanViewModel)
        {
            if (userBanViewModel.Id == null)
            {
                this.AddNotification("Invalid user id!", NotificationType.ERROR);
                return Redirect("/User/List");
            }

            using (var db = new ApplicationDbContext())
            {
                var ids = db.Users.Select(i => i.Id).ToList();

                if (!ids.Contains(userBanViewModel.Id))
                {
                    this.AddNotification("User not found!", NotificationType.ERROR);
                    return Redirect("/User/List");
                }

                var user = db.Users.Find(userBanViewModel.Id);

                // Get data for user to ban from database
                var currBanDate = db.Users.Where(u => u.Id == userBanViewModel.Id).Select(b => b.UserAccessControl.BanEndTime).FirstOrDefault();
                var currUser = db.Users.Where(u => u.Email == userBanViewModel.UserName).FirstOrDefault();

                if (userBanViewModel.BanEndDate != currBanDate && userBanViewModel.BanEndDate > DateTime.Now)
                {
                    // Save new ban date to user
                    currUser.UserAccessControl.BanEndTime = userBanViewModel.BanEndDate;

                    db.Users.Attach(currUser);
                    var entity = db.Entry(currUser.UserAccessControl);
                    entity.Property(b => b.BanEndTime).IsModified = true;
                    db.SaveChanges();

                    this.AddNotification($"User '{userBanViewModel.FullName}' successfully banned till '{userBanViewModel.BanEndDate.ToLongDateString()} {userBanViewModel.BanEndDate.ToLongTimeString()}'!", NotificationType.INFO);

                    return Redirect("/User/List");
                }
                else
                {
                    currUser.UserAccessControl.BanEndTime = userBanViewModel.BanEndDate;

                    db.Users.Attach(currUser);
                    var entity = db.Entry(currUser.UserAccessControl);
                    entity.Property(b => b.BanEndTime).IsModified = true;
                    db.SaveChanges();

                    this.AddNotification($"User '{currUser.FullName}' ban was removed! Select date greater than 'Now' date to ban the user!", NotificationType.WARNING);

                    return Redirect("/User/List");
                }
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void SetUserRole(EditUserViewModel viewModel, ApplicationUser user, ApplicationDbContext db)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            foreach (var role in viewModel.Roles)
            {
                if (role.IsSelected)
                {
                    userManager.AddToRole(user.Id, role.Name);
                }
                else
                {
                    userManager.RemoveFromRole(user.Id, role.Name);
                }
            }
        }

        private IList<Role> GetRoles(ApplicationUser user, ApplicationDbContext db)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            var roles = db.Roles.Select(r => r.Name)
                .OrderBy(r => r)
                .ToList();

            var userRoles = new List<Role>();

            foreach (var roleName in roles)
            {
                var role = new Role { Name = roleName };

                if (userManager.IsInRole(user.Id, roleName))
                {
                    role.IsSelected = true;
                }

                userRoles.Add(role);
            }

            return userRoles;
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

        private bool IsAuthorizedToOperate(Playlist playlist)
        {
            bool isAdmin = this.User.IsInRole("Admin");
            bool isUploader = playlist.IsCreator(this.User.Identity.Name);

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
