using System;
using System.Collections.Generic;

namespace RapPhim3.Models;

public partial class Movie
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly? ReleaseDate { get; set; }

    public int? Duration { get; set; }

    public string? LandscapeImage { get; set; }

    public string? PortraitImage { get; set; }

    public int? CountryId { get; set; }

    public string? TrailerUrl { get; set; }

    public virtual Country? Country { get; set; }

    public virtual ICollection<ShowTime> ShowTimes { get; set; } = new List<ShowTime>();

    public virtual ICollection<Actor> Actors { get; set; } = new List<Actor>();

    public virtual ICollection<Director> Directors { get; set; } = new List<Director>();

    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();
}
