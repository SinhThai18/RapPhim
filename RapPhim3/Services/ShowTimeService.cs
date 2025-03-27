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
                .OrderByDescending(s => s.ShowDate) // Sắp xếp theo ShowDate giảm dần
                .ToList();
        }

        public bool AddShowTime(string showDate, int roomId, string showTime, int movieId, int bufferTime = 60)
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

        public ShowTime? GetShowTimeById(int id)
        {
            return _context.ShowTimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .FirstOrDefault(s => s.Id == id);
        }

        public bool CheckDuplicateShowTime(DateTime showDate, string showTime, int roomId, int showTimeId)
        {
            try
            {
                DateOnly parsedDate = DateOnly.FromDateTime(showDate); 
                TimeOnly parsedShowTime = TimeOnly.Parse(showTime);    // Chuyển string -> TimeOnly

                return _context.ShowTimes.Any(s =>
                    s.ShowDate == parsedDate &&
                    s.ShowTime1 == parsedShowTime &&
                    s.RoomId == roomId && // Dùng trực tiếp RoomId thay vì s.Room.Id
                    s.Id != showTimeId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi kiểm tra trùng suất chiếu: " + ex.Message);
                return false;
            }
        }


        public bool EditShowTime(ShowTime model)
        {
            var existingShowTime = _context.ShowTimes.Find(model.Id);
            if (existingShowTime == null) return false;

            // Kiểm tra trùng suất chiếu trước khi cập nhật
            if (_context.ShowTimes.Any(s =>
                s.ShowDate == model.ShowDate &&
                s.ShowTime1 == model.ShowTime1 &&
                s.RoomId == model.RoomId &&
                s.Id != model.Id))
            {
                return false;
            }

            existingShowTime.ShowDate = model.ShowDate;
            existingShowTime.ShowTime1 = model.ShowTime1;
            existingShowTime.RoomId = model.RoomId;
            existingShowTime.MovieId = model.MovieId;

            _context.SaveChanges();
            return true;
        }


        public List<DateOnly> GetShowDatesByMovie(int movieId)
        {
            return _context.ShowTimes.Where(st => st.MovieId == movieId)
                .Select(st => st.ShowDate)
                .Distinct()
                .ToList();
        }

        public List<object> GetShowTimesByDate(int movieId, DateOnly date)
        {
            return _context.ShowTimes
                .Where(st => st.MovieId == movieId && st.ShowDate == date)
                .Select(st => new { id = st.Id, time = st.ShowTime1.ToString("HH:mm") })
                .ToList<object>();
        }
    }
}
