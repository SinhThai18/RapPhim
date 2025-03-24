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
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .ToList();
        }

        public List<TimeOnly> GetAvailableShowTimes(int roomId, DateOnly showDate)
        {
            var allShowTimes = new List<TimeOnly>
            {
                new TimeOnly(9, 0),
                new TimeOnly(14, 0),
                new TimeOnly(17, 0)
            };

            var bookedShowTimes = _context.ShowTimes
                .Where(s => s.RoomId == roomId && s.ShowDate == showDate)
                .Select(s => s.ShowTime1)
                .ToList();

            return allShowTimes.Except(bookedShowTimes).ToList();
        }

        public bool AddShowTime(string showDate, int roomId, string showTime, int movieId)
        {
            try
            {
                var newShowTime = new ShowTime
                {
                    ShowDate = DateOnly.Parse(showDate),
                    RoomId = roomId,
                    ShowTime1 = TimeOnly.Parse(showTime),
                    MovieId = movieId
                };

                _context.ShowTimes.Add(newShowTime);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
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
