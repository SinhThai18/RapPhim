using System;
using System.Collections.Generic;

namespace RapPhim3.Models;

public partial class Seat
{
    public int Id { get; set; }

    public int HallId { get; set; }

    public string SeatNumber { get; set; } = null!;

    public bool? IsAvailable { get; set; }

    public virtual Hall Hall { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
