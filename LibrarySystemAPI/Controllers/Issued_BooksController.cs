using LibrarySystemAPI.DataAccessLayer;
using LibrarySystemAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Net;
using System.Runtime.Intrinsics.X86;
using static System.Reflection.Metadata.BlobBuilder;

namespace LibrarySystemAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class Issued_BooksController : ControllerBase
    {
        public readonly LibrarySystemContext _context;
        string status_r = "Failure";
        string message;
        public Issued_BooksController(LibrarySystemContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult GetAutoComplete() {
            try
            {
                var book = (from m in _context.Book_Data
                            select new
                            {
                                bookid = m.BookId,
                                booktitle = m.BookTitle
                            }).ToList();
                if (book.Count() == null)
                {
                    throw new Exception($"No Book available");
                }
                return Ok(new { status = "Success", data = book });
            }
            catch (Exception ex)
            {
               return Ok(new { status = "Failure", message = ex.Message });
            }
           
        }

       /* try
            {
                var books = (from i in _context.Issued_Books
                             join bs in _context.User_Data on i.UserId equals bs.UserId
                             where i.BookStockId.Contains(bookid) || bs.UserId.Equals(id)
                             select new
                             {
                                 UserId = bs.UserId,
                                 UserName = bs.UserName,
                                 BookStockId = i.BookStockId,
                                 IssueDate = i.IssueDate,
                                 ReturnDate = i.ReturnDate,
                                 BookIStatus = i.BookIStatus,
                             }).ToList();

                if (books.Count == 0)
                {
                    throw new Exception($"No History of Book With {id} available");
    }
                return Ok(new { status = "Success", data = books
});
            }
            catch (Exception ex)
            {
                return Ok(new { status = status_r, message = ex.Message });
            }*/
        [HttpGet("{status}")]
        public IActionResult Getwithstatus(string status) {
            try
            {
                var bo=_context.Issued_Books.ToList();
                var currentDate = DateOnly.FromDateTime(DateTime.Now);
                foreach (var b in bo)
                {
                    var returnDate = b.ReturnDate.CompareTo(currentDate);
                    if (returnDate < 0 && b.BookIStatus != "Returned")
                    {
                        b.BookIStatus = "Late";
                        _context.SaveChanges();
                    }

                }
                var books_issue = ( from issue in _context.Issued_Books join
                                    bookstock in _context.Book_Stock  on issue.BookStockId equals bookstock.BookStockId
                                    join book in _context.Book_Data on bookstock.BookId equals book.BookId 
                                    join user in _context.User_Data on issue.UserId equals user.UserId
                                    where issue.BookIStatus.Contains(status)
                                    select new
                                    {
                                        userId=issue.UserId,
                                        userName=user.UserName,
                                        bookId=bookstock.BookStockId,
                                        bookTitle=book.BookTitle,
                                        issueDate= issue.IssueDate,
                                        returnDate= issue.ReturnDate,
                                        BookStatus=issue.BookIStatus
                                    }).ToList();
                
                
                if (books_issue.Count() == 0)
                {
                    throw new Exception($"No Book With {status} available");
                   
                }
                return Ok(new {status= "Success" , data= books_issue});
            }
            catch (Exception ex)
            {
                return Ok(new { status = status_r, message = ex.Message });
            }
        }

        [HttpGet]

        public IActionResult GetHistory(string id) 
        {
            try
            {
                var books = (from i in _context.Issued_Books
                             join bs in _context.User_Data on i.UserId equals bs.UserId
                             where i.BookStockId.Contains(id)
                             select new
                             {
                                 UserId = bs.UserId,
                                 UserName = bs.UserName,
                                 BookStockId = i.BookStockId,
                                 IssueDate = i.IssueDate,
                                 ReturnDate = i.ReturnDate,
                                 BookIStatus = i.BookIStatus,
                             }).ToList();

                if (books.Count == 0)
                {
                    throw new Exception($"No History of Book With {id} available");
                }
                return Ok(new { status = "Success", data = books });
            }
            catch (Exception ex)
            {
                return Ok(new { status = status_r, message = ex.Message });
            }

        }
        [HttpGet]
        public IActionResult GetHistoryByID( string userid)
        {
            try
            {
                var books = (from i in _context.Issued_Books
                             join bs in _context.User_Data on i.UserId equals bs.UserId
                             join b in _context.Book_Stock on i.BookStockId equals b.BookStockId
                             join u in _context.Book_Data on b.BookId equals u.BookId
                             where bs.UserId.Equals(userid)
                             select new
                             {
                                 UserId = bs.UserId,
                                 bookId= u.BookId,
                                 bookTitle = u.BookTitle,
                                 bookImg = u.BookImg,
                                 UserName = bs.UserName,
                                 IssueDate = i.IssueDate,
                                 ReturnDate = i.ReturnDate,
                                 BookIStatus = i.BookIStatus,
                             }).ToList();

                if (books.Count == 0)
                {
                    throw new Exception($"No History of Book With {userid} available");
                }
                return Ok(new { status = "Success", data = books });
            }
            catch (Exception ex)
            {
                return Ok(new { status = status_r, message = ex.Message });
            }


        }

        [HttpPost]
        public IActionResult PostIssueBook([FromBody] Issued_BooksAPI model)
        {
            try
            {
                var I= _context.Issued_Books.ToList();
                var len = I.Count;
                var dateN = DateOnly.FromDateTime(DateTime.Now);
                var rdate = dateN.AddMonths(1);
                var check = _context.Issued_Books.Where(s => s.BookStockId == model.BookStockId && s.BookIStatus != "Returned");
                var checkuser = _context.Issued_Books.Where(p => p.UserId == model.UserId && p.BookIStatus == "Issued").ToList();
                var bookid = model.BookStockId.Split("St")[0];
                Console.WriteLine(bookid);
                var samebook = _context.Issued_Books.Where(a => a.UserId == model.UserId && a.BookStockId.Contains(bookid) && a.BookIStatus != "Returned").Count();
                var stock = _context.Book_Stock.Find(model.BookStockId);
                if(samebook > 0)
                {
                    throw new Exception("User can not issue same book twice");
                }
                if(checkuser.Count() >= 3)
                {
                    throw new Exception("User can Issue upto 3 Books Only");
                }
                if (check.Count() > 0)
                {
                    throw new Exception("Book is Already Issued");
                }
                var u = _context.User_Data.Where(s => s.UserId == model.UserId).ToList();
                if (u.Count <= 0 )
                {
                    throw new Exception("User Don't Exist");
                }
                if(model.IssueDate != "")
                {
                    dateN = DateOnly.Parse(model.IssueDate);
                }
                if(model.ReturnDate != "")
                {
                    rdate = DateOnly.Parse(model.ReturnDate); 
                }
                var bookItem = new Issued_Books()
                {
                    IssueId = len + 1,
                    BookStockId = model.BookStockId,
                    UserId = model.UserId,
                    IssueDate = dateN,
                    ReturnDate = rdate,
                    BookIStatus = "Issued"
                };
                if(stock==null)
                {
                    throw new Exception("Book Stock not available");
                }

                stock.StockStatus = "Not";
                _context.Issued_Books.Add(bookItem);
                _context.SaveChanges();
                status_r = "Success";
                    message = "Book Issued Successful :)";
            }
            catch (Exception ex)
            {
               message= ex.Message;
            }
            return Ok(new { status = status_r, message = message });
        }

        [HttpPut]
        public IActionResult Update()
        {
            try
            {
                var books = _context.Issued_Books.ToList(); // Load all issued books into memory
                var currentDate = DateOnly.FromDateTime(DateTime.Now);

                if (books.Count == 0)
                {
                    throw new Exception("No Issued Book with Delayed Date Found");
                }

                foreach (var book in books)
                {
                    if (book.BookIStatus == "Returned")
                    {
                        _context.Issued_Books.Remove(book);
                    }
                    else
                    {
                        var returnDate = book.ReturnDate.CompareTo(currentDate);


                        if (returnDate < 0)
                        {
                            book.BookIStatus = "Late";
                        }

                    }
                }

                _context.SaveChanges();
                status_r = "Success";
                message = "Late Status Updated";
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Ok(new { status = status_r, message = message });
        }

        [HttpPut("{id}/{user}")]
        public IActionResult ReturnBook(string id, string user)
        {
            string status_r = "Error";
            string message = "";

            try
            {
                var book = _context.Issued_Books.SingleOrDefault(b => b.BookStockId == id && b.UserId == user && (b.BookIStatus == "Issued" || b.BookIStatus == "Late"));
                if (book == null)
                {
                    throw new Exception($"Book with ID {id} not found for user {user}");
                }

                book.BookIStatus = "Returned";
                var stock = _context.Book_Stock.Find(id);
                stock.StockStatus = "Available";


                // Save changes to the database
                _context.SaveChanges();

                status_r = "Success";
                message = "Book Returned";
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return Ok(new { status = status_r, message = message });
        }

    }
}
