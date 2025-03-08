using System;
using System.Collections.Generic;

namespace RapPhim3.Models;

public partial class Cinema
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Location { get; set; }

    public string? ContactInfo { get; set; }

    public virtual ICollection<Hall> Halls { get; set; } = new List<Hall>();
}
