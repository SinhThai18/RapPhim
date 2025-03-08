using System;
using System.Collections.Generic;

namespace RapPhim3.Models;

public partial class MovieSchedule
{
    public int Id { get; set; }

    public int MovieId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Movie Movie { get; set; } = null!;

    public virtual ICollection<MovieProjection> MovieProjections { get; set; } = new List<MovieProjection>();
}
