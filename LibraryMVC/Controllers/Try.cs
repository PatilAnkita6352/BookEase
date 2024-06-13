using Microsoft.AspNetCore.Mvc;

namespace LibraryMVC.Controllers
{
    public class Try : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
