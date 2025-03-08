using System;
using System.Collections.Generic;

namespace RapPhim3.Models;

public partial class Actor
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Biography { get; set; }

    public string? ImageUrl { get; set; }

    public virtual ICollection<Movie> Movies { get; set; } = new List<Movie>();
}
