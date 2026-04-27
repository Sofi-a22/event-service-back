using Microsoft.AspNetCore.Mvc;
using EventService.Core.Interfaces;
using EventService.Core.DTOs;

namespace EventService.Controllers;

[Route("api")]
[ApiController]
public class StatsController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public StatsController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    // GET: api/organizer/{id}/stats
    [HttpGet("organizer/{organisateurId}/stats")]
    public async Task<ActionResult> GetOrganizerStats(int organisateurId)
    {
        var evenements = await _uow.Evenements.GetByOrganisateurAsync(organisateurId);
        var evenementList = evenements.ToList();

        if (!evenementList.Any())
            return Ok(new OrganizerStatsDto());

        var stats = new OrganizerStatsDto
        {
            TotalEvenements = evenementList.Count,
            EvenementsActifs = evenementList.Count(e => e.StartDate >= DateTime.UtcNow),
            Evenements = evenementList.Select(e =>
            {
                var billetsVendus = e.BilletTypes
                    .SelectMany(bt => bt.Billets)
                    .Count(b => b.Statut == "Confirme");

                var revenue = e.BilletTypes
                    .SelectMany(bt => bt.Billets
                        .Where(b => b.Statut == "Confirme")
                        .Select(b => bt.Prix))
                    .Sum();

                return new EvenementStatDto
                {
                    Id = e.Id,
                    Titre = e.Titre,
                    StartDate = e.StartDate,
                    Capacite = e.Capacite,
                    PlacesRestantes = e.PlacesRestantes,
                    BilletsVendus = billetsVendus,
                    Revenue = revenue,
                    TauxRemplissage = e.Capacite > 0
                        ? Math.Round((double)billetsVendus / e.Capacite * 100, 1)
                        : 0
                };
            }).ToList()
        };

        stats.TotalBilletsVendus = stats.Evenements.Sum(e => e.BilletsVendus);
        stats.RevenueTotal = stats.Evenements.Sum(e => e.Revenue);

        return Ok(stats);
    }

    // GET: api/events/{id}/stats
    [HttpGet("events/{id}/stats")]
    public async Task<ActionResult> GetEventStats(int id, [FromQuery] int organisateurId)
    {
        var evenement = await _uow.Evenements.GetByIdAsync(id);

        if (evenement == null)
            return NotFound(new { message = $"Event with ID {id} not found" });

        if (evenement.OrganisateurId != organisateurId)
            return StatusCode(403, new { message = "You are not the owner of this event" });

        var billetsVendus = evenement.BilletTypes
            .SelectMany(bt => bt.Billets)
            .Count(b => b.Statut == "Confirme");

        var revenue = evenement.BilletTypes
            .SelectMany(bt => bt.Billets
                .Where(b => b.Statut == "Confirme")
                .Select(b => bt.Prix))
            .Sum();

        var statParType = evenement.BilletTypes.Select(bt => new
        {
            bt.Id,
            bt.Nom,
            bt.Prix,
            bt.Quantite,
            Vendus = bt.Billets.Count(b => b.Statut == "Confirme"),
            Reserves = bt.Billets.Count(b => b.Statut == "Reserve"),
            Disponibles = bt.Billets.Count(b => b.Statut == "Disponible"),
            Revenue = bt.Billets.Count(b => b.Statut == "Confirme") * bt.Prix
        });

        return Ok(new
        {
            evenementId = evenement.Id,
            titre = evenement.Titre,
            capacite = evenement.Capacite,
            placesRestantes = evenement.PlacesRestantes,
            billetsVendus,
            revenue,
            tauxRemplissage = evenement.Capacite > 0
                ? Math.Round((double)billetsVendus / evenement.Capacite * 100, 1)
                : 0,
            statParType
        });
    }

    // GET: api/events/{id}/participants
    [HttpGet("events/{id}/participants")]
    public async Task<ActionResult> GetParticipants(int id, [FromQuery] int organisateurId)
    {
        var evenement = await _uow.Evenements.GetByIdAsync(id);

        if (evenement == null)
            return NotFound(new { message = $"Event with ID {id} not found" });

        if (evenement.OrganisateurId != organisateurId)
            return StatusCode(403, new { message = "You are not the owner of this event" });

        var participants = evenement.BilletTypes
            .SelectMany(bt => bt.Billets
                .Where(b => b.Statut == "Confirme" || b.Statut == "Reserve")
                .Select(b => new
                {
                    billetId = b.Id,
                    code = b.Code,
                    statut = b.Statut,
                    visiteurId = b.VisiteurId,
                    billetType = bt.Nom,
                    prix = bt.Prix,
                    dateReservation = b.DateReservation
                }))
            .OrderBy(p => p.dateReservation)
            .ToList();

        return Ok(new
        {
            evenementId = id,
            totalParticipants = participants.Count,
            participants
        });
    }
}