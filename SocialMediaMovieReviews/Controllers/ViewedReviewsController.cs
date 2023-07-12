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
    public class ViewedReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAntiforgery _antiforgery;

        public ViewedReviewsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IAntiforgery antiforgery)
        {
            _context = context;
            _userManager = userManager;
            _antiforgery = antiforgery;
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
        public async Task<JsonResult> AddView(int reviewid)
        {
            var userid = (await _userManager.GetUserAsync(User)).Id;
            bool doesExist = _context.ViewedReview.Any(vr => vr.UserId == userid && vr.ReviewId == reviewid);

            if (!(doesExist))
            {
                ViewedReview viewedreview = new ViewedReview();
                viewedreview.ReviewId = reviewid;
                viewedreview.UserId = userid;
                _context.Add(viewedreview);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                //return Redirect(Request.Headers["Referer"].ToString());

                // Retrieve the URL of the movie details page based on the MovieId
                /*var movieDetailsUrl = Url.Action("Details", "Movies", new { id = review.MovieId });
                return Redirect(movieDetailsUrl);*/
                return Json(new
                {
                    viewedreview_id = viewedreview.Id,
                    viewedreview_reviewid = viewedreview.ReviewId,
                    viewedreview_userid = viewedreview.UserId,
                    result = "viewed already"
                });
            }else
            {
                return Json(new
                {
                    result = "now viewed"
                });
            }
        }

        // GET: ViewedReviews
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ViewedReview.Include(v => v.Review).Include(v => v.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ViewedReviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ViewedReview == null)
            {
                return NotFound();
            }

            var viewedReview = await _context.ViewedReview
                .Include(v => v.Review)
                .Include(v => v.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (viewedReview == null)
            {
                return NotFound();
            }

            return View(viewedReview);
        }

        // GET: ViewedReviews/Create
        public IActionResult Create()
        {
            ViewData["ReviewId"] = new SelectList(_context.Review, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: ViewedReviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ReviewId,UserId")] ViewedReview viewedReview)
        {
            if (ModelState.IsValid)
            {
                _context.Add(viewedReview);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ReviewId"] = new SelectList(_context.Review, "Id", "Id", viewedReview.ReviewId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", viewedReview.UserId);
            return View(viewedReview);
        }

        // GET: ViewedReviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ViewedReview == null)
            {
                return NotFound();
            }

            var viewedReview = await _context.ViewedReview.FindAsync(id);
            if (viewedReview == null)
            {
                return NotFound();
            }
            ViewData["ReviewId"] = new SelectList(_context.Review, "Id", "Id", viewedReview.ReviewId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", viewedReview.UserId);
            return View(viewedReview);
        }

        // POST: ViewedReviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ReviewId,UserId")] ViewedReview viewedReview)
        {
            if (id != viewedReview.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewedReview);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ViewedReviewExists(viewedReview.Id))
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
            ViewData["ReviewId"] = new SelectList(_context.Review, "Id", "Id", viewedReview.ReviewId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", viewedReview.UserId);
            return View(viewedReview);
        }

        // GET: ViewedReviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ViewedReview == null)
            {
                return NotFound();
            }

            var viewedReview = await _context.ViewedReview
                .Include(v => v.Review)
                .Include(v => v.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (viewedReview == null)
            {
                return NotFound();
            }

            return View(viewedReview);
        }

        // POST: ViewedReviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ViewedReview == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ViewedReview'  is null.");
            }
            var viewedReview = await _context.ViewedReview.FindAsync(id);
            if (viewedReview != null)
            {
                _context.ViewedReview.Remove(viewedReview);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ViewedReviewExists(int id)
        {
          return (_context.ViewedReview?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
