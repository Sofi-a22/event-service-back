using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventService.Core.Models;
using EventService.Core.DTOs;
using EventService.Infrastructure.Data;

namespace EventService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BilletTypesController : ControllerBase
{
    private readonly EventDbContext _context;

    public BilletTypesController(EventDbContext context)
    {
        _context = context;
    }
    // GET: api/BilletTypes/event/{eventId}
    [HttpGet("event/{eventId}")]
    public async Task<ActionResult<IEnumerable<BilletType>>> GetBilletTypesByEvent(int eventId)
    {
        var billetTypes = await _context.BilletTypes
            .Where(bt => bt.EvenementId == eventId)
            .ToListAsync();

        return Ok(billetTypes);
    }

    // POST: api/billettypes/event/{eventId}
    [HttpPost("event/{eventId}")]
    public async Task<ActionResult<BilletType>> CreateBilletType(int eventId, [FromBody] CreateBilletTypeDto createDto)
    {
        var evenement = await _context.Evenements.FindAsync(eventId);
        if (evenement == null)
            return NotFound(new { message = $"Event with ID {eventId} not found" });

        var existing = await _context.BilletTypes
            .FirstOrDefaultAsync(bt => bt.EvenementId == eventId && bt.Nom == createDto.Nom);

        if (existing != null)
            return Conflict(new { message = $"Ticket type '{createDto.Nom}' already exists" });

        var billetType = new BilletType
        {
            Nom = createDto.Nom,
            Prix = createDto.Prix,
            Quantite = createDto.Quantite,
            Vendu = 0,
            EvenementId = eventId,
            Billets = new List<Billet>()
        };

        // ✅ Génération automatique des billets individuels
        var prefix = evenement.Titre.Replace(" ", "").Substring(0, Math.Min(6, evenement.Titre.Length)).ToUpper();

        for (int i = 1; i <= createDto.Quantite; i++)
        {
            var code = $"{prefix}-{createDto.Nom.ToUpper()}-{i:D4}";

            var billet = new Billet
            {
                Code = code,
                Statut = "Disponible",
                BilletType = billetType
            };
            billetType.Billets.Add(billet);
        }

        _context.BilletTypes.Add(billetType);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            id = billetType.Id,
            nom = billetType.Nom,
            prix = billetType.Prix,
            quantite = billetType.Quantite,
            billetsGeneres = billetType.Billets.Count,
            message = $"{billetType.Quantite} billets générés automatiquement"
        });
    }
    // PUT: api/BilletTypes/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBilletType(int id, [FromBody] CreateBilletTypeDto updateDto)
    {
        var billetType = await _context.BilletTypes.FindAsync(id);
        if (billetType == null)
            return NotFound(new { message = $"Ticket type with ID {id} not found" });

        billetType.Nom = updateDto.Nom;
        billetType.Prix = updateDto.Prix;
        billetType.Quantite = updateDto.Quantite;

        await _context.SaveChangesAsync();
        return Ok(billetType);
    }

    // DELETE: api/BilletTypes/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBilletType(int id)
    {
        var billetType = await _context.BilletTypes
            .Include(bt => bt.Billets)
            .FirstOrDefaultAsync(bt => bt.Id == id);

        if (billetType == null)
            return NotFound(new { message = $"Ticket type with ID {id} not found" });

        if (billetType.Billets.Any(b => b.Statut != "Disponible"))
            return BadRequest(new { message = "Cannot delete ticket type with tickets already sold or reserved" });

        _context.BilletTypes.Remove(billetType);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}