using System;
using System.Collections.Generic;

namespace RapPhim3.Models;

public partial class MovieComment
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int MovieId { get; set; }

    public string? Content { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Movie Movie { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
