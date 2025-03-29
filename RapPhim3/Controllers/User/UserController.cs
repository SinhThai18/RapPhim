using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RapPhim3.Models;
using RapPhim3.Services;
using RapPhim3.ViewModel;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace RapPhim3.Controllers.User
{
    public class UserController : Controller
    {

        private readonly UserService _userService;
        private readonly MovieService _movieService;
        private readonly ShowTimeService _showTimeService;
        private readonly SeatService _seatService;
        private readonly TicketService _ticketService;

        public UserController(UserService userService, MovieService movieService, ShowTimeService showTimeService,
           SeatService seatService, TicketService ticketService)
        {
            _userService = userService;
            _movieService = movieService;
            _showTimeService = showTimeService;
            _seatService = seatService;
            _ticketService = ticketService;
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

            // Lấy danh sách vé đã thanh toán của user
            var paidTickets = await _ticketService.GetPaidTicketsByUser(user.Id);

            var viewModel = new ProfileViewModel
            {
                User = user,
                PaidTickets = paidTickets
            };

            return View(viewModel);
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
            var now = DateTime.Now;
            var showTimes = _showTimeService.GetShowTimesByDate(movieId, selectedDate)
                .Select(st => new
                {
                    Id = (int)st.GetType().GetProperty("id").GetValue(st),
                    Time = TimeOnly.Parse((string)st.GetType().GetProperty("time").GetValue(st)).ToString("HH:mm")
                })
                .Where(st => !(selectedDate == DateOnly.FromDateTime(now) && TimeOnly.Parse(st.Time) < TimeOnly.FromDateTime(now)))
                .ToList();

            return Json(showTimes);
        }


        public async Task<IActionResult> GetSeats(int showTimeId)
        {
            // Lấy danh sách các ghế đã có vé (loại bỏ vé có trạng thái "Paid")

            var paidSeats = await _ticketService.GetPaidSeats(showTimeId); // Lấy danh sách ghế đã thanh toán

            var allSeats = await _seatService.GetSeatsByShowTime(showTimeId);

            // Chỉ lấy những ghế không nằm trong danh sách ghế đã thanh toán
            var availableSeats = allSeats
                .Where(seat => !paidSeats.Contains(seat.Id))
                .ToList();

            var groupedSeats = availableSeats
                .GroupBy(s => s.SeatType)
                .Select(g => new
                {
                    SeatType = g.Key,
                    Seats = g.Select(s => new { s.Id, s.SeatNumber, s.Price }).ToList()
                }).ToList();

            return Json(groupedSeats);
        }



        [HttpGet]
        public IActionResult CheckLogin()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            return Json(new { isLoggedIn = userId.HasValue });
        }


        [HttpPost]
        public async Task<IActionResult> CreatePendingTicket([FromBody] TicketRequestModel request)
        {
            var email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                return Unauthorized("Người dùng chưa đăng nhập.");
            }

            var user = await _userService.GetUserByEmail(email);
            if (user == null)
            {
                return NotFound("Người dùng không tồn tại.");
            }

            var tickets = request.SeatIds.Select(seatId => new Ticket
            {
                ShowTimeId = request.ShowTimeId,
                SeatId = seatId,
                UserId = user.Id,
                Price = _ticketService.GetSeatPrice(seatId).Result,
                PaymentStatus = "Pending"
            }).ToList();

            foreach (var ticket in tickets)
            {
                await _ticketService.CreateTicket(ticket);
            }

            

            return Json(new { ticketId = tickets.First().Id });
        }

        public async Task<IActionResult> Payment(int ticketId)
        {
            if (ticketId == 0 )
            {
                return RedirectToAction("BuyTickets");
            }
            var ticket = await _ticketService.GetTicketById(ticketId);
            if (ticket == null || ticket.PaymentStatus != "Pending")
            {
                return NotFound("Vé không tồn tại hoặc đã thanh toán.");
            }

            // Thông tin thanh toán
            string accountNumber = "1699918082003"; // Số tài khoản MB Bank của bạn
            string bankShortName = "MBBank"; // Tên ngắn của MB Bank theo SePay
            decimal amount = await _ticketService.GetTotalPendingAmountAsync(ticketId); // Số tiền cần thanh toán
            string description = $"Thanh toan ve xem phim so {ticket.Id}"; // Nội dung chuyển khoản
            string encodedDescription = Uri.EscapeDataString(description); // Encode nội dung tránh lỗi URL

            // Tạo đường dẫn QR Code theo cấu trúc của SePay
            string qrCodeUrl = $"https://qr.sepay.vn/img?acc={accountNumber}&bank={bankShortName}&amount={amount}&des={encodedDescription}";

            var model = new PaymentViewModel
            {
                TicketId = ticket.Id,
                Amount = await _ticketService.GetTotalPendingAmountAsync(ticketId),
                QrCodeUrl = qrCodeUrl
            };

            return View(model);
        }

       

        [HttpGet]
        public async Task<IActionResult> VerifyPayment(int ticketId)
        {
            var ticket = await _ticketService.GetTicketById(ticketId);
            if (ticket == null)
            {
                return NotFound(new { success = false, message = "Vé không tồn tại" });
            }

            string accountNumber = "1699918082003"; // Số tài khoản MB Bank
            decimal amount = await _ticketService.GetTotalPendingAmountAsync(ticketId); // Số tiền cần thanh toán
            string apiToken = "LJ5BD8WIAPNYEX6NCA2MWBGXZ4W9HQVMOY9VLDRVX10AEQDMHRJTNR32BBFOFGEI"; // API token

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);

                string apiUrl = $"https://my.sepay.vn/userapi/transactions/list?account_number={accountNumber}&limit=20";

                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
                    if (!response.IsSuccessStatusCode)
                    {
                        return BadRequest(new { success = false, message = "Không thể lấy dữ liệu giao dịch." });
                    }

                    var responseData = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonConvert.DeserializeObject<TransactionResponse>(responseData);

                    if (jsonResponse == null || jsonResponse.Status != 200 || jsonResponse.Transactions == null)
                    {
                        return BadRequest(new { success = false, message = "Lỗi khi lấy danh sách giao dịch." });
                    }

                    foreach (var transaction in jsonResponse.Transactions)
                    {
                        if (transaction.TransactionContent.Contains($"Thanh toan ve xem phim so {ticket.Id}"))
                        {
                            await _ticketService.UpdateTicketsStatusAsync(ticket.Id); // Cập nhật tất cả các vé liên quan

                            // Kiểm tra lại trạng thái của vé sau khi cập nhật
                            var updatedTicket = await _ticketService.GetTicketById(ticket.Id);
                            if (updatedTicket.PaymentStatus == "Paid")
                            {
                                // Tạo QR code chứa thông tin vé
                                string qrData = $"Vé ID: {updatedTicket.Id}";
                                byte[] qrCodeImage = GenerateQRCode(qrData);
                                // Truyền dữ liệu sang PaymentSuccess
                                TempData["TicketId"] = updatedTicket.Id;
                                TempData["QrCodeImage"] = Convert.ToBase64String(qrCodeImage);

                                TempData.Keep("TicketId");
                                TempData.Keep("QrCodeImage");

                            }

                            return Json(new { success = true });
                        }
                    }

                    return Ok(new { success = false, message = "Không tìm thấy giao dịch hợp lệ. Bạn chưa thanh toán!" });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { success = false, message = "Lỗi hệ thống.", error = ex.Message });
                }
            }
        }


        [HttpPost]
        public async Task<IActionResult> CancelPayment(int ticketId)
        {
            var ticket = await _ticketService.GetTicketById(ticketId);
            if (ticket == null || ticket.PaymentStatus != "Pending")
            {
                return Json(new { success = false, message = "Vé không tồn tại hoặc đã được thanh toán." });
            }

            // Xóa vé khỏi database
            bool isDeleted = await _ticketService.DeleteTicket(ticketId);
            if (!isDeleted)
            {
                return Json(new { success = false, message = "Không thể xóa vé, vui lòng thử lại." });
            }

            return Json(new { success = true });
        }


        // Định nghĩa lớp để ánh xạ JSON
        public class TransactionResponse
        {
            [JsonProperty("status")]
            public int Status { get; set; }

            [JsonProperty("transactions")]
            public List<Transaction> Transactions { get; set; }
        }

        public class Transaction
        {
            [JsonProperty("amount_in")]
            public string AmountIn { get; set; }

            [JsonProperty("transaction_content")]
            public string TransactionContent { get; set; }
        }

        [HttpGet]
        public IActionResult PaymentSuccess()
        {  
            return View();
        }



        public IActionResult PaymentFail()
        {
            return View();
        }

        public byte[] GenerateQRCode(string qrText)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(20); // Trả về ảnh QR dưới dạng byte[]
        }

        [HttpGet]
        public async Task<IActionResult> GetTicketQRCode(int ticketId)
        {
            var ticket = await _ticketService.GetTicketById(ticketId);
            if (ticket == null || ticket.PaymentStatus != "paid")
            {
                return NotFound("Vé không tồn tại hoặc chưa thanh toán.");
            }

            string qrContent = $"Vé số: {ticket.Id}";

            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q);
                PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
                byte[] qrCodeImage = qrCode.GetGraphic(20);

                return File(qrCodeImage, "image/png");
            }
        }
    }
}

