using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RapPhim3.Models;
using RapPhim3.Services;

namespace RapPhim3.Controllers.User
{
    public class UserController : Controller
    {
        
        private readonly UserService userService;

        public UserController(UserService userService)
        {
            this.userService = userService;
        }

        public async Task<IActionResult> Profile()
        {
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(role) || role != "Customer")
            {
                return RedirectToAction("Index", "Home");
            }
            var email = HttpContext.Session.GetString("Email");

            if (email == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var user = await userService.GetUserByEmail(email);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(int Id, string FullName, string Email, string PhoneNumber, string NewPassword, string ConfirmPassword)
        {
            if (NewPassword != ConfirmPassword)
            {
                ModelState.AddModelError("Password", "Mật khẩu xác nhận không khớp.");
                return View("Profile", await userService.GetUserById(Id));
            }

            bool result = await userService.UpdateUserProfile(Id, FullName, Email, PhoneNumber, NewPassword);
            if (!result)
            {
                ModelState.AddModelError("", "Cập nhật không thành công!");
                return View("Profile", await userService.GetUserById(Id));
            }

            TempData["Success"] = "Cập nhật thành công!";
            return RedirectToAction("Profile");
        }
    }
}
