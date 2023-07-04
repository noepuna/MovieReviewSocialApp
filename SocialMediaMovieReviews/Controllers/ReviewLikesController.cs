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
    public class ReviewLikesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAntiforgery _antiforgery;

        public ReviewLikesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IAntiforgery antiforgery)
        {
            _context = context;
            _userManager = userManager;
            _antiforgery = antiforgery;
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

        public JsonResult GetAntiforgeryToken()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            var token = tokens.RequestToken;
            return Json(new { token });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<JsonResult> AddLike(bool liked, int reviewid, int movieid /*[Bind("Id,UserId,ReviewId,MovieId,isLiked")] ReviewLike reviewLike*/)
        {
            var userid = (await _userManager.GetUserAsync(User)).Id;
            ReviewLike reviewLike = new ReviewLike();
            reviewLike.isLiked = liked;
            reviewLike.UserId = userid;
            reviewLike.ReviewId = reviewid;
            reviewLike.MovieId = movieid;
            
            _context.Add(reviewLike);
            await _context.SaveChangesAsync();

            var movieDetailsUrl = Url.Action("Details", "Movies", new { id = reviewLike.MovieId });
            var likecount = _context.ReviewLikes.Where(l => l.isLiked == true && l.ReviewId == reviewid).Count();
            var dislikecount = _context.ReviewLikes.Where(l => l.isLiked == false && l.ReviewId == reviewid).Count();

            /*return Redirect(movieDetailsUrl);*/
            if (liked == true)
            {
                return Json(new { success = true, msg = "Added Like", id = reviewLike.Id, likecount = likecount, dislikecount = dislikecount});
            }
            else
            {
                return Json(new { success = true, msg = "Added Dislike", id = reviewLike.Id, likecount = likecount, dislikecount = dislikecount });
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> RemoveLike(int likeid, bool? isliked)
        {
            var like = _context.ReviewLikes.First(r => r.Id == likeid);
            var reviewid = like.ReviewId;
            if (isliked.HasValue)
            {
                like.isLiked = isliked.Value;
            }
            else
            {
                _context.ReviewLikes.Remove(like);
            }
            await _context.SaveChangesAsync();
            var likecount = _context.ReviewLikes.Where(l => l.isLiked == true && l.ReviewId == reviewid).Count();
            var dislikecount = _context.ReviewLikes.Where(l => l.isLiked == false && l.ReviewId == reviewid).Count();

            /*return Redirect(movieDetailsUrl);*/
            if (isliked == true)
            {
                return Json(new { success = true, msg = "Switched to Like", likecount = likecount, dislikecount = dislikecount });
            }
            else if (isliked == false)
            {
                return Json(new { success = true, msg = "Switched to Dislike", likecount = likecount, dislikecount = dislikecount });
            }else
            {
                return Json(new { success = true, msg = "Removed", likecount = likecount, dislikecount = dislikecount });
            }
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
