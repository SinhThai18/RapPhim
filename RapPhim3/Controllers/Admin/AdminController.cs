using Microsoft.AspNetCore.Mvc;
using RapPhim3.Models;
using RapPhim3.Services;
using RapPhim3.ViewModel;

namespace RapPhim3.Controllers.Admin
{
    public class AdminController : Controller
    {
        private readonly MovieService _movieService;

        public AdminController(MovieService movieService)
        {
            _movieService = movieService;
        }

        public IActionResult Home()
        {
            return View();
        }

        public IActionResult List()
        {
            var movies = _movieService.GetMovies();
            return View(movies);
        }

        public IActionResult Add()
        {
            ViewBag.Genres = _movieService.GetGenres();
            ViewBag.Countries = _movieService.GetCountries();
            return View();
        }

        [HttpPost]
        public IActionResult Add(MovieViewModel model)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState.IsValid = false");
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    foreach (var error in state.Errors)
                    {
                        Console.WriteLine($"Lỗi ở {key}: {error.ErrorMessage}");
                    }
                }

                ViewBag.Genres = _movieService.GetGenres();
                ViewBag.Countries = _movieService.GetCountries();
                return View(model);
            }
            // Kiểm tra GenreIds trước khi xử lý
            if (model.GenreIds == null || !model.GenreIds.Any())
            {
                ModelState.AddModelError("GenreIds", "Vui lòng chọn ít nhất một thể loại.");
                ViewBag.Genres = _movieService.GetGenres();
                ViewBag.Countries = _movieService.GetCountries();
                return View(model);
            }

            // Tạo movie từ dữ liệu nhập vào
            var movie = new Movie
            {
                Title = model.Title,
                Description = model.Description,
                ReleaseDate = model.ReleaseDate,
                Duration = model.Duration,
                LandscapeImage = model.LandscapeImage,
                PortraitImage = model.PortraitImage,
                CountryId = model.CountryId,
                TrailerUrl = model.TrailerUrl,
                Genres = _movieService.GetGenresByIds(model.GenreIds)
            };

            // Xử lý danh sách diễn viên (Actors)
            if (!string.IsNullOrEmpty(model.Actors))
            {
                var actorNames = model.Actors.Split(',').Select(a => a.Trim()).ToList();
                movie.Actors = _movieService.GetOrCreateActors(actorNames);
            }

            _movieService.AddMovie(movie);

            return RedirectToAction("List");
        }
    }
}
