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
                .ToList();
        }

        public Movie? GetMovieById(int id)
        {
            return _context.Movies
                .FirstOrDefault(m => m.Id == id);
        }

        public List<Genre> GetGenres()
        {
            return _context.Genres
                .ToList();
        }

        public List<Country> GetCountries()
        {
            return _context.Countries
                .ToList();
        }
    }
}
