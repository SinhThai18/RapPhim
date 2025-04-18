﻿using RapPhim3.Models;
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
                 .Where(t => (t.PaymentStatus == "paid" || t.PaymentStatus == "Success"))
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
                .Where(t => (t.PaymentStatus == "paid" || t.PaymentStatus == "Success"))
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
        //public async Task CreateTicket(Ticket ticket)
        //{
        //    _context.Tickets.Add(ticket);
        //    await _context.SaveChangesAsync();
        //}

        public async Task<bool> CreateTicket(Ticket ticket)
        {
            var existingTicket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.SeatId == ticket.SeatId && t.ShowTimeId == ticket.ShowTimeId);

            if (existingTicket != null && existingTicket.PaymentStatus == "paid")
            {
                return false; // Vé đã tồn tại và đã thanh toán, không thể đặt lại
            }

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            return true; // Vé tạo thành công
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
                .Where(t => t.ShowTimeId == showTimeId && t.PaymentStatus == "paid")
                .Select(t => t.SeatId)
                .ToListAsync();
        }

        public async Task<List<int>> GetPaidSeats(int showTimeId)
        {
            return await _context.Tickets
                .Where(t => t.ShowTimeId == showTimeId &&
                           (t.PaymentStatus == "paid" || t.PaymentStatus == "success")) // Thêm điều kiện lọc
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

        public async Task<bool> UpdateTicketsStatusAsync(int ticketId)
        {
            // Tìm ticket theo ID
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
            {
                throw new Exception("Ticket not found.");
            }

            // Lấy danh sách các vé cần cập nhật
            var ticketsToUpdate = await _context.Tickets
                .Where(t => t.ShowTimeId == ticket.ShowTimeId
                            && t.UserId == ticket.UserId
                            && t.PaymentStatus == "Pending")
                .ToListAsync();

            if (!ticketsToUpdate.Any())
            {
                return false; // Không có vé nào để cập nhật
            }

            // Cập nhật trạng thái paymentStatus thành "paid"
            foreach (var t in ticketsToUpdate)
            {
                t.PaymentStatus = "paid";
            }

            await _context.SaveChangesAsync(); // Lưu thay đổi vào database

            return true;
        }

        public async Task<decimal> GetTotalPendingAmountAsync(int ticketId)
        {
            // Tìm ticket theo ID
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
            {
                throw new Exception("Ticket not found.");
            }

            // Tính tổng price của các vé có cùng showtimeId, userId và paymentStatus = "pending"
            var totalAmount = await _context.Tickets
                .Where(t => t.ShowTimeId == ticket.ShowTimeId
                            && t.UserId == ticket.UserId
                            && t.PaymentStatus == "Pending")
                .SumAsync(t => t.Price);

            return totalAmount;
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
                ticket.PaymentStatus = "paid"; // Cập nhật trạng thái thanh toán
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteTicket(int ticketId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
            {
                return false;
            }
            int? showtimeId = ticket.ShowTimeId;
            int userId = ticket.UserId;

            // Tìm tất cả các vé có cùng showtimeId, userId và paymentStatus = "Pending"
            var ticketsToDelete = _context.Tickets
                .Where(t => t.ShowTimeId == showtimeId
                         && t.UserId == userId
                         && t.PaymentStatus == "Pending")
                .ToList();

            if (ticketsToDelete.Any())
            {
                _context.Tickets.RemoveRange(ticketsToDelete);
                _context.SaveChanges();
                return true; // Xóa thành công
            }

            return false; // Không có vé nào để xóa
        }

        public async Task<List<Ticket>> GetPaidTicketsByUser(int userId)
        {
            return await _context.Tickets
                .Where(t => t.UserId == userId && (t.PaymentStatus == "paid" 
                || t.PaymentStatus == "Success" || t.PaymentStatus=="refund"))
                .Include(t => t.Seat)
                .Include(t => t.ShowTime)
                    .ThenInclude(st => st.Movie) // Bao gồm thông tin phim
                .Include(t => t.ShowTime)
                    .ThenInclude(st => st.Room) // Bao gồm thông tin phòng
                .OrderByDescending(t => t.Id)
                .ToListAsync();
        }




        public async Task<List<Ticket>> GetTicketsAsync()
        {
            return await _context.Tickets
                .Where(t => t.PaymentStatus != "Pending")
                .Include(t => t.ShowTime).ThenInclude(s=>s.Movie)
                .Include(t => t.Seat)
                .Include(t => t.User)
                .OrderByDescending (t => t.Id)
                .ToListAsync();
        }

        public async Task<bool> UpdatePaymentStatusAsync(int ticketId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null) return false;

            ticket.PaymentStatus = "Success";
            await _context.SaveChangesAsync();
            return true;
        }
    }
}