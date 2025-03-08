using System;
using System.Collections.Generic;

namespace RapPhim3.Models;

public partial class StarRating
{
    public int Id { get; set; }

    public int MovieId { get; set; }

    public int? Rating { get; set; }

    public virtual Movie Movie { get; set; } = null!;
}
