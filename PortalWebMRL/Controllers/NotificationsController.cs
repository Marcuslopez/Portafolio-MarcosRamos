using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PortalWebMRL.Controllers
{
    public class NotificationsController : Controller
    {
        // GET: NotificationsController

        [HttpGet]
        public IActionResult Index()
        {
            // La vista leerá localStorage con JS
            return View();
        }



    }
}
