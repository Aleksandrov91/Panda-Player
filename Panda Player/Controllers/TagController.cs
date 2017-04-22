using Panda_Player.Extensions;
using Panda_Player.Models;
using Panda_Player.Models.PandaPlayer;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Panda_Player.Controllers
{
    public class TagController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }


        // GET: Tag
        public ActionResult List(int? id)
        {
            if (id == null)
            {
                this.AddNotification("Song does not exist", NotificationType.ERROR);
                return RedirectToAction("MySongs");
            }

            using (var db = new ApplicationDbContext())
            {
               
            }

            return View();
        }
    }
}