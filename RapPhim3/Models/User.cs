using System;
using System.Collections.Generic;

namespace RapPhim3.Models;

public partial class User
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string Role { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual Admin? Admin { get; set; }

    public virtual ICollection<MovieComment> MovieComments { get; set; } = new List<MovieComment>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<SaleTransaction> SaleTransactions { get; set; } = new List<SaleTransaction>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
