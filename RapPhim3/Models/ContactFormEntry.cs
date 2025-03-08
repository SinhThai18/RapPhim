using System;
using System.Collections.Generic;

namespace RapPhim3.Models;

public partial class ContactFormEntry
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Message { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }
}
