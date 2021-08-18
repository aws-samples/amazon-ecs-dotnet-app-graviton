using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Data;
using MvcMovie.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MvcMovie.Attribute;

namespace MvcMovie.Controllers
{
    [Authorize]
    [EnforceChangePassword]
    public class HomeController : Controller
    {
        private readonly MvcMovieContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<HomeController> _logger;

        public HomeController(MvcMovieContext context, UserManager<AppUser> userMgr, ILogger<HomeController> logger)
        {
            _context = context;
            _userManager = userMgr;
            _logger = logger;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string movieGenre, string searchString, int? winYear)
        {
            // Use LINQ to get list of genres.
            var genreQuery = from m in _context.Movie
                                            orderby m.Genre
                                            select m.Genre;

            var yearQuery = from m in _context.Movie
                                            orderby m.Year
                                            select m.Year;

            var movies = from m in _context.Movie
                         select m;

            if (!string.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(s => s.Title.ToLower().Contains(searchString.ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(movieGenre))
            {
                movies = movies.Where(s => s.Genre.ToLower().Equals(movieGenre.ToLower().Trim()));
            }

            if(winYear.HasValue && winYear.Value > 0)
            {
                movies = movies.Where(s => s.Year == winYear.Value);
            }

            var movieGenreVM = new MovieGenreViewModel
            {
                MovieGenre = movieGenre,
                SearchString = searchString,
                Genres = new SelectList(await genreQuery.Distinct().ToListAsync()),
                Years = new SelectList(await yearQuery.Distinct().ToListAsync()),
                Movies = await movies.ToListAsync()
            };

            return View(movieGenreVM);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Year,Genre,Director")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Movie Id: {movie.Id}, Title: {movie.Title} is created by {_userManager.GetUserName(User)}.");
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Year,Genre,Director")] Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Movie Id: {movie.Id}, Title: {movie.Title} is updated by {_userManager.GetUserName(User)}.");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Movie.Any(e => e.Id == movie.Id))
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
            return View(movie);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            _context.Movie.Remove(movie);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Movie Id: {movie.Id}, Title: {movie.Title} is deleted by {_userManager.GetUserName(User)}.");
            return RedirectToAction(nameof(Index));
        }
    }
}
