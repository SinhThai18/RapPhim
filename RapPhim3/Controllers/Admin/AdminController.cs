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
            if (movie.TrailerUrl!.Contains("watch?v="))
            {
                movie.TrailerUrl = movie.TrailerUrl.Replace("watch?v=", "embed/");
            }
            ViewBag.Genres = _movieService.GetGenres();
            ViewBag.Countries = _movieService.GetCountries();

            return View(movie);
        }

        // POST: Movies/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Movie movie)
        {
            if (ModelState.IsValid)
            {
                _movieService.UpdateMovie(movie);
                return RedirectToAction("Index");
            }

            ViewBag.Genres = _movieService.GetGenres();
            ViewBag.Countries = _movieService.GetCountries();
            return View(movie);
        }
    }
 }
