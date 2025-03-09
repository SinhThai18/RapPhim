using RapPhim3.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

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
                .Include(m => m.Genres)
                .Include(m => m.Country)
                .ToList();
        }

        public Movie? GetMovieById(int id)
        {
            return _context.Movies
                .FirstOrDefault(m => m.Id == id);
        }

        public List<Genre> GetGenres()
        {
            return _context.Genres.ToList();
        }

        public List<Country> GetCountries()
        {
            return _context.Countries.ToList();
        }

        
        public List<Movie> FilterMovies(int? genreId, int? countryId, int? year)
        {
            var movies = _context.Movies
                .Include(m => m.Genres)
                .Include(m => m.Country)
                .AsQueryable();

            if (genreId.HasValue)
            {
                movies = movies.Where(m => m.Genres.Any(g => g.Id == genreId.Value));
            }

            if (countryId.HasValue)
            {
                movies = movies.Where(m => m.Country.Id == countryId.Value);
            }

            if (year.HasValue)
            {
                movies = movies.Where(m => m.ReleaseDate.HasValue && m.ReleaseDate.Value.Year == year.Value);
            }

            return movies.ToList();
        }
    }
}
