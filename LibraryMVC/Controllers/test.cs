using Microsoft.AspNetCore.Mvc;

namespace LibraryMVC.Controllers
{
    public class test : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
