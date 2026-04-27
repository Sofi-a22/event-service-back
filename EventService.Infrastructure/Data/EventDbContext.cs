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

        // ── Evenement ────────────────────────────────────────────────
        modelBuilder.Entity<Evenement>(entity =>
        {
            entity.ToTable("EVENEMENTS");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                  .ValueGeneratedOnAdd();

            entity.Property(e => e.Titre)
                  .IsRequired()
                  .HasMaxLength(200)
                  .HasColumnName("TITRE");

            entity.Property(e => e.Description)
                  .HasColumnName("DESCRIPTION");

            entity.Property(e => e.StartDate)
                  .IsRequired()
                  .HasColumnName("START_DATE");

            entity.Property(e => e.EndDate)
                  .IsRequired()
                  .HasColumnName("END_DATE");

            entity.Property(e => e.TypeEvent)
                  .HasMaxLength(50)
                  .HasColumnName("TYPE_EVENT");

            entity.Property(e => e.Categorie)
                  .HasMaxLength(50)
                  .HasColumnName("CATEGORIE");

            entity.Property(e => e.Disponibilite)
                  .HasMaxLength(50)
                  .HasColumnName("DISPONIBILITE");

            entity.Property(e => e.Capacite)
                  .IsRequired()
                  .HasColumnName("CAPACITE");

            entity.Property(e => e.PlacesRestantes)
                  .IsRequired()
                  .HasColumnName("PLACES_RESTANTES");

            entity.Property(e => e.LienPartage)
                  .HasMaxLength(255)
                  .HasColumnName("LIEN_PARTAGE");

            entity.Property(e => e.OrganisateurId)
                  .IsRequired()
                  .HasColumnName("ORGANISATEUR_ID");

            entity.Property(e => e.LocalisationId)
                  .HasColumnName("LOCALISATION_ID");

            entity.Property(e => e.CreatedAt)
                  .HasColumnName("CREATED_AT");

            entity.Property(e => e.UpdatedAt)
                  .HasColumnName("UPDATED_AT");
            entity.Property(e => e.IsDeleted)
      .HasColumnName("IS_DELETED")
      .HasColumnType("NUMBER(1)")
      .HasConversion<int>();
            // Relations
            entity.HasMany(e => e.BilletTypes)
                  .WithOne(bt => bt.Evenement)
                  .HasForeignKey(bt => bt.EvenementId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Localisation)
                  .WithMany(l => l.Evenements)
                  .HasForeignKey(e => e.LocalisationId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // ── BilletType ───────────────────────────────────────────────
        modelBuilder.Entity<BilletType>(entity =>
        {
            entity.ToTable("BILLET_TYPES");

            entity.HasKey(bt => bt.Id);
            entity.Property(bt => bt.Id)
                  .ValueGeneratedOnAdd();

            entity.Property(bt => bt.Nom)
                  .IsRequired()
                  .HasMaxLength(100)
                  .HasColumnName("NOM");

            entity.Property(bt => bt.Prix)
                  .IsRequired()
                  .HasPrecision(10, 2)
                  .HasColumnName("PRIX");

            entity.Property(bt => bt.Quantite)
                  .IsRequired()
                  .HasColumnName("QUANTITE");

            entity.Property(bt => bt.Vendu)
                  .HasDefaultValue(0)
                  .HasColumnName("VENDU");

            entity.Property(bt => bt.EvenementId)
                  .IsRequired()
                  .HasColumnName("EVENEMENT_ID");

            // Relations
            entity.HasMany(bt => bt.Billets)
                  .WithOne(b => b.BilletType)
                  .HasForeignKey(b => b.BilletTypeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ── Billet ───────────────────────────────────────────────────
        modelBuilder.Entity<Billet>(entity =>
        {
            entity.ToTable("BILLETS");

            entity.HasKey(b => b.Id);
            entity.Property(b => b.Id)
                  .ValueGeneratedOnAdd();

            entity.Property(b => b.Code)
                  .IsRequired()
                  .HasMaxLength(50)
                  .HasColumnName("CODE");

            entity.HasIndex(b => b.Code)
                  .IsUnique();

            entity.Property(b => b.Statut)
                  .IsRequired()
                  .HasMaxLength(20)
                  .HasDefaultValue("Disponible")
                  .HasColumnName("STATUT");

            entity.Property(b => b.DateReservation)
                  .HasColumnName("DATE_RESERVATION");

            entity.Property(b => b.DateValidation)
                  .HasColumnName("DATE_VALIDATION");

            entity.Property(b => b.VisiteurId)
                  .HasColumnName("VISITEUR_ID");

            entity.Property(b => b.BilletTypeId)
                  .IsRequired()
                  .HasColumnName("BILLET_TYPE_ID");

            entity.Property(b => b.PaymentTransactionId)
                  .HasMaxLength(100)
                  .IsRequired(false)
                  .HasColumnName("PAYMENT_TRANSACTION_ID");
        });

        // ── Localisation ─────────────────────────────────────────────
        modelBuilder.Entity<Localisation>(entity =>
        {
            entity.ToTable("LOCALISATIONS");

            entity.HasKey(l => l.Id);
            entity.Property(l => l.Id)
                  .ValueGeneratedOnAdd();

            entity.Property(l => l.Adresse)
                  .IsRequired()
                  .HasMaxLength(200)
                  .HasColumnName("ADRESSE");

            entity.Property(l => l.Ville)
                  .IsRequired()
                  .HasMaxLength(100)
                  .HasColumnName("VILLE");

            entity.Property(l => l.CodePostal)
                  .IsRequired()
                  .HasMaxLength(20)
                  .HasColumnName("CODE_POSTAL");

            entity.Property(l => l.Latitude)
                  .HasPrecision(10, 8)
                  .HasColumnName("LATITUDE");

            entity.Property(l => l.Longitude)
                  .HasPrecision(11, 8)
                  .HasColumnName("LONGITUDE");
        });
    }
}