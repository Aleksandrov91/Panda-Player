using Panda_Player.Models;
using System.Linq;
using System.Web.Mvc;

namespace Panda_Player.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {

            var lastSongs = db.Songs.OrderByDescending(s => s.UploadDate).Take(3);
            return View(lastSongs);
        }
    }
}