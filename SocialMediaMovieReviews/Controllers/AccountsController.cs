using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IAntiforgery _antiforgery;

        public AccountsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IAntiforgery antiforgery)
        {
            _context = context;
            _userManager = userManager;
            _antiforgery = antiforgery;
        }
        public async Task<IActionResult> User(string username)
        {
            if (username == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = _context.Users.Include(u => u.Reviews).ThenInclude(r => r.Likes).Include(u => u.Reviews).ThenInclude(r => r.Movie).Include(u => u.Likes)
                .Include(u => u.Following)
                .Include(u => u.Followers)
                .FirstOrDefault(x => x.UserName == username);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        public JsonResult GetAntiforgeryToken()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            var token = tokens.RequestToken;
            return Json(new { token });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<JsonResult> Follow(bool follow, string userid, string user_to_follow_id)
        {
            ApplicationUser user = _context.Users.Where(u => u.Id == userid).Include(u => u.Followers).Include(u => u.Following).FirstOrDefault();
            ApplicationUser user_to_follow = _context.Users.Where(u => u.Id == user_to_follow_id).Include(u => u.Followers).Include(u => u.Following).FirstOrDefault();


            if (follow == true)
            {
                user.Following.Add(user_to_follow);
                user_to_follow.Followers.Add(user);
                // Save the changes to the database
                await _context.SaveChangesAsync();
                return Json(new { success = true, msg = "Now following: " + user_to_follow.UserName });
            }
            else
            {
                user.Following.Remove(user_to_follow);
                user_to_follow.Followers.Remove(user);
                // Save the changes to the database
                await _context.SaveChangesAsync();
                return Json(new { success = true, msg = "Unfollowed: " + user_to_follow.UserName });
            }

        }
    }
}
