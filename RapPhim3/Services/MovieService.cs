using RapPhim3.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using RapPhim3.ViewModel;

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
            // Lấy hoặc tạo mới danh sách diễn viên
            var actors = GetOrCreateActors(movie.Actors.Select(a => a.Name).ToList());
            movie.Actors = actors;

            // Lấy hoặc tạo mới danh sách đạo diễn
            var directors = GetOrCreateDirectors(movie.Directors.Select(d => d.Name).ToList());
            movie.Directors = directors;

            // Lấy danh sách thể loại từ database (tránh thêm thể loại trùng lặp)
            movie.Genres = _context.Genres.Where(g => movie.Genres.Select(mg => mg.Id).Contains(g.Id)).ToList();

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

        public List<Movie> GetTopMoviesByNewest()
        {
            return _context.Movies
                .OrderByDescending(m => m.ReleaseDate) // Sắp xếp theo ngày phát hành mới nhất
                .Take(5)
                .ToList();
        }
        public MovieTypeViewModel GetMovieTypeViewModel()
        {
            var today = DateTime.Today;

            return new MovieTypeViewModel
            {
                AllMovies = _context.Movies.ToList(),
                MoviesShowingToday = _context.Movies
                    .Where(m => m.ShowTimes.Any(st => st.ShowDate == DateOnly.FromDateTime(today)))
                    .ToList(),
                Genres = _context.Genres.ToList(),
                Countries = _context.Countries.ToList()
            };
        }
    }
}
