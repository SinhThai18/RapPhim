using System;
using System.Collections.Generic;

namespace RapPhim3.Models;

public partial class Setting
{
    public int Id { get; set; }

    public string Key { get; set; } = null!;

    public string? Value { get; set; }
}
