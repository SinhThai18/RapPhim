using RapPhim3.Models;

namespace RapPhim3.ViewModel
{
    public class ProfileViewModel
    {
        public User User { get; set; }
        public List<Ticket> PaidTickets { get; set; }

        public bool HasUnreadNotifications { get; set; }

        public List<Notification> Notifications { get; set; }
    }
}
