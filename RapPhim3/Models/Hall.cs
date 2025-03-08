using System;
using System.Collections.Generic;

namespace RapPhim3.Models;

public partial class Hall
{
    public int Id { get; set; }

    public int CinemaId { get; set; }

    public string? Category { get; set; }

    public int? Capacity { get; set; }

    public virtual Cinema Cinema { get; set; } = null!;

    public virtual ICollection<MovieProjection> MovieProjections { get; set; } = new List<MovieProjection>();

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
}
