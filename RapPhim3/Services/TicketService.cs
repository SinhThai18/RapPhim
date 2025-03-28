using Microsoft.EntityFrameworkCore;
using RapPhim3.Models;
using RapPhim3.ViewModel;

namespace RapPhim3.Services
{
    public class TicketService
    {
        private readonly RapPhimContext _context;

        public TicketService(RapPhimContext context)
        {
            _context = context;
        }

        // Lấy doanh thu theo phim
        public async Task<List<MovieRevenueViewModel>> GetRevenueByMovieAsync()
        {
            return await _context.Tickets
                .Include(t => t.ShowTime)
                .ThenInclude(st => st.Movie)
                .GroupBy(t => t.ShowTime.Movie.Title)
                .Select(g => new MovieRevenueViewModel
                {
                    MovieTitle = g.Key,
                    TotalRevenue = g.Sum(t => t.Price)
                })
                .ToListAsync();
        }

        // Lấy doanh thu theo ngày
        public async Task<List<DailyRevenueViewModel>> GetRevenueByDateAsync()
        {
            return await _context.Tickets
                .GroupBy(t => t.BookingTime.Date)
                .Select(g => new DailyRevenueViewModel
                {
                    Date = g.Key,
                    TotalRevenue = g.Sum(t => t.Price)
                })
                .OrderBy(g => g.Date)
                .ToListAsync();
        }
    }

}


