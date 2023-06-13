using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SocialMediaMovieReviews.Data;
using SocialMediaMovieReviews.Models;

namespace SocialMediaMovieReviews.Controllers
{
    public class ReviewLikesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewLikesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ReviewLikes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ReviewLikes.Include(r => r.Movie).Include(r => r.Review).Include(r => r.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ReviewLikes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ReviewLikes == null)
            {
                return NotFound();
            }

            var reviewLike = await _context.ReviewLikes
                .Include(r => r.Movie)
                .Include(r => r.Review)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reviewLike == null)
            {
                return NotFound();
            }

            return View(reviewLike);
        }

        // GET: ReviewLikes/Create
        public IActionResult Create()
        {
            ViewData["MovieId"] = new SelectList(_context.Movie, "Id", "Id");
            ViewData["ReviewId"] = new SelectList(_context.Review, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: ReviewLikes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,ReviewId,MovieId,isLiked")] ReviewLike reviewLike)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reviewLike);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MovieId"] = new SelectList(_context.Movie, "Id", "Id", reviewLike.MovieId);
            ViewData["ReviewId"] = new SelectList(_context.Review, "Id", "Id", reviewLike.ReviewId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", reviewLike.UserId);
            return View(reviewLike);
        }

        // GET: ReviewLikes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ReviewLikes == null)
            {
                return NotFound();
            }

            var reviewLike = await _context.ReviewLikes.FindAsync(id);
            if (reviewLike == null)
            {
                return NotFound();
            }
            ViewData["MovieId"] = new SelectList(_context.Movie, "Id", "Id", reviewLike.MovieId);
            ViewData["ReviewId"] = new SelectList(_context.Review, "Id", "Id", reviewLike.ReviewId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", reviewLike.UserId);
            return View(reviewLike);
        }

        // POST: ReviewLikes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,ReviewId,MovieId,isLiked")] ReviewLike reviewLike)
        {
            if (id != reviewLike.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reviewLike);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewLikeExists(reviewLike.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MovieId"] = new SelectList(_context.Movie, "Id", "Id", reviewLike.MovieId);
            ViewData["ReviewId"] = new SelectList(_context.Review, "Id", "Id", reviewLike.ReviewId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", reviewLike.UserId);
            return View(reviewLike);
        }

        // GET: ReviewLikes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ReviewLikes == null)
            {
                return NotFound();
            }

            var reviewLike = await _context.ReviewLikes
                .Include(r => r.Movie)
                .Include(r => r.Review)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reviewLike == null)
            {
                return NotFound();
            }

            return View(reviewLike);
        }

        // POST: ReviewLikes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ReviewLikes == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ReviewLikes'  is null.");
            }
            var reviewLike = await _context.ReviewLikes.FindAsync(id);
            if (reviewLike != null)
            {
                _context.ReviewLikes.Remove(reviewLike);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewLikeExists(int id)
        {
          return (_context.ReviewLikes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
