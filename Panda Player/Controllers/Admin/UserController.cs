using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Panda_Player.Models;
using Panda_Player.Models.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Panda_Player.Controllers.Admin
{
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
    }
}