using RapPhim3.Models;

namespace RapPhim3.ViewModel
{
    public class BuyTicketViewModel
    {
        public Movie Movie { get; set; }
        public List<DateOnly> ShowDates { get; set; }

    }
}
