using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMediaMovieReviews.Data;
using SocialMediaMovieReviews.Models;

namespace SocialMediaMovieReviews.Controllers
{
    public class AccountsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> User(string username)
        {
            if (username == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = _context.Users.Include(u => u.Reviews).ThenInclude(r => r.Likes).Include(u => u.Reviews).ThenInclude(r => r.Movie)
                .Include(u => u.Following)
                .Include(u => u.Followers)
                .FirstOrDefault(x => x.UserName == username);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
    }
}
