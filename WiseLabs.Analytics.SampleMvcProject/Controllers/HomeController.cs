using System.Web.Mvc;

namespace WiseLabs.Analytics.SampleMvcProject.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Tracker.Track("1", "test", "init");
            SplitTesting.Experiment("1", "test", () => { }, () => { });
            return View();

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}