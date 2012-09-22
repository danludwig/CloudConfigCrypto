using System.Web.Mvc;

namespace CloudConfigCrypto.Web.Controllers
{
    public class CloudConfigCryptoController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult CreateCertificates()
        {
            return View();
        }

        public ActionResult ImportCertificatesLocally()
        {
            return View();
        }
    }
}
