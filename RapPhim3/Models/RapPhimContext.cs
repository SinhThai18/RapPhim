using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RapPhim3.Models;

public partial class RapPhimContext : DbContext
{
    public RapPhimContext()
    {
    }

    public RapPhimContext(DbContextOptions<RapPhimContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Actor> Actors { get; set; }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<AdminContactFormEntry> AdminContactFormEntries { get; set; }

    public virtual DbSet<Cinema> Cinemas { get; set; }

    public virtual DbSet<ContactFormEntry> ContactFormEntries { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Director> Directors { get; set; }

    public virtual DbSet<FaqEntry> FaqEntries { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Hall> Halls { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<MovieComment> MovieComments { get; set; }

    public virtual DbSet<MovieProjection> MovieProjections { get; set; }

    public virtual DbSet<MovieSchedule> MovieSchedules { get; set; }

    public virtual DbSet<Privacy> Privacies { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<SaleTransaction> SaleTransactions { get; set; }

    public virtual DbSet<Seat> Seats { get; set; }

    public virtual DbSet<Setting> Settings { get; set; }

    public virtual DbSet<StarRating> StarRatings { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=RapPhim;User Id=sa;Password=160203;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Actor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Actors__3214EC0716153AED");

            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Admins__3214EC0711D86D64");

            entity.HasIndex(e => e.UserId, "UQ__Admins__1788CC4D988EDAFD").IsUnique();

            entity.HasOne(d => d.User).WithOne(p => p.Admin)
                .HasForeignKey<Admin>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Admins__UserId__4F7CD00D");
        });

        modelBuilder.Entity<AdminContactFormEntry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AdminCon__3214EC0788B031E7");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Admin).WithMany(p => p.AdminContactFormEntries)
                .HasForeignKey(d => d.AdminId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AdminCont__Admin__0D7A0286");
        });

        modelBuilder.Entity<Cinema>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cinemas__3214EC076A922B0B");

            entity.Property(e => e.ContactInfo).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<ContactFormEntry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ContactF__3214EC076C062F5C");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Country__3214EC075FA5C9FF");

            entity.ToTable("Country");

            entity.HasIndex(e => e.Name, "UQ__Country__737584F6AF145B21").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Director>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Director__3214EC0744146C65");

            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<FaqEntry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FaqEntri__3214EC0767510157");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Genres__3214EC07B1B3ECC4");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Hall>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Halls__3214EC0776DFE08E");

            entity.Property(e => e.Category).HasMaxLength(100);

            entity.HasOne(d => d.Cinema).WithMany(p => p.Halls)
                .HasForeignKey(d => d.CinemaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Halls__CinemaId__619B8048");
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Movies__3214EC07DBFB65A5");

            entity.Property(e => e.LandscapeImage).HasMaxLength(500);
            entity.Property(e => e.PortraitImage).HasMaxLength(500);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.TrailerUrl).HasMaxLength(500);

            entity.HasOne(d => d.Country).WithMany(p => p.Movies)
                .HasForeignKey(d => d.CountryId)
                .HasConstraintName("FK__Movies__CountryI__17036CC0");

            entity.HasOne(d => d.Director).WithMany(p => p.Movies)
                .HasForeignKey(d => d.DirectorId)
                .HasConstraintName("FK__Movies__Director__5629CD9C");

            entity.HasOne(d => d.Genre).WithMany(p => p.Movies)
                .HasForeignKey(d => d.GenreId)
                .HasConstraintName("FK__Movies__GenreId__571DF1D5");

            entity.HasMany(d => d.Actors).WithMany(p => p.Movies)
                .UsingEntity<Dictionary<string, object>>(
                    "MovieActor",
                    r => r.HasOne<Actor>().WithMany()
                        .HasForeignKey("ActorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MovieActo__Actor__5CD6CB2B"),
                    l => l.HasOne<Movie>().WithMany()
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MovieActo__Movie__5BE2A6F2"),
                    j =>
                    {
                        j.HasKey("MovieId", "ActorId").HasName("PK__MovieAct__EEA9AABE8ED4BECA");
                        j.ToTable("MovieActors");
                    });
        });

        modelBuilder.Entity<MovieComment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MovieCom__3214EC074CAB7FD8");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Movie).WithMany(p => p.MovieComments)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MovieComm__Movie__02FC7413");

            entity.HasOne(d => d.User).WithMany(p => p.MovieComments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MovieComm__UserI__02084FDA");
        });

        modelBuilder.Entity<MovieProjection>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MoviePro__3214EC07405A082D");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Hall).WithMany(p => p.MovieProjections)
                .HasForeignKey(d => d.HallId)
                .HasConstraintName("FK__MovieProj__HallI__6A30C649");

            entity.HasOne(d => d.Schedule).WithMany(p => p.MovieProjections)
                .HasForeignKey(d => d.ScheduleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MovieProj__Sched__693CA210");
        });

        modelBuilder.Entity<MovieSchedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MovieSch__3214EC07742CB24F");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Movie).WithMany(p => p.MovieSchedules)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MovieSche__Movie__656C112C");
        });

        modelBuilder.Entity<Privacy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Privacie__3214EC0776F4DCE6");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reviews__3214EC07A2DB6BEE");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Movie).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__MovieId__7E37BEF6");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__UserId__7D439ABD");
        });

        modelBuilder.Entity<SaleTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SaleTran__3214EC0777A4E159");

            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TransactionDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.SaleTransactions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SaleTrans__UserI__787EE5A0");
        });

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Seats__3214EC0738975D65");

            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.SeatNumber).HasMaxLength(10);

            entity.HasOne(d => d.Hall).WithMany(p => p.Seats)
                .HasForeignKey(d => d.HallId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Seats__HallId__6E01572D");
        });

        modelBuilder.Entity<Setting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Settings__3214EC07E407DDF0");

            entity.Property(e => e.Key).HasMaxLength(255);
        });

        modelBuilder.Entity<StarRating>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StarRati__3214EC07010580A2");

            entity.HasOne(d => d.Movie).WithMany(p => p.StarRatings)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StarRatin__Movie__1332DBDC");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tickets__3214EC07051559C4");

            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Projection).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.ProjectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tickets__Project__71D1E811");

            entity.HasOne(d => d.Seat).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.SeatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tickets__SeatId__72C60C4A");

            entity.HasOne(d => d.User).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tickets__UserId__70DDC3D8");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC079B9B26CE");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105348839C435").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Role).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
