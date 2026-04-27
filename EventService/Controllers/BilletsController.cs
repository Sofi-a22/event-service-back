using Microsoft.AspNetCore.Mvc;
using EventService.Core.Interfaces;
using EventService.Core.DTOs;

namespace EventService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BilletsController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public BilletsController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    // GET: api/billets/user/{userId}
    [HttpGet("user/{userId}")]
    public async Task<ActionResult> GetBilletsByUser(int userId)
    {
        var billets = await _uow.Billets.GetByVisiteurAsync(userId);

        var result = billets.Select(b => new BilletResponseDto
        {
            Id = b.Id,
            Code = b.Code,
            Statut = b.Statut,
            DateReservation = b.DateReservation,
            BilletTypeNom = b.BilletType.Nom,
            Prix = b.BilletType.Prix,
            EvenementId = b.BilletType.EvenementId,
            EvenementTitre = b.BilletType.Evenement.Titre,
            EvenementStartDate = b.BilletType.Evenement.StartDate,
            EvenementEndDate = b.BilletType.Evenement.EndDate,
            EvenementType = b.BilletType.Evenement.TypeEvent
        });

        return Ok(result);
    }

    // GET: api/billets/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult> GetBillet(int id)
    {
        var billet = await _uow.Billets.GetByIdAsync(id);

        if (billet == null)
            return NotFound(new { message = $"Ticket with ID {id} not found" });

        var result = new BilletResponseDto
        {
            Id = billet.Id,
            Code = billet.Code,
            Statut = billet.Statut,
            DateReservation = billet.DateReservation,
            BilletTypeNom = billet.BilletType.Nom,
            Prix = billet.BilletType.Prix,
            EvenementId = billet.BilletType.EvenementId,
            EvenementTitre = billet.BilletType.Evenement.Titre,
            EvenementStartDate = billet.BilletType.Evenement.StartDate,
            EvenementEndDate = billet.BilletType.Evenement.EndDate,
            EvenementType = billet.BilletType.Evenement.TypeEvent
        };

        return Ok(result);
    }
}