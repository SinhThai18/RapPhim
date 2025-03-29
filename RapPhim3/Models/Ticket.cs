using System;
using System.Collections.Generic;

namespace RapPhim3.Models;

public partial class Ticket
{
    public int Id { get; set; }

    public int ShowTimeId { get; set; }

    public int SeatId { get; set; }

    public int UserId { get; set; }

    public DateTime BookingTime { get; set; } = DateTime.Now;

    public decimal Price { get; set; }

    public string? PaymentStatus { get; set; } = "UnPaid";

    public virtual Seat Seat { get; set; } = null!;

    public virtual ShowTime ShowTime { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
