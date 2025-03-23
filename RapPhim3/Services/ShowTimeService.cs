using Microsoft.EntityFrameworkCore;
using RapPhim3.Models;

namespace RapPhim3.Services
{
    public class ShowTimeService
    {
        private readonly RapPhimContext _context;

        public ShowTimeService(RapPhimContext context)
        {
            _context = context;
        }

        public List<ShowTime> ShowTimes()
        {
            return _context.ShowTimes
                .Include(s=>s.Movie)
                .Include(s=>s.Room)
                .ToList();

        }

        public List<Movie> GetMoviesByDate(DateOnly date)
        {
            return _context.ShowTimes
                .Where(s => s.ShowDate == date)
                .Select(s => s.Movie)
                .Distinct()
                .ToList();
        }

        public List<ShowTime> GetShowTimesByDate(DateOnly date)
        {
            return _context.ShowTimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .Where(s => s.ShowDate == date)
                .ToList();
        }

    }
}
