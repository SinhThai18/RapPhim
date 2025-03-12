using System;
using System.Linq;
using System.Threading.Tasks;
using RapPhim3.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace RapPhim3.Services
{
    public class AccountService
    {
        private readonly RapPhimContext _context;

        public AccountService(RapPhimContext context)
        {
            _context = context;
        }

        // Hàm đăng ký tài khoản
        public async Task<bool> RegisterUser(string fullName, string email, string password, string phoneNumber)
        {
            if (_context.Users.Any(u => u.Email == email))
            {
                return false; // Email đã tồn tại
            }

            var newUser = new User
            {
                FullName = fullName,
                Email = email,
                PasswordHash = HashPassword(password),
                PhoneNumber = phoneNumber,
                Role = "Customer",
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return true;
        }

        // Hàm hash password bằng SHA256
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (var t in bytes)
                {
                    builder.Append(t.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public async Task<bool> ConfirmEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || user.EmailConfirmed)
            {
                return false; // Người dùng không tồn tại hoặc đã xác nhận trước đó
            }

            user.EmailConfirmed = true;
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
