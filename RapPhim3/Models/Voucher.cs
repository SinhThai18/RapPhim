using System;
using System.Collections.Generic;

namespace RapPhim3.Models;

public partial class Voucher
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public decimal DiscountAmount { get; set; }

    public bool IsActive { get; set; }

    public int? UserId { get; set; }

    public virtual User? User { get; set; }
}
