using LibrarySystemAPI.DataAccessLayer;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using LibraryMVC.Models;
using System;
using System.Text.Json.Nodes;

namespace LibraryMVC.Controllers
{
    public class AuthController : Controller
    {
        public LibrarySystemContext _context;

        public AuthController(LibrarySystemContext context)
        {
            _context = context;
        }

        string status = "Failure";
        string message;
        
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult LoginValid([FromBody] UserLoginViewModel model)
        {
            try
            {
                var name = model.LogUserName;
                var pass = model.LogPassword;

                var user = _context.User_Data.FirstOrDefault(u => u.UserEmail == name);
                if (user == null || user.UserPass != pass )
                {
                    throw new Exception("Invalid  User name or Password");
                }

                var validUser = _context.User_Data.FirstOrDefault(s => s.UserEmail == name && s.UserPass == pass);
                if (validUser == null)
                {
                    throw new Exception("Invalid User name or Password");
                }

                var librarian = _context.User_Data.FirstOrDefault(s => s.UserEmail == name && s.UserPass == pass);
                if (librarian == null)
                {
                    throw new Exception("User is not a librarian");
                }

                var username = librarian.UserName;
                string jsonModel = JsonSerializer.Serialize(model);
                HttpContext.Session.SetString("LogUser", jsonModel);

                return Ok(new { status = "Success", url = "/BookView/Index", message = "Login Successful.", data=username});
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Failure", message = ex.Message, url = "/Auth/Login" });
            }
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("LogUser");
            return RedirectToAction("Login","Auth");
        }

    }
}
