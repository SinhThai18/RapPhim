using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RapPhim3.Models; 
using System.Linq;

namespace RapPhim3.Controllers.Movies
{
    public class MoviesController : Controller
    {
        private readonly RapPhimContext _context; // Database context

        public MoviesController(RapPhimContext context)
        {
            _context = context;
        }

        [Route("Movies/MovieDetail/{id}")]
        public IActionResult MovieDetail(int id)
        {
            var movie = _context.Movies.
                Include(m => m.Country).
                Include(m => m.Genres).
                Include(m => m.Directors).
                Include(m => m.Actors).
                FirstOrDefault(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }
            if (movie.TrailerUrl.Contains("watch?v="))
            {
                movie.TrailerUrl = movie.TrailerUrl.Replace("watch?v=", "embed/");
            }
            return View("MovieDetail", movie);
        }

    }
}
