using LibrarySystemAPI.DataAccessLayer;
using LibrarySystemAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlTypes;
using System.Net;

namespace LibrarySystemAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class Book_StockController : ControllerBase
    {
        public LibrarySystemContext _context;
        string status = "Failure";
        string message;
        public Book_StockController(LibrarySystemContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetStockCount()
        {
            try
            {
                var stock = _context.Book_Stock.ToList();
                if (stock.Count == 0)
                {
                    throw new Exception("No Stock Available");
                }
                return Ok(new { status = "Success", data = stock });
            }
            catch (Exception ex)
            {
                return Ok(new { status = status, message = ex.Message });
            }
        }
        [HttpGet("{id}")]
        public IActionResult GetStock(string id)
        {
            try
            {
                var stock = (from bs in _context.Book_Stock
                             where bs.BookId == id && bs.StockStatus == "Available"
                             select new
                             {
                                 bookStockId = bs.BookStockId,
                                 bookstatus = bs.StockStatus
                             }).ToList();


                if (stock.Count == 0)
                {
                    throw new Exception("No Stock Available");
                }
                return Ok(new { status = "Success", data = stock });
            }
            catch (Exception ex)
            {
                return Ok(new { status = status, message = ex.Message });
            }
        }
        [HttpGet]
        public IActionResult Getremove()
        {
            try
            {
                var stock = _context.Book_Data.Where(s => s.BookStatus == "Removed").ToList();
                if (stock.Count() == 0)
                {
                    throw new Exception("No book Available");
                }
                return Ok(new { status = "Success", data = stock });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Failure", message = ex.Message });
            }
        }
        [HttpPut("{id}")]
        public IActionResult ActivateBook(string id)
        {
            try
            {
                var book = _context.Book_Data.Find(id);
                if (book == null)
                {
                    throw new Exception("Book is not available");
                }
                book.BookStatus = "Not Available";
                _context.SaveChanges();
                return Ok(new { status = "Success", message = "Book Activated" });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Failure", message = ex.Message });
            }


        }
        [HttpGet]
        public IActionResult GetServerTime(string timeZone = "Asia/Kolkata")
        {
            var serverTime = (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZone)));
            var formattedDate = serverTime.ToString("yyyy-MM-dd");
            return Ok(new { status="Success" ,data = formattedDate });
        }
        [HttpGet]
        public IActionResult AutoCompleteUserBook(string user, string book, string status)
        {
            try
            {
                Console.WriteLine(user + book);
                var query = from issue in _context.Issued_Books
                            join bookstock in _context.Book_Stock on issue.BookStockId equals bookstock.BookStockId
                            join bok in _context.Book_Data on bookstock.BookId equals bok.BookId
                            join usr in _context.User_Data on issue.UserId equals usr.UserId
                            where ((user != "None" && book != "None" && usr.UserId == user && bok.BookId == book) ||
                                  (user != "None" && book == "None" && usr.UserId == user) ||
                                  (user == "None" && book != "None" && bok.BookId == book) )&&
                                  issue.BookIStatus == status
                            select new
                            {
                                UserId = usr.UserId,
                                bookId = bookstock.BookStockId,
                                bookTitle = bok.BookTitle,
                                bookImg = bok.BookImg,
                                UserName = usr.UserName,
                                IssueDate = issue.IssueDate,
                                ReturnDate = issue.ReturnDate,
                                BookIStatus = issue.BookIStatus,
                            };

                var books = query.ToList();
                return Ok(new { status = "Success", data = books });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Failure", message = ex.Message });
            }
        }

    }    }
