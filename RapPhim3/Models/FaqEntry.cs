using System;
using System.Collections.Generic;

namespace RapPhim3.Models;

public partial class FaqEntry
{
    public int Id { get; set; }

    public string Question { get; set; } = null!;

    public string Answer { get; set; } = null!;
}
