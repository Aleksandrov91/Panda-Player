using Panda_Player.Models;
using Panda_Player.Models.ViewModels;
using System.Linq;
using System.Web.Mvc;

namespace Panda_Player.Controllers
{
    public abstract class BaseController : Controller
    {
        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var viewResult = filterContext.Result as ViewResult;
            var partialViewResult = filterContext.Result as PartialViewResult;
            if (viewResult != null || partialViewResult != null)
            {

                var viewModel = viewResult != null ? (BaseViewModel)viewResult.Model : (BaseViewModel)partialViewResult.Model;
                if (viewModel != null)
                {
                    var db = new ApplicationDbContext();

                    var lastSong = db.Songs
                        .OrderByDescending(s => s.UploadDate)
                        .Select(p => p.SongPath)
                        .FirstOrDefault();
                    ViewBag.LastSong = lastSong;
                    viewModel.Path = lastSong;
                }
            }
        }
    }
}