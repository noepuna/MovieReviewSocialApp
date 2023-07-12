using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SocialMediaMovieReviews.Data;
using SocialMediaMovieReviews.Models;

namespace SocialMediaMovieReviews.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAntiforgery _antiforgery;

        public ReviewsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IAntiforgery antiforgery)
        {
            _context = context;
            _userManager = userManager;
            _antiforgery = antiforgery;
        }

        // GET: Reviews
        public async Task<IActionResult> Index()
        {
            var userid = (await _userManager.GetUserAsync(User)).Id;
            var unseenReviews = _context.Review.Where(r1 => !r1.Views.Any(v => v.UserId == userid)).Include(r => r.Movie).Include(r => r.User).Include(r => r.Likes);
            var applicationDbContext = _context.Review.Include(r => r.Movie).Include(r => r.User).Include(r => r.Likes);
            return View(await unseenReviews.ToListAsync());
        }

        // GET: Reviews
        [Authorize]
        public async Task<IActionResult> MyReviews()
        {
            var userid = (await _userManager.GetUserAsync(User)).Id;
            var applicationDbContext = _context.Review.Where(r => r.UserId == userid).Include(r => r.Movie);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Reviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Review == null)
            {
                return NotFound();
            }

            var review = await _context.Review
                .Include(r => r.Movie)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
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
        public async Task<JsonResult> AddReview(string reviewtext, float reviewrating, int reviewmovieid)
        {
            var userid = (await _userManager.GetUserAsync(User)).Id;
            var movie = _context.Movie.Where(m => m.Id == reviewmovieid).FirstOrDefault();
            Review review = new Review();
            review.UserId = userid;
            review.Text = reviewtext;
            review.Rating = reviewrating;
            review.MovieId = reviewmovieid;
            review.Movie = movie;

            review.UserId = userid;
            _context.Add(review);
            await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
            //return Redirect(Request.Headers["Referer"].ToString());

            // Retrieve the URL of the movie details page based on the MovieId
            /*var movieDetailsUrl = Url.Action("Details", "Movies", new { id = review.MovieId });
            return Redirect(movieDetailsUrl);*/
            return Json(new { reviewid = review.Id, review_text = review.Text, 
                review_rating = review.Rating, review_username = review.User.UserName,
                review_movietitle = review.Movie.Title
            });
        }

        // GET: Reviews/Create
        public IActionResult Create()
        {
            ViewData["MovieId"] = new SelectList(_context.Movie, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Text,Rating,MovieId,UserId")] Review review)
        {
            if (ModelState.IsValid)
            {
                _context.Add(review);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MovieId"] = new SelectList(_context.Movie, "Id", "Id", review.MovieId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", review.UserId);
            return View(review);
        }

        // GET: Reviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Review == null)
            {
                return NotFound();
            }

            var review = await _context.Review.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            ViewData["MovieId"] = new SelectList(_context.Movie, "Id", "Id", review.MovieId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", review.UserId);
            return View(review);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Text,Rating,MovieId,UserId")] Review review)
        {
            if (id != review.Id)
            {
                return NotFound();
            }

            
                try
                {
                    _context.Update(review);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            return Redirect(Request.Headers["Referer"].ToString());

            ViewData["MovieId"] = new SelectList(_context.Movie, "Id", "Id", review.MovieId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", review.UserId);
            /*return View(review);*/
            return Redirect(Request.Headers["Referer"].ToString());
        }

        // GET: Reviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Review == null)
            {
                return NotFound();
            }

            var review = await _context.Review
                .Include(r => r.Movie)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Review == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Review'  is null.");
            }
            var review = await _context.Review.FindAsync(id);
            if (review != null)
            {
                var likes_to_remove = _context.ReviewLikes.Where(rl => rl.ReviewId == review.Id);
                _context.ReviewLikes.RemoveRange(likes_to_remove);
                var views_to_remove = _context.ViewedReview.Where(vr => vr.ReviewId == review.Id);
                _context.ViewedReview.RemoveRange(views_to_remove);
                _context.Review.Remove(review);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewExists(int id)
        {
          return (_context.Review?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
