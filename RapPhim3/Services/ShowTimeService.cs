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

        public bool AddShowTime(string showDate, int roomId, string showTime, int movieId, int bufferTime = 15)
        {
            try
            {
                var movie = _context.Movies.Find(movieId);
                if (movie == null) return false;

                var newShowTime = TimeOnly.Parse(showTime);
                var duration = movie.Duration ?? 0; // Nếu Duration là null, gán mặc định 0
                var endTime = newShowTime.AddMinutes(duration + bufferTime); // Thêm bufferTime vào thời gian kết thúc

                // Kiểm tra trùng lịch chiếu
                var existingShowTimes = _context.ShowTimes
                    .Include(s => s.Movie) 
                    .Where(s => s.RoomId == roomId && s.ShowDate == DateOnly.Parse(showDate))
                    .ToList();

                foreach (var existingShow in existingShowTimes)
                {
                    var existingDuration = existingShow.Movie.Duration ?? 0;
                    var existingEndTime = existingShow.ShowTime1.AddMinutes(existingDuration + bufferTime); // Thêm bufferTime vào suất chiếu cũ

                    if ((newShowTime >= existingShow.ShowTime1 && newShowTime < existingEndTime) ||
                        (endTime > existingShow.ShowTime1 && endTime <= existingEndTime))
                    {
                        return false; // Suất chiếu bị trùng
                    }
                }

                var newShow = new ShowTime
                {
                    ShowDate = DateOnly.Parse(showDate),
                    RoomId = roomId,
                    ShowTime1 = newShowTime,
                    MovieId = movieId
                };

                _context.ShowTimes.Add(newShow);
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


        public List<Room> GetRooms()
        {
            return _context.Rooms.ToList();
        }
    }
}
