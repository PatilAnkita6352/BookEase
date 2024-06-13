using Microsoft.AspNetCore.Mvc;
using LibrarySystemAPI.DataAccessLayer;

namespace LibraryMVC.Controllers
{
    public class UserView_DataController : Controller
    {
        public LibrarySystemContext _context;

        public UserView_DataController(LibrarySystemContext context)
        {
            _context = context;
        }
        public bool isValid()
        {
            var Logsession = HttpContext.Session.GetString("LogUser");
            if (string.IsNullOrEmpty(Logsession))
            {
                return false;
            }
            return true;
        }
        [HttpGet]
        public IActionResult searchUser(string username)
        {
            try
            {
                var users = _context.User_Data.Where(s => s.UserName.Contains(username)).ToList();
                if (users.Count == 0)
                {
                    throw new Exception("No users found with this name.");
                }
                return Ok(new { status = "Success", data = users });

            }
            catch (Exception ex)
            {
                return Ok(new { status = "Failure", message = ex.Message });
            }
        }
        public IActionResult AddUser()
        {
            if (!isValid())
            {
                return RedirectToAction("Login", "Auth"); ;
            }
            return View();
        }
        public IActionResult AllUser()
        {
            if (!isValid())
            {
                return RedirectToAction("Login", "Auth"); ;
            }
            return View();
        }
        public IActionResult UserHistory()
        {
            if (!isValid())
            {
                return RedirectToAction("Login", "Auth"); ;
            }
            return View();
        }
    }
}
