using System;
using System.Collections.Generic;

namespace RapPhim3.Models;

public partial class AdminContactFormEntry
{
    public int Id { get; set; }

    public int AdminId { get; set; }

    public string Message { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual Admin Admin { get; set; } = null!;
}
