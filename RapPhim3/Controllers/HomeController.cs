using Microsoft.AspNetCore.Mvc;
using RapPhim3.Services;

namespace RapPhim3.Controllers
{
    public class HomeController : Controller
    {
        private readonly MovieService _movieService;

        public HomeController(MovieService movieService)
        {
            _movieService = movieService;
        }

        public IActionResult Index()
        {
            var movies = _movieService.GetMovies();
            var topMovies = movies.OrderByDescending(m => m.Id).Take(3).ToList();
            ViewBag.TopMovies = topMovies;
            return View(movies);
        }

        public IActionResult MovieDetail(int id)
        {
            var movie = _movieService.
                GetMovieById(id); // Lấy phim theo ID
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }
    }
}
