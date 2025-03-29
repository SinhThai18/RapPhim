namespace RapPhim3.ViewModel
{
    public class CustomerViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public decimal TotalSpent { get; set; } // Tổng tiền khách đã mua
    }
}
