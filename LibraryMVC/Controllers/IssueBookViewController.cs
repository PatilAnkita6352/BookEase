using Microsoft.AspNetCore.Mvc;

namespace LibraryMVC.Controllers
{
    public class IssueBookViewController : Controller
    {
        public bool isValid()
        {
            var Logsession = HttpContext.Session.GetString("LogUser");
            if (string.IsNullOrEmpty(Logsession))
            {
                return false;
            }
            return true;
        }
        public IActionResult IssueBook()
        {
            if (!isValid())
            {
                return RedirectToAction("Login", "Auth"); ;
            }
            DateTime today= DateTime.Today;
            DateTime nextmonth = today.AddMonths(1);

            ViewBag.Today = today.ToString("yyyy-MM-dd");
            ViewBag.NextMonth=nextmonth.ToString("yyyy-MM-dd");

            return View();
        }
        public IActionResult ReturnBook()
        {
            if (!isValid())
            {
                return RedirectToAction("Login", "Auth"); ;
            }
            DateTime today = DateTime.Today;
            ViewBag.Today = today.ToString("yyyy-MM-dd");
          
            return View();
        }
        public IActionResult AllIssuedBook()
        {
            if (!isValid())
            {
                return RedirectToAction("Login", "Auth"); ;
            }
            return View();
        }
        public IActionResult LateReturn()
        {
            if (!isValid())
            {
                return RedirectToAction("Login", "Auth"); ;
            }
            return View();
        }
        public IActionResult History()
        {
            if (!isValid())
            {
                return RedirectToAction("Login", "Auth"); ;
            }
            return View();
        }
    }
}
