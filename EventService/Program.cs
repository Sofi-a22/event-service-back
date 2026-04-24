using EventService.Core.Models;
using EventService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS for React
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder =>
        {
            builder.WithOrigins("http://localhost:5173")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});

// ✅ UTILISEZ InMemory DATABASE POUR LES TESTS (pas besoin d'Oracle)
builder.Services.AddDbContext<EventDbContext>(options =>
    options.UseInMemoryDatabase("EventSphereTestDb"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();

// ✅ SEED DATA - Créer des données de test
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<EventDbContext>();

    // Vérifier si la base est vide
    if (!context.Evenements.Any())
    {
        Console.WriteLine("📦 Seed data: Creating test events...");

        // 1. Créer un événement de test
        var evenement = new Evenement
        {
            Titre = "Summer Music Festival",
            Description = "Join us for an unforgettable day of music with top artists from around the world!",
            StartDate = new DateTime(2026, 7, 15, 16, 0, 0),
            EndDate = new DateTime(2026, 7, 15, 23, 0, 0),
            TypeEvent = "ONSITE",
            Categorie = "Music",
            Disponibilite = "Disponible",
            Capacite = 500,
            PlacesRestantes = 500,
            LienPartage = "summer-fest-2026",
            OrganisateurId = 1,
            CreatedAt = DateTime.UtcNow
        };
        context.Evenements.Add(evenement);
        context.SaveChanges();  // Pas besoin de await en synchrone

        // 2. Créer les types de billets VIP
        var vipType = new BilletType
        {
            Nom = "VIP",
            Prix = 150,
            Quantite = 50,
            Vendu = 0,
            EvenementId = evenement.Id,
            Billets = new List<Billet>()
        };

        // Générer les 50 billets VIP
        for (int i = 1; i <= 50; i++)
        {
            vipType.Billets.Add(new Billet
            {
                Code = $"VIP-{i:D3}",
                Statut = "Disponible",
                BilletType = vipType
            });
        }
        context.BilletTypes.Add(vipType);

        // 3. Créer les types de billets Normal
        var normalType = new BilletType
        {
            Nom = "Normal",
            Prix = 45,
            Quantite = 400,
            Vendu = 0,
            EvenementId = evenement.Id,
            Billets = new List<Billet>()
        };

        // Générer les 400 billets Normal
        for (int i = 1; i <= 400; i++)
        {
            normalType.Billets.Add(new Billet
            {
                Code = $"NORMAL-{i:D3}",
                Statut = "Disponible",
                BilletType = normalType
            });
        }
        context.BilletTypes.Add(normalType);

        // 4. Créer un deuxième événement
        var evenement2 = new Evenement
        {
            Titre = "Tech Conference 2026",
            Description = "The biggest tech conference of the year",
            StartDate = new DateTime(2026, 9, 15, 9, 0, 0),
            EndDate = new DateTime(2026, 9, 17, 18, 0, 0),
            TypeEvent = "HYBRID",
            Categorie = "Conference",
            Disponibilite = "Disponible",
            Capacite = 1000,
            PlacesRestantes = 1000,
            LienPartage = "tech-conf-2026",
            OrganisateurId = 2,
            CreatedAt = DateTime.UtcNow
        };
        context.Evenements.Add(evenement2);

        // Types de billets pour Tech Conference
        var conferenceVip = new BilletType
        {
            Nom = "VIP",
            Prix = 299,
            Quantite = 100,
            Vendu = 0,
            EvenementId = evenement2.Id,
            Billets = new List<Billet>()
        };

        for (int i = 1; i <= 100; i++)
        {
            conferenceVip.Billets.Add(new Billet
            {
                Code = $"TECH-VIP-{i:D3}",
                Statut = "Disponible",
                BilletType = conferenceVip
            });
        }
        context.BilletTypes.Add(conferenceVip);

        var conferenceNormal = new BilletType
        {
            Nom = "Normal",
            Prix = 149,
            Quantite = 500,
            Vendu = 0,
            EvenementId = evenement2.Id,
            Billets = new List<Billet>()
        };

        for (int i = 1; i <= 500; i++)
        {
            conferenceNormal.Billets.Add(new Billet
            {
                Code = $"TECH-NORMAL-{i:D3}",
                Statut = "Disponible",
                BilletType = conferenceNormal
            });
        }
        context.BilletTypes.Add(conferenceNormal);

        context.SaveChanges();

        Console.WriteLine($"✅ Seed data completed: {context.Evenements.Count()} events, {context.BilletTypes.Count()} ticket types, {context.Billets.Count()} individual tickets");
    }
}

app.Run();