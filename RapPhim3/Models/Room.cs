using System;
using System.Collections.Generic;

namespace RapPhim3.Models;

public partial class Room
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();

    public virtual ICollection<ShowTime> ShowTimes { get; set; } = new List<ShowTime>();
}
