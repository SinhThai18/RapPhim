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
                .FirstOrDefault(m => m.Id == id); ;
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

        public List<Genre> GetGenresByIds(List<int> genreIds)
        {
            return _context.Genres.Where(g => genreIds.Contains(g.Id)).ToList();
        }

        public void AddMovie(Movie movie)
        {
            foreach (var actor in movie.Actors)
            {
                actor.Id = 0; // Đảm bảo ID = 0 để EF tự động sinh ID
            }

            foreach (var director in movie.Directors)
            {
                director.Id = 0;
            }

            foreach (var genre in movie.Genres)
            {
                genre.Id = 0;
            }

            _context.Movies.Add(movie);
            _context.SaveChanges();
        }


        public List<Actor> GetOrCreateActors(List<string> actorNames)
        {
            var existingActors = _context.Actors.Where(a => actorNames.Contains(a.Name)).ToList();

            var newActors = actorNames
                .Where(name => !existingActors.Any(a => a.Name == name))
                .Select(name => new Actor { Name = name })
                .ToList();

            _context.Actors.AddRange(newActors);
            _context.SaveChanges();

            return existingActors.Concat(newActors).ToList();
        }

        public List<Director> GetOrCreateDirectors(List<string> directorNames)
        {
            var existingDirectors = _context.Directors
                .Where(d => directorNames.Contains(d.Name))
                .ToList();

            var newDirectors = directorNames
                .Where(d => existingDirectors.All(ed => ed.Name != d))
                .Select(d => new Director { Name = d })
                .ToList();

            if (newDirectors.Any())
            {
                _context.Directors.AddRange(newDirectors);
                _context.SaveChanges();
            }

            return existingDirectors.Concat(newDirectors).ToList();
        }

        public void UpdateMovie(Movie updatedMovie)
        {
            var existingMovie = _context.Movies
                .Include(m => m.Genres)
                .Include(m => m.Actors)
                .Include(m => m.Directors)
                .FirstOrDefault(m => m.Id == updatedMovie.Id);

            if (existingMovie == null)
            {
                throw new KeyNotFoundException("Không tìm thấy phim.");
            }

            // Cập nhật thông tin cơ bản
            existingMovie.Title = updatedMovie.Title;
            existingMovie.Description = updatedMovie.Description;
            existingMovie.ReleaseDate = updatedMovie.ReleaseDate;
            existingMovie.CountryId = updatedMovie.CountryId;

            // Cập nhật danh sách thể loại
            existingMovie.Genres.Clear();
            foreach (var genre in _context.Genres.Where(g => updatedMovie.Genres.Select(ug => ug.Id).Contains(g.Id)))
            {
                existingMovie.Genres.Add(genre);
            }

            // Cập nhật danh sách diễn viên
            existingMovie.Actors.Clear();
            var actors = GetOrCreateActors(updatedMovie.Actors.Select(a => a.Name).ToList());
            foreach (var actor in actors)
            {
                existingMovie.Actors.Add(actor);
            }

            // Cập nhật danh sách đạo diễn
            existingMovie.Directors.Clear();
            var directors = GetOrCreateDirectors(updatedMovie.Directors.Select(d => d.Name).ToList());
            foreach (var director in directors)
            {
                existingMovie.Directors.Add(director);
            }

            _context.SaveChanges();
        }


    }
}
