using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using RapPhim3.Services;
using RapPhim3.ViewModel;

namespace RapPhim3.Controllers.Account
{
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;
        private readonly EmailService _emailService;
        private readonly IMemoryCache _cache;
        private const int MAX_ATTEMPTS = 5;
        private const int BLOCK_TIME_MINUTES = 15;

        public AccountController(AccountService accountService, EmailService emailService, IMemoryCache cache)
        {
            _accountService = accountService;
            _emailService = emailService;
            _cache = cache;
        }

        [HttpPost]
        public async Task<IActionResult> Register(string fullName, string email, string password, string phoneNumber)
        {
            bool isRegistered = await _accountService.RegisterUser(fullName, email, password, phoneNumber);

            if (!isRegistered)
            {
                TempData["Error"] = "Email đã tồn tại!";
                return RedirectToAction("Index", "Home");
            }

            // Gửi email xác nhận
            string subject = "Xác nhận tài khoản";
            string body = $"<p>Chào {fullName},</p><p>Vui lòng nhấn vào <a href='https://localhost:7156/Account/Confirm?email={email}'>đây</a> để xác nhận tài khoản.</p>";
            await _emailService.SendEmail(email, subject, body);

            TempData["Success"] = "Vui lòng kiểm tra email để xác nhận tài khoản!";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Confirm(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Email không hợp lệ!";
                return RedirectToAction("Index", "Home");
            }

            bool isConfirmed = await _accountService.ConfirmEmail(email);

            if (isConfirmed)
            {
                TempData["Success"] = "Tài khoản đã được xác nhận thành công! Vui lòng đăng nhập lại";
            }
            else
            {
                TempData["Error"] = "Xác nhận tài khoản thất bại. Có thể email không tồn tại hoặc đã được xác nhận trước đó.";
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            string cacheKey = $"LoginAttempts_{username}";
            int attempts = _cache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(BLOCK_TIME_MINUTES);
                return 0;
            });

            if (attempts >= MAX_ATTEMPTS)
            {
                return Json(new { success = false, message = "Bạn đã nhập sai quá nhiều lần. Hãy thử lại sau 15 phút." });
            }

            if (await _accountService.IsValidUser(username, password))
            {
                _cache.Remove(cacheKey); // Xóa cache nếu đăng nhập thành công
                return Json(new { success = true, message = "Đăng nhập thành công!" });
            }
            else
            {
                _cache.Set(cacheKey, attempts + 1, TimeSpan.FromMinutes(BLOCK_TIME_MINUTES));
                return Json(new { success = false, message = $"Sai tài khoản hoặc mật khẩu! Bạn còn {MAX_ATTEMPTS - attempts - 1} lần thử." });
            }
        }

    }
}


