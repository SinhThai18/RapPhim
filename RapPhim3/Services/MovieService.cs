using RapPhim3.Models;
using Microsoft.EntityFrameworkCore;


namespace RapPhim3.Services
{
    public class MovieService
    {
        private readonly RapPhimContext _context;

        public MovieService(RapPhimContext context)
        {
            _context = context;
        }

        public List<Movie> GetMovies()
        {
            return _context.Movies
                .Include(m => m.Director)
                .Include(m => m.Genre)
                .ToList();
        }
    }
}
