using Microsoft.AspNetCore.Mvc;
using RapPhim3.Models;
using RapPhim3.Services;
using RapPhim3.ViewModel;

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


        public IActionResult MovieType()
        {
            // Lấy danh sách thể loại phim
            var genres = _movieService.GetGenres();

            // Lấy danh sách quốc gia
            var countries = _movieService.GetCountries();

            // Lấy toàn bộ danh sách phim
            var allMovies = _movieService.GetMovies();

            // Gửi dữ liệu sang View
            var model = new MovieTypeViewModel
            {
                Genres = genres,
                Countries = countries,
                AllMovies = allMovies
            };

            return View("MovieType",model);
        }


        public IActionResult Actors()
        {
            return View();
        }

        public IActionResult Directors()
        {
            return View();
        }

        public IActionResult Reviews()
        {
            return View();
        }
    }
}
