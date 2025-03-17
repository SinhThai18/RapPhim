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
    }
}
