using System;
using System.Collections.Generic;

namespace RapPhim3.Models;

public partial class SaleTransaction
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime? TransactionDate { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
