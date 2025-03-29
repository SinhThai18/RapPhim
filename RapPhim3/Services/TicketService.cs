using RapPhim3.Models;
using Microsoft.EntityFrameworkCore;
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
                 .Where(t => t.PaymentStatus == "Paid")
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
                .Where(t => t.PaymentStatus == "Paid")
                .GroupBy(t => t.BookingTime.Date)
                .Select(g => new DailyRevenueViewModel
                {
                    Date = g.Key,
                    TotalRevenue = g.Sum(t => t.Price)
                })
                .OrderBy(g => g.Date)
                .ToListAsync();
        }

        // Tạo vé mới
        public async Task CreateTicket(Ticket ticket)
        {
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
        }

        // Lấy giá vé dựa vào ghế
        public async Task<decimal> GetSeatPrice(int seatId)
        {
            var seat = await _context.Seats.FindAsync(seatId);
            return seat?.Price ?? 0; // Nếu không tìm thấy, trả về 0
        }

        public async Task<List<int>> GetBookedSeats(int showTimeId)
        {
            return await _context.Tickets
                .Where(t => t.ShowTimeId == showTimeId && t.PaymentStatus == "Paid")
                .Select(t => t.SeatId)
                .ToListAsync();
        }

        public async Task<List<int>> GetPaidSeats(int showTimeId)
        {
            return await _context.Tickets
                .Where(t => t.ShowTimeId == showTimeId && t.PaymentStatus == "Paid")
                .Select(t => t.SeatId)
                .ToListAsync();
        }


        // Lấy danh sách vé theo User
        public async Task<List<Ticket>> GetTicketsByUser(int userId)
        {
            return await _context.Tickets
                .Where(t => t.UserId == userId)
                .Include(t => t.Seat)
                .Include(t => t.ShowTime)
                .ToListAsync();
        }

        // Lấy chi tiết vé theo ID
        public async Task<Ticket?> GetTicketById(int ticketId)
        {
            return await _context.Tickets
                .Include(t => t.Seat)
                .Include(t => t.ShowTime)
                .FirstOrDefaultAsync(t => t.Id == ticketId);
        }

        public async Task UpdateTicket(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
        }

        public decimal CalculateTotalPrice(List<int> seatIds)
        {
            decimal totalPrice = 0;

            foreach (var seatId in seatIds)
            {
                var seat = _context.Seats.FirstOrDefault(s => s.Id == seatId);
                if (seat != null)
                {
                    totalPrice += seat.Price;
                }
            }

            return totalPrice;
        }

        public async Task MarkTicketsAsPaid(int showTimeId, List<int> seatIds)
        {
            var tickets = await _context.Tickets
                .Where(t => t.ShowTimeId == showTimeId && seatIds.Contains(t.SeatId))
                .ToListAsync();

            foreach (var ticket in tickets)
            {
                ticket.PaymentStatus = "Paid"; // Cập nhật trạng thái thanh toán
            }

            await _context.SaveChangesAsync();
        }


    }
}