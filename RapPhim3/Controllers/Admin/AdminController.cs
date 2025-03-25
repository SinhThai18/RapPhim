using Microsoft.AspNetCore.Mvc;
using RapPhim3.Models;
using RapPhim3.Services;
using RapPhim3.ViewModel;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace RapPhim3.Controllers.Admin
{
    public class AdminController : Controller
    {
        private readonly MovieService _movieService;

        private readonly ShowTimeService _showTimeService;

        private readonly AccountService _accountService;

        public AdminController(MovieService movieService, ShowTimeService showTimeService, AccountService accountService)
        {
            _movieService = movieService;
            _showTimeService = showTimeService;
            _accountService = accountService;
        }

        public IActionResult Home()
        {
            return View();
        }

        public IActionResult List()
        {
            var movies = _movieService.GetMovies();
            return View(movies);
        }

        public IActionResult Add()
        {
            ViewBag.Genres = _movieService.GetGenres();
            ViewBag.Countries = _movieService.GetCountries();
            return View();
        }

        [HttpPost]
        public IActionResult Add(MovieViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Genres = _movieService.GetGenres();
                ViewBag.Countries = _movieService.GetCountries();
                return View(model);
            }

            if (model.GenreIds == null || !model.GenreIds.Any())
            {
                ModelState.AddModelError("GenreIds", "Vui lòng chọn ít nhất một thể loại.");
                ViewBag.Genres = _movieService.GetGenres();
                ViewBag.Countries = _movieService.GetCountries();
                return View(model);
            }

            if (model.TrailerUrl.Contains("watch?v="))
            {
                model.TrailerUrl = model.TrailerUrl.Replace("watch?v=", "embed/");
            }

            var movie = new Movie
            {
                Title = model.Title,
                Description = model.Description,
                ReleaseDate = model.ReleaseDate,
                Duration = model.Duration,
                LandscapeImage = model.LandscapeImage,
                PortraitImage = model.PortraitImage,
                CountryId = model.CountryId,
                TrailerUrl = model.TrailerUrl,
                Genres = _movieService.GetGenresByIds(model.GenreIds),
                Actors = _movieService.GetOrCreateActors(model.Actors?.Split(',').Select(a => a.Trim()).ToList()),
                Directors = _movieService.GetOrCreateDirectors(model.Directors?.Split(',').Select(d => d.Trim()).ToList())
            };

            // Lưu vào session
            HttpContext.Session.SetString("PendingMovie", JsonConvert.SerializeObject(movie));

            return RedirectToAction("Demo");
        }

        public IActionResult Demo()
        {
            var movieJson = HttpContext.Session.GetString("PendingMovie");
            if (string.IsNullOrEmpty(movieJson))
            {
                return RedirectToAction("Add");
            }

            var movie = JsonConvert.DeserializeObject<Movie>(movieJson);
            return View(movie);
        }

        [HttpPost]
        public IActionResult Confirm()
        {
            var movieJson = HttpContext.Session.GetString("PendingMovie");
            if (string.IsNullOrEmpty(movieJson))
            {
                return RedirectToAction("Add");
            }

            var movie = JsonConvert.DeserializeObject<Movie>(movieJson);
            _movieService.AddMovie(movie);

            HttpContext.Session.Remove("PendingMovie");

            return RedirectToAction("List");
        }
        public IActionResult Edit(int id)
        {
            var movie = _movieService.GetMovieById(id);
            if (movie == null)
            {
                return NotFound();
            }

            // Chuẩn hóa TrailerUrl từ "watch?v=" thành "embed/"
            if (!string.IsNullOrEmpty(movie.TrailerUrl) && movie.TrailerUrl.Contains("watch?v="))
            {
                movie.TrailerUrl = movie.TrailerUrl.Replace("watch?v=", "embed/");
            }

            ViewBag.Genres = _movieService.GetGenres();
            ViewBag.Countries = _movieService.GetCountries();

            var model = new MovieViewModel
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                ReleaseDate = movie.ReleaseDate,
                Duration = movie.Duration,
                LandscapeImage = movie.LandscapeImage,
                PortraitImage = movie.PortraitImage,
                CountryId = movie.CountryId,
                TrailerUrl = movie.TrailerUrl,
                GenreIds = movie.Genres.Select(g => g.Id).ToList(),
                Actors = string.Join(", ", movie.Actors.Select(a => a.Name)),
                Directors = string.Join(", ", movie.Directors.Select(d => d.Name))
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(MovieViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Genres = _movieService.GetGenres();
                ViewBag.Countries = _movieService.GetCountries();
                return View(model);
            }

            var movie = _movieService.GetMovieById(model.Id);
            if (movie == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin cơ bản
            movie.Title = model.Title;
            movie.Description = model.Description;
            movie.ReleaseDate = model.ReleaseDate;
            movie.Duration = model.Duration;
            movie.LandscapeImage = model.LandscapeImage;
            movie.PortraitImage = model.PortraitImage;
            movie.CountryId = model.CountryId;
            movie.TrailerUrl = model.TrailerUrl;

            // Cập nhật thể loại
            movie.Genres = _movieService.GetGenresByIds(model.GenreIds);

            // Cập nhật danh sách diễn viên
            movie.Actors = _movieService.GetOrCreateActors(model.Actors?.Split(',').Select(a => a.Trim()).ToList());

            // Cập nhật danh sách đạo diễn
            movie.Directors = _movieService.GetOrCreateDirectors(model.Directors?.Split(',').Select(d => d.Trim()).ToList());

            _movieService.UpdateMovie(movie);
            return RedirectToAction("List");
        }


        //-------------------------------------------------------------------------------

        public IActionResult ListShowTimes()
        {
            ViewBag.Rooms = _showTimeService.GetRooms(); // Lấy danh sách phòng chiếu
            ViewBag.Movies = _movieService.GetMovies(); // Lấy danh sách phim

            var showTimes = _showTimeService.ShowTimes();
            return View(showTimes);
        }


        public async Task<IActionResult> AdminProfile()
        
        {
            var userEmail = HttpContext.Session.GetString("Email"); // Lấy email từ session/login
            var user = await _accountService.GetUserByEmail(userEmail);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> AdminProfile(int id, string fullName, string email, string phoneNumber, string password)
        {
            var result = await _accountService.UpdateUserProfile(id, fullName, email, phoneNumber, password);

            if (result)
            {
                TempData["SuccessMessage"] = "Cập nhật thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Cập nhật thất bại!";
            }

            return RedirectToAction("AdminProfile");
        }

        [HttpPost]
        public IActionResult AddShowTime(string ShowDate, int RoomId, string ShowTime, int MovieId)
        {
            if (string.IsNullOrEmpty(ShowDate) || RoomId == 0 || string.IsNullOrEmpty(ShowTime) || MovieId == 0)
            {
                TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ thông tin!";
                return RedirectToAction("ListShowTimes");
            }

            bool success = _showTimeService.AddShowTime(ShowDate, RoomId, ShowTime, MovieId);
            if (success)
            {
                TempData["SuccessMessage"] = "Thêm suất chiếu thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Suất chiếu bị trùng thời gian với suất chiếu khác!";
            }

            return RedirectToAction("ListShowTimes");
        }

        [HttpGet]
        public IActionResult GetShowTime(int id)
        {
            var showTime = _showTimeService.GetShowTimeById(id);
            if (showTime == null) return NotFound();

            return Json(new
            {
                id = showTime.Id,
                showDate = showTime.ShowDate.ToString("yyyy-MM-dd"),
                showTime = showTime.ShowTime1.ToString(@"HH\:mm"),
                roomId = showTime.Room.Id,
                movieId = showTime.Movie.Id
            });
        }

        [HttpGet]
        public IActionResult CheckDuplicateShowTime(DateTime showDate, string showTime, int roomId, int showTimeId)
        {
            bool isDuplicate = _showTimeService.CheckDuplicateShowTime(showDate, showTime, roomId, showTimeId);
            return Json(new { isDuplicate });
        }

        [HttpPost]
        public IActionResult EditShowTime(ShowTime model)
        {
            bool success = _showTimeService.EditShowTime(model);
            if (!success)
            {
                TempData["ErrorMessage"] = "Suất chiếu này đã tồn tại hoặc có lỗi!";
                return RedirectToAction("Index");
            }

            TempData["SuccessMessage"] = "Chỉnh sửa suất chiếu thành công!";
            return RedirectToAction("ListShowTimes");
        }
    }
}

