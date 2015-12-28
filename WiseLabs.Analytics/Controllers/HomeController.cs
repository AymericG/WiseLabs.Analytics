using System.Web.Mvc;

namespace WiseLabs.Analytics.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            Tracker.Track("1", "test", "init");
            Tracker.Track("1", "2015-07-10", "init");
            Tracker.Track("1", "2015-07-17", "init");
            Tracker.Track("1", "2015-07-10", "acquisition");
            Tracker.Track("1", "2015-07-17", "acquisition");
            Tracker.Track("1", "2015-07-10", "retention");
            Tracker.Track("1", "2015-07-17", "retention");
            return View();
        }
    }
}