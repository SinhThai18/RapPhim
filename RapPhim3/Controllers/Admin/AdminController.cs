using Microsoft.AspNetCore.Mvc;
using RapPhim3.Models;
using RapPhim3.Services;
using RapPhim3.ViewModel;
using Newtonsoft.Json;

namespace RapPhim3.Controllers.Admin
{
    public class AdminController : Controller
    {
        private readonly MovieService _movieService;

        private readonly ShowTimeService _showTimeService;

        public AdminController(MovieService movieService, ShowTimeService showTimeService)
        {
            _movieService = movieService;
            _showTimeService = showTimeService;
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
                ViewBag.Genres = _movieService.GetGenres();
                ViewBag.Countries = _movieService.GetCountries();
                return View(model);
            }

            if (model.GenreIds == null || !model.GenreIds.Any())
            {
                ModelState.AddModelError("GenreIds", "Vui lòng chọn ít nhất một thể loại.");
                ViewBag.Genres = _movieService.GetGenres();
                ViewBag.Countries = _movieService.GetCountries();
                return View(model);
            }

            if (model.TrailerUrl.Contains("watch?v="))
            {
                model.TrailerUrl = model.TrailerUrl.Replace("watch?v=", "embed/");
            }

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
                Genres = _movieService.GetGenresByIds(model.GenreIds),
                Actors = _movieService.GetOrCreateActors(model.Actors?.Split(',').Select(a => a.Trim()).ToList()),
                Directors = _movieService.GetOrCreateDirectors(model.Directors?.Split(',').Select(d => d.Trim()).ToList())
            };

            // Lưu vào session
            HttpContext.Session.SetString("PendingMovie", JsonConvert.SerializeObject(movie));

            return RedirectToAction("Demo");
        }

        public IActionResult Demo()
        {
            var movieJson = HttpContext.Session.GetString("PendingMovie");
            if (string.IsNullOrEmpty(movieJson))
            {
                return RedirectToAction("Add");
            }

            var movie = JsonConvert.DeserializeObject<Movie>(movieJson);
            return View(movie);
        }

        [HttpPost]
        public IActionResult Confirm()
        {
            var movieJson = HttpContext.Session.GetString("PendingMovie");
            if (string.IsNullOrEmpty(movieJson))
            {
                return RedirectToAction("Add");
            }

            var movie = JsonConvert.DeserializeObject<Movie>(movieJson);
            _movieService.AddMovie(movie);

            HttpContext.Session.Remove("PendingMovie");

            return RedirectToAction("List");
        }
        public IActionResult Edit(int id)
        {
            var movie = _movieService.GetMovieById(id);
            if (movie == null)
            {
                return NotFound();
            }

            // Chuẩn hóa TrailerUrl từ "watch?v=" thành "embed/"
            if (!string.IsNullOrEmpty(movie.TrailerUrl) && movie.TrailerUrl.Contains("watch?v="))
            {
                movie.TrailerUrl = movie.TrailerUrl.Replace("watch?v=", "embed/");
            }

            ViewBag.Genres = _movieService.GetGenres();
            ViewBag.Countries = _movieService.GetCountries();

            var model = new MovieViewModel
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                ReleaseDate = movie.ReleaseDate,
                Duration = movie.Duration,
                LandscapeImage = movie.LandscapeImage,
                PortraitImage = movie.PortraitImage,
                CountryId = movie.CountryId,
                TrailerUrl = movie.TrailerUrl,
                GenreIds = movie.Genres.Select(g => g.Id).ToList(),
                Actors = string.Join(", ", movie.Actors.Select(a => a.Name)),
                Directors = string.Join(", ", movie.Directors.Select(d => d.Name))
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(MovieViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Genres = _movieService.GetGenres();
                ViewBag.Countries = _movieService.GetCountries();
                return View(model);
            }

            var movie = _movieService.GetMovieById(model.Id);
            if (movie == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin cơ bản
            movie.Title = model.Title;
            movie.Description = model.Description;
            movie.ReleaseDate = model.ReleaseDate;
            movie.Duration = model.Duration;
            movie.LandscapeImage = model.LandscapeImage;
            movie.PortraitImage = model.PortraitImage;
            movie.CountryId = model.CountryId;
            movie.TrailerUrl = model.TrailerUrl;

            // Cập nhật thể loại
            movie.Genres = _movieService.GetGenresByIds(model.GenreIds);

            // Cập nhật danh sách diễn viên
            movie.Actors = _movieService.GetOrCreateActors(model.Actors?.Split(',').Select(a => a.Trim()).ToList());

            // Cập nhật danh sách đạo diễn
            movie.Directors = _movieService.GetOrCreateDirectors(model.Directors?.Split(',').Select(d => d.Trim()).ToList());

            _movieService.UpdateMovie(movie);
            return RedirectToAction("List");
        }


        //-------------------------------------------------------------------------------

        public IActionResult ListShowTimes()
        {
            var showTimes = _showTimeService.ShowTimes();
            return View(showTimes);
        }

    }
}
