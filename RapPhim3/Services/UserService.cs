using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RapPhim3.Models;

namespace RapPhim3.Services
{
    public class UserService
    {
        private readonly RapPhimContext _context;

        public UserService(RapPhimContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return null;

            return await _context.Users
                .Where(u => u.Email == email)
                .Select(u => new User
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    PasswordHash = u.PasswordHash
                })
                .FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserById(int userId)
        {
            return await _context.Users.FindAsync(userId);
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
    }
}
