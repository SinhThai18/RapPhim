using Microsoft.AspNetCore.Mvc;
using RapPhim3.Services;

namespace RapPhim3.Controllers.Account
{
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;
        private readonly EmailService _emailService;

        public AccountController(AccountService accountService, EmailService emailService)
        {
            _accountService = accountService;
            _emailService = emailService;
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
                TempData["Success"] = "Tài khoản đã được xác nhận thành công!";
            }
            else
            {
                TempData["Error"] = "Xác nhận tài khoản thất bại. Có thể email không tồn tại hoặc đã được xác nhận trước đó.";
            }

            return RedirectToAction("Index", "Home");
        }


    }
}

