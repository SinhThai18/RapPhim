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

        public async Task<User> GetUserByUsername(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == username);
        }

        // So sánh mật khẩu nhập vào với mật khẩu đã hash trong database
        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(enteredPassword));
                StringBuilder builder = new StringBuilder();
                foreach (var t in bytes)
                {
                    builder.Append(t.ToString("x2"));
                }
                return builder.ToString() == storedHash;
            }
        }
        public async Task<bool> IsValidUser(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == username);
            if (user == null || !VerifyPassword(password, user.PasswordHash))
            {
                return false;
            }
            if (!user.EmailConfirmed)
            {
                return false;
            }
            return true;
        }

        public async Task<User> GetUserByEmail(string username)
        {
            return await _context.Users
                .Where(u => u.Email == username)
                .Select(u => new User { Id = u.Id,FullName=u.FullName, Email = u.Email,PhoneNumber=u.PhoneNumber,
                    Role=u.Role,PasswordHash = u.PasswordHash })
                .FirstOrDefaultAsync();
        }

       

        public async Task<bool> UpdateUser(int userId, string fullName, string email, string phoneNumber)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false; // Không tìm thấy user
            }

            user.FullName = fullName;
            user.Email = email;
            user.PhoneNumber = phoneNumber;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateUserProfile(int userId, string fullName, string email, string phoneNumber, string password)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.FullName = fullName;
            user.Email = email;
            user.PhoneNumber = phoneNumber;

            if (!string.IsNullOrWhiteSpace(password))
            {
                user.PasswordHash = HashPassword(password);
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
