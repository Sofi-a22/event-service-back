using Microsoft.EntityFrameworkCore;
using EventService.Core.Models;

namespace EventService.Infrastructure.Data;

public class EventDbContext : DbContext
{
    public EventDbContext(DbContextOptions<EventDbContext> options)
        : base(options)
    {
    }

    public DbSet<Evenement> Evenements { get; set; }
    public DbSet<BilletType> BilletTypes { get; set; }
    public DbSet<Billet> Billets { get; set; }
    public DbSet<Localisation> Localisations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Prix precision
        modelBuilder.Entity<BilletType>()
            .Property(b => b.Prix)
            .HasPrecision(18, 2);

        // Relations
        modelBuilder.Entity<BilletType>()
            .HasMany(bt => bt.Billets)
            .WithOne(b => b.BilletType)
            .HasForeignKey(b => b.BilletTypeId);

        modelBuilder.Entity<Evenement>()
            .HasMany(e => e.BilletTypes)
            .WithOne(bt => bt.Evenement)
            .HasForeignKey(bt => bt.EvenementId);

        modelBuilder.Entity<Evenement>()
            .HasOne(e => e.Localisation)
            .WithMany(l => l.Evenements)
            .HasForeignKey(e => e.LocalisationId);
    }
}