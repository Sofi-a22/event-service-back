using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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

    // GET: api/billets/event/{eventId}
    // Returns all purchased tickets for an event (for organizer participants view)
    [HttpGet("event/{eventId}")]
    [Authorize]
    public async Task<ActionResult> GetBilletsByEvent(int eventId)
    {
        var billets = await _uow.Billets.GetByEventAsync(eventId);

        var result = billets.Select(b => new
        {
            id = b.Id,
            code = b.Code,
            statut = b.Statut,
            dateReservation = b.DateReservation,
            visiteurId = b.VisiteurId,
            billetTypeNom = b.BilletType.Nom,
            prix = b.BilletType.Prix,
            evenementId = b.BilletType.EvenementId,
        });

        return Ok(result);
    }

    // POST: api/billets/validate
    // Used by organizer's QR scanner to mark a ticket as used
    [HttpPost("validate")]
    [Authorize]
    public async Task<ActionResult> ValidateTicket([FromBody] ValidateTicketDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Code))
            return BadRequest(new { message = "Ticket code is required" });

        var billet = await _uow.Billets.GetByCodeAsync(dto.Code.Trim().ToUpper());

        if (billet == null)
            return NotFound(new { message = "Ticket not found. Invalid code." });

        // Verify the ticket belongs to the specified event
        if (dto.EventId.HasValue && billet.BilletType.EvenementId != dto.EventId.Value)
            return BadRequest(new { message = "This ticket does not belong to this event." });

        // Check current status
        switch (billet.Statut)
        {
            case "Utilise":
                return Conflict(new
                {
                    message = "This ticket has already been used.",
                    validatedAt = billet.DateValidation,
                    ticketType = billet.BilletType.Nom
                });

            case "Annule":
                return BadRequest(new { message = "This ticket has been cancelled." });

            case "Disponible":
                return BadRequest(new { message = "This ticket was never purchased." });

            case "Reserve":
            case "Confirme":
                // ✅ Valid — mark as used
                billet.Statut = "Utilise";
                billet.DateValidation = DateTime.UtcNow;
                _uow.Billets.Update(billet);
                await _uow.SaveChangesAsync();

                return Ok(new
                {
                    message = "Ticket validated successfully!",
                    ticketType = billet.BilletType.Nom,
                    visitorId = billet.VisiteurId,
                    eventTitle = billet.BilletType.Evenement.Titre,
                    validatedAt = billet.DateValidation
                });

            default:
                return BadRequest(new { message = $"Unknown ticket status: {billet.Statut}" });
        }
    }
}