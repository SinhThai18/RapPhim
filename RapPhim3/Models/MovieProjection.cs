using System;
using System.Collections.Generic;

namespace RapPhim3.Models;

public partial class MovieProjection
{
    public int Id { get; set; }

    public int ScheduleId { get; set; }

    public DateOnly ShowDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public int? Duration { get; set; }

    public int? HallId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Hall? Hall { get; set; }

    public virtual MovieSchedule Schedule { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
