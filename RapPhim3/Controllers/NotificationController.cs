using Microsoft.AspNetCore.Mvc;
using RapPhim3.Models;

namespace RapPhim3.Controllers
{
    public class NotificationController : Controller
    {

        private readonly RapPhimContext _context;

        public NotificationController(RapPhimContext context)
        {
            _context = context;
        }
        [HttpPost]
        public IActionResult MarkAsRead(int notificationId)
        {
            var notification = _context.Notifications.Find(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                _context.SaveChanges();
            }
            return Ok();
        }

    }
}
