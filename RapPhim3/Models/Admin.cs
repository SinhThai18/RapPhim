using System;
using System.Collections.Generic;

namespace RapPhim3.Models;

public partial class Admin
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? Permissions { get; set; }

    public virtual ICollection<AdminContactFormEntry> AdminContactFormEntries { get; set; } = new List<AdminContactFormEntry>();

    public virtual User User { get; set; } = null!;
}
