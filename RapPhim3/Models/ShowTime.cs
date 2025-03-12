using System;
using System.Collections.Generic;

namespace RapPhim3.Models;

public partial class ShowTime
{
    public int Id { get; set; }

    public int MovieId { get; set; }

    public int RoomId { get; set; }

    public DateOnly ShowDate { get; set; }

    public TimeOnly ShowTime1 { get; set; }

    public virtual Movie Movie { get; set; } = null!;

    public virtual Room Room { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
