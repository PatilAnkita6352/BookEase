 using Microsoft.AspNetCore.Mvc;
using LibrarySystemAPI.DataAccessLayer;


namespace LibraryMVC.Controllers
{
    public class BookViewController : Controller
    {
        public LibrarySystemContext _context;

        public BookViewController(LibrarySystemContext context)
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
        public IActionResult Index()
        {
            if (!isValid())
            {
                return RedirectToAction("Login", "Auth"); ;
            }
            return View();
        }
        public IActionResult AddBook()
        {
            if (!isValid())
            {
                return RedirectToAction("Login", "Auth"); ;
            }
            return View();
        }
        public async Task<IActionResult> Upload()
        {
            try
            {
                var file = Request.Form.Files[0]; // Get the uploaded file

                // Validate file type, size, and other necessary checks

                var uploadDirectory = Path.Combine("wwwroot", "Upload");
                var filePath = Path.Combine(uploadDirectory, file.FileName); // Set the file path

                // Ensure the directory exists before saving the file
                Directory.CreateDirectory(uploadDirectory);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream); // Save the file to disk
                }

                var imagePath = "/Upload/" + file.FileName; // Image path to save in the database

                return Ok(new { status = "Success", url = imagePath }); // Return the image path
            }
            catch (Exception ex)
            {
                // Log the error for debugging purposes
                Console.WriteLine("Error uploading file: " + ex.Message);

                return StatusCode(500, new { status = "Failure", message = "An error occurred while uploading the file." }); // Return a generic error message
            }

        }
        public IActionResult AddStock()
        {
            if (!isValid())
            {
                return RedirectToAction("Login", "Auth"); ;
            }
            return View();
        }
        [HttpGet]
        public IActionResult GetBookName(string id)
        {
            try
            {
                var book = _context.Book_Data.Where(s => s.BookId.Contains(id)).Distinct().ToList();
                if (book.Count <= 0) throw new Exception("No Book Available");
                return Ok(new { status = "Success", data = book });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Failure", message = ex.Message });
            }
        }

        public IActionResult EditBook()
        {
            if (!isValid())
            {
                return RedirectToAction("Login", "Auth"); ;
            }
            return View();
        }
        public IActionResult DeactivateBook()
        {
            if (!isValid())
            {
                return RedirectToAction("Login", "Auth"); ;
            }
            return View();
        }
        public IActionResult DeleteBook()
        {
            if (!isValid())
            {
                return RedirectToAction("Login", "Auth"); ;
            }
            return View();
        }
        public IActionResult RemovedBooks()
        {
            if (!isValid())
            {
                return RedirectToAction("Login", "Auth"); ;
            }
            return View();
        }
        public IActionResult BookInfo()
        {
            if (!isValid())
            {
                return RedirectToAction("Login", "Auth"); ;
            }
            return View();
        }
    }
}
