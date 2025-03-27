using System;
using RapPhim3.Models;

namespace RapPhim3.Services
{
    public class SeatService
    {
        private readonly RapPhimContext _context;

        public SeatService(RapPhimContext context)
        {
            _context = context;
        }

        public List<object> GetSeatsByShowTime(int showTimeId)
        {
            return _context.Seats
                .Where(s => s.Room.ShowTimes.Any(st => st.Id == showTimeId))
                .GroupBy(s => s.SeatType)
                .Select(g => new
                {
                    SeatType = g.Key,
                    Seats = g.Select(s => new { s.Id, s.SeatNumber, s.Price }).ToList()
                })
                .ToList<object>();
        }

    }
}
