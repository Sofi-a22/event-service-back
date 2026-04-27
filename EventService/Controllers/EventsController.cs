using Microsoft.AspNetCore.Mvc;
using EventService.Core.Models;
using EventService.Core.DTOs;
using EventService.Core.Interfaces;

namespace EventService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EventsController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly IPurchaseService _purchaseService;

    public EventsController(IUnitOfWork uow, IPurchaseService purchaseService)
    {
        _uow = uow;
        _purchaseService = purchaseService;
    }

    // GET: api/events?page=1&pageSize=10
    [HttpGet]
    public async Task<ActionResult> GetEvents(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var events = await _uow.Evenements.GetAllAsync(page, pageSize);
        var total = await _uow.Evenements.GetTotalCountAsync();

        return Ok(new
        {
            data = events,
            page,
            pageSize,
            total,
            totalPages = (int)Math.Ceiling((double)total / pageSize)
        });
    }

    // GET: api/events/5
    [HttpGet("{id}")]
    public async Task<ActionResult> GetEvent(int id)
    {
        var evenement = await _uow.Evenements.GetByIdAsync(id);

        if (evenement == null)
            return NotFound(new { message = $"Event with ID {id} not found" });

        return Ok(evenement);
    }

    // GET: api/events/organizer/{organisateurId}
    [HttpGet("organizer/{organisateurId}")]
    public async Task<ActionResult> GetEventsByOrganizer(int organisateurId)
    {
        var events = await _uow.Evenements.GetByOrganisateurAsync(organisateurId);
        return Ok(events);
    }

    // POST: api/events
    [HttpPost]
    public async Task<ActionResult> CreateEvent(CreateEventDto createDto)
    {
        var evenement = new Evenement
        {
            Titre = createDto.Titre,
            Description = createDto.Description,
            StartDate = createDto.StartDate,
            EndDate = createDto.EndDate,
            TypeEvent = createDto.TypeEvent,
            Categorie = createDto.Categorie,
            Disponibilite = createDto.Disponibilite ?? "Draft",
            Capacite = createDto.Capacite,
            PlacesRestantes = createDto.Capacite,
            LienPartage = createDto.LienPartage ?? GenerateLienPartage(),
            OrganisateurId = createDto.OrganisateurId,
            LocalisationId = createDto.LocalisationId,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _uow.Evenements.AddAsync(evenement);
        await _uow.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEvent), new { id = evenement.Id }, evenement);
    }

    // PUT: api/events/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEvent(int id, UpdateEventDto updateDto)
    {
        if (id != updateDto.Id)
            return BadRequest(new { message = "ID mismatch" });

        var existingEvent = await _uow.Evenements.GetByIdAsync(id);

        if (existingEvent == null)
            return NotFound(new { message = $"Event with ID {id} not found" });

        // ✅ Owner check
        if (existingEvent.OrganisateurId != updateDto.OrganisateurId)
            return StatusCode(403, new { message = "You are not the owner of this event" });

        // ✅ Guard: new capacity cannot be less than tickets already sold
        var soldCount = existingEvent.Capacite - existingEvent.PlacesRestantes;
        if (updateDto.Capacite < soldCount)
            return BadRequest(new
            {
                message = $"New capacity ({updateDto.Capacite}) cannot be less than tickets already sold ({soldCount})"
            });

        existingEvent.Titre = updateDto.Titre;
        existingEvent.Description = updateDto.Description;
        existingEvent.StartDate = updateDto.StartDate;
        existingEvent.EndDate = updateDto.EndDate;
        existingEvent.TypeEvent = updateDto.TypeEvent;
        existingEvent.Categorie = updateDto.Categorie;
        existingEvent.Disponibilite = updateDto.Disponibilite;
        existingEvent.Capacite = updateDto.Capacite;
        existingEvent.PlacesRestantes = updateDto.Capacite - soldCount;
        existingEvent.LocalisationId = updateDto.LocalisationId;
        existingEvent.UpdatedAt = DateTime.UtcNow;

        _uow.Evenements.Update(existingEvent);
        await _uow.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/events/5 (soft delete)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvent(int id, [FromQuery] int organisateurId)
    {
        var evenement = await _uow.Evenements.GetByIdAsync(id);

        if (evenement == null)
            return NotFound(new { message = $"Event with ID {id} not found" });

        // ✅ Owner check
        if (evenement.OrganisateurId != organisateurId)
            return StatusCode(403, new { message = "You are not the owner of this event" });

        // ✅ Guard: cannot delete if sold/reserved tickets exist
        var hasSoldTickets = evenement.BilletTypes
            .SelectMany(bt => bt.Billets)
            .Any(b => b.Statut == "Confirme" || b.Statut == "Reserve");

        if (hasSoldTickets)
            return BadRequest(new { message = "Cannot delete event with sold or reserved tickets" });

        _uow.Evenements.SoftDelete(evenement);
        await _uow.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/events/{id}/purchase
    [HttpPost("{id}/purchase")]
    public async Task<IActionResult> PurchaseTickets(int id, [FromBody] PurchaseRequestDto request)
    {
        var (success, error, response) = await _purchaseService.PurchaseTicketsAsync(id, request);

        if (!success)
            return BadRequest(new { message = error });

        return Ok(response);
    }

    // GET: api/events/search
    [HttpGet("search")]
    public async Task<ActionResult> SearchEvents(
        [FromQuery] string? q,
        [FromQuery] string? categorie,
        [FromQuery] string? typeEvent,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var events = await _uow.Evenements.SearchAsync(q, categorie, typeEvent, startDate, endDate, page, pageSize);
        var total = await _uow.Evenements.GetSearchCountAsync(q, categorie, typeEvent, startDate, endDate);

        return Ok(new
        {
            data = events,
            page,
            pageSize,
            total,
            totalPages = (int)Math.Ceiling((double)total / pageSize)
        });
    }

    // GET: api/events/categories
    [HttpGet("categories")]
    public async Task<ActionResult> GetCategories()
    {
        var categories = await _uow.Evenements.GetCategoriesAsync();
        return Ok(categories);
    }

    // GET: api/events/upcoming
    [HttpGet("upcoming")]
    public async Task<ActionResult> GetUpcomingEvents()
    {
        var events = await _uow.Evenements.GetUpcomingAsync(10);
        return Ok(events);
    }

    // GET: api/events/free
    [HttpGet("free")]
    public async Task<ActionResult> GetFreeEvents()
    {
        var events = await _uow.Evenements.GetFreeAsync();
        return Ok(events);
    }

    // ── Private helpers ──────────────────────────────────────────────

    private static string GenerateLienPartage()
    {
        return Guid.NewGuid().ToString("N")[..8];
    }
}