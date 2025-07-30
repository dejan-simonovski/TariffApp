using Microsoft.AspNetCore.Mvc;

namespace TariffApp.Controllers
{
    public class FeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
