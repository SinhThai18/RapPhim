using Microsoft.AspNetCore.Mvc;
using RapPhim3.Services;

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
            return View();
        }

        public IActionResult Update()
        {
            return View();
        }
    }
}
