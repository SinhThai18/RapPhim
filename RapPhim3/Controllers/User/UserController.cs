using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RapPhim3.Models;
using RapPhim3.Services;
using RapPhim3.ViewModel;

namespace RapPhim3.Controllers.User
{
    public class UserController : Controller
    {
        
        private readonly UserService _userService;
        private readonly MovieService _movieService;
        private readonly ShowTimeService _showTimeService;
        private readonly SeatService _seatService;

        public UserController(UserService userService, MovieService movieService, ShowTimeService showTimeService,
           SeatService seatService )
        {
            _userService = userService;
            _movieService = movieService;
            _showTimeService = showTimeService;
            _seatService = seatService;
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
            var user = await _userService.GetUserByEmail(email);
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
                return View("Profile", await _userService.GetUserById(Id));
            }

            bool result = await _userService.UpdateUserProfile(Id, FullName, Email, PhoneNumber, NewPassword);
            if (!result)
            {
                ModelState.AddModelError("", "Cập nhật không thành công!");
                return View("Profile", await _userService.GetUserById(Id));
            }

            TempData["Success"] = "Cập nhật thành công!";
            return RedirectToAction("Profile");
        }

        public IActionResult BuyTickets(int movieId)
        {
            var movie = _movieService.GetMovieById(movieId);
            if (movie == null)
                return NotFound();

            var showDates = _showTimeService.GetShowDatesByMovie(movieId);

            var viewModel = new BuyTicketViewModel
            {
                Movie = movie,
                ShowDates = showDates
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult GetShowTimes(int movieId, DateOnly selectedDate)
        {
            var showTimes = _showTimeService.GetShowTimesByDate(movieId, selectedDate);
            return Json(showTimes);
        }

        [HttpGet]
        public IActionResult GetSeats(int showTimeId)
        {
            var seats = _seatService.GetSeatsByShowTime(showTimeId);
            return Json(seats);
        }
    }
}
