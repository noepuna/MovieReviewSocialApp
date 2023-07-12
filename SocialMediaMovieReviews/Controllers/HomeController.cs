using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMediaMovieReviews.Data;
using SocialMediaMovieReviews.Models;
using System.Diagnostics;

namespace SocialMediaMovieReviews.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> User(string userid)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == userid);
            
            return _context.Users != null ?
                              View("~/Views/Home/UserPage.cshtml", user) :
                              Problem("Entity set 'ApplicationDbContext.Movie'  is null.");
        }

        public async Task<IActionResult> Search(string searchtext, string searchtype)
        {
            if (searchtype == "search-movies")
            {
                return _context.Movie != null ?
                              View(await _context.Movie.Where(m => m.Title.Contains(searchtext)).Include(m => m.Reviews).ToListAsync()) :
                              Problem("Entity set 'ApplicationDbContext.Movie'  is null.");
            }
            else
            {
                return _context.Users != null ?
                              View("~/Views/Home/Search-Accounts.cshtml", await _context.Users.Where(m => m.UserName.Contains(searchtext)).Include(m => m.Reviews).ToListAsync()) :
                              Problem("Entity set 'ApplicationDbContext.Users'  is null.");
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}