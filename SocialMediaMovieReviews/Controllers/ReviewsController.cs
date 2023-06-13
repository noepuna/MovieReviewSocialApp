using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SocialMediaMovieReviews.Data;
using SocialMediaMovieReviews.Models;

namespace MovieReviewMediaApp.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reviews
        [Authorize]
        public async Task<IActionResult> MyReviews()
        {
            var userid = (await _userManager.GetUserAsync(User)).Id;
            var applicationDbContext = _context.Review.Where(r => r.UserId == userid).Include(r => r.Movie);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Reviews
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Review.Include(r => r.Movie).Include(r => r.User);
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
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }


        // GET: Reviews/Create
        [Authorize]
        public IActionResult Create(int? movieid)
        {
            ViewData["MovieId"] = new SelectList(_context.Movie, "Id", "Title");
            ViewData["Id"] = movieid;
            var mov = _context.Movie.Where(m => m.Id == movieid);
            ViewData["Movie"] = mov;
            return View();
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> CreateReview([Bind("Id,Text,Rating,MovieId,UserId")] Review review)
        {
            //if (ModelState.IsValid)
            //{
            var userid = (await _userManager.GetUserAsync(User)).Id;
            review.User = (await _userManager.GetUserAsync(User));
            review.UserId = userid;

            _context.Add(review);
            await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
            return Redirect(Request.Headers["Referer"].ToString());
            //}
            ViewData["MovieId"] = new SelectList(_context.Movie, "Id", "Id", review.MovieId);
            return View(review);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddReview(string Text, float Rating, int MovieId)
        {
            var userid = (await _userManager.GetUserAsync(User)).Id;
            Review review = new Review();
            review.User = (await _userManager.GetUserAsync(User));
            review.UserId = userid;
            review.Text = Text;
            review.Rating = Rating;
            review.MovieId = MovieId;

            _context.Add(review);
            await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
            TempData["SuccessMessage"] = "Your review has been posted.";
            //return Redirect(Request.Headers["Referer"].ToString());

            // Retrieve the URL of the movie details page based on the MovieId
            var movieDetailsUrl = Url.Action("Details", "Movies", new { id = MovieId });
            return Redirect(movieDetailsUrl);
        }


        [Authorize]
        // GET: Reviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var userid = (await _userManager.GetUserAsync(User)).Id;
            var review = await _context.Review.FindAsync(id);

            if (userid == review.UserId)
            {
                if (id == null || _context.Review == null)
                {
                    return NotFound();
                }

                review = await _context.Review.FindAsync(id);
                if (review == null)
                {
                    return NotFound();
                }
                ViewData["MovieId"] = new SelectList(_context.Movie, "Id", "Id", review.MovieId);
                return View(review);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
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

            //if (ModelState.IsValid)
            //{
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
                //}


            }
            ViewData["MovieId"] = new SelectList(_context.Movie, "Id", "Id", review.MovieId);
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        // GET: Reviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var userid = (await _userManager.GetUserAsync(User)).Id;
            var review = await _context.Review.FindAsync(id);

            if (userid == review.UserId)
            {

                if (id == null || _context.Review == null)
                {
                    return NotFound();
                }

                review = await _context.Review
                    .Include(r => r.Movie)
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (review == null)
                {
                    return NotFound();
                }

                return View(review);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
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
            var review = await _context.Review.Include(r => r.Likes).FirstOrDefaultAsync(r => r.Id == id);
            if (review != null)
            {
                foreach (var like in  review.Likes.ToList()) {
                    _context.ReviewLikes.Remove(like);
                }
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
