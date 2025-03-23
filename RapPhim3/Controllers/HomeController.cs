using Microsoft.AspNetCore.Mvc;
using RapPhim3.Models;
using RapPhim3.Services;
using RapPhim3.ViewModel;

namespace RapPhim3.Controllers
{
    public class HomeController : Controller
    {
        private readonly MovieService _movieService;
        private readonly ShowTimeService _showTimeService;
        public HomeController(MovieService movieService, ShowTimeService showTimeService)
        {
            _movieService = movieService;
            _showTimeService = showTimeService;
        }

        public IActionResult Index(DateOnly? selectedDate)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            DateOnly targetDate = selectedDate ?? today;

            // Lấy danh sách phim có suất chiếu trong ngày được chọn
            var movies = _showTimeService.GetMoviesByDate(targetDate);
            ViewBag.SelectedDate = targetDate;
            ViewBag.Next7Days = Enumerable.Range(0, 7).Select(i => today.AddDays(i)).ToList();
            ViewBag.LastDate = ((List<DateOnly>)ViewBag.Next7Days).Last();


            ViewBag.TopMovies = _movieService.GetTopMoviesByNewest() ?? new List<Movie>();

            return View(movies);
        }



        public IActionResult MovieType()
        {
            var viewModel = _movieService.GetMovieTypeViewModel();
            return View(viewModel);
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
