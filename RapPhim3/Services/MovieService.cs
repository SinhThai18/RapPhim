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
                .AsNoTracking() // Ngăn không cho EF Core giữ trạng thái tracking
                .Include(m => m.Genres)
                .Include(m => m.Country)
                .Include(m => m.Actors)
                .Include(m => m.Directors)
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
            //sua loi tieng viet co dau
            var existingActors = _context.Actors
                .AsEnumerable()
                .Where(a => actorNames.Any(name => string.Equals(a.Name, name, StringComparison.InvariantCultureIgnoreCase)))
                .ToList();
            
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
     .AsEnumerable()
     .Where(d => directorNames.Any(name => string.Equals(d.Name, name, StringComparison.InvariantCultureIgnoreCase)))
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
            existingMovie.Genres = _context.Genres
                .Where(g => updatedMovie.Genres.Select(ug => ug.Id).Contains(g.Id))
                .ToList();

            // Cập nhật danh sách diễn viên
            var newActors = GetOrCreateActors(updatedMovie.Actors.Select(a => a.Name).ToList());
            existingMovie.Actors = newActors.ToList(); // Gán danh sách mới tránh lỗi

            // Cập nhật danh sách đạo diễn
            var newDirectors = GetOrCreateDirectors(updatedMovie.Directors.Select(d => d.Name).ToList());
            existingMovie.Directors = newDirectors.ToList();

            _context.SaveChanges();
        }


    }
}
