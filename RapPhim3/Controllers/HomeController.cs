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
            
            var genres = _movieService.GetGenres();
            var countries = _movieService.GetCountries();
             var allMovies = _movieService.GetMovies();

            var model = new MovieTypeViewModel
            {
                Genres = genres,
                Countries = countries,
                AllMovies = allMovies
            };

            return View("MovieType",model);
        }

        public IActionResult FilterMovies(int? genreId, int? countryId, int? year)
        {
            var movies = _movieService.GetMovies();

            if (genreId.HasValue)
            {
                movies = movies.Where(m => m.Genres.Any(g => g.Id == genreId.Value)).ToList();
                ViewBag.SelectedGenre = genreId;
            }

            if (countryId.HasValue)
            {
                movies = movies.Where(m => m.Country.Id == countryId.Value).ToList();
                ViewBag.SelectedCountry = countryId;
            }

            if (year.HasValue)
            {
                movies = movies.Where(m => m.ReleaseDate.HasValue && m.ReleaseDate.Value.Year == year.Value).ToList();
                ViewBag.SelectedYear = year;
            }

            var viewModel = new MovieTypeViewModel
            {
                Genres = _movieService.GetGenres(),
                Countries = _movieService.GetCountries(),
                AllMovies = movies
            };

            return View("MovieType", viewModel);
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
