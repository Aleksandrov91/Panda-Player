using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Panda_Player.Extensions;
using Panda_Player.Models;
using Panda_Player.Models.Identity;
using Panda_Player.Models.Manage.Admin;
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
        // GET: User
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: User/List
        public ActionResult List()
        {
            var db = new ApplicationDbContext();

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            var userViewModels = db.Users.ToArray().Select(user => new UserDetailsViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Roles = string.Join(", ", userManager.GetRoles(user.Id)),
            })
            .ToArray();

            //var users = db.Users.Include(r => r.Roles).Select(user => db.Roles.Where(role => user.Roles.Select(a => a.RoleId).ToArray().Contains(role.Id)).Select(a => a.Name).ToArray()).ToArray();

            return View(userViewModels);
        }

        // GET: User/Edit
        public ActionResult Edit (string id)
        {
            if (id == null)
            {
                return Redirect("/User/List");
            }

            var db = new ApplicationDbContext();

            var user = db.Users.Find(id);

            if (user == null)
            {
                return Redirect("/User/List");
            }

            var viewModel = new EditUserViewModel();
            viewModel.User = user;
            viewModel.Roles = GetRoles(user, db);

            return View(viewModel);
        }

        // Post: User/Edit
        [HttpPost]
        public ActionResult Edit (string id, EditUserViewModel viewModel)
        {
            var db = new ApplicationDbContext();

            var user = db.Users.Find(id);

            if(user == null)
            {
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

                this.AddNotification("Deleting the user will delete all songs and playlists created by him!", NotificationType.WARNING);

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
            }

            this.AddNotification("User and all data have been deleted successfully!", NotificationType.INFO);
            return Redirect("/User/List");
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
                if(role.IsSelected)
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
    }
}