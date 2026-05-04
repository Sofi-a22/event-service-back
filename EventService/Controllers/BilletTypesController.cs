using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EventService.Core.Models;
using EventService.Core.DTOs;
using EventService.Core.Interfaces;
using EventService.Core.Helpers;

namespace EventService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BilletTypesController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly ITicketCodeGenerator _codeGenerator;

    public BilletTypesController(IUnitOfWork uow, ITicketCodeGenerator codeGenerator)
    {
        _uow = uow;
        _codeGenerator = codeGenerator;
    }

    [AllowAnonymous]
    [HttpGet("event/{eventId}")]
    public async Task<ActionResult> GetBilletTypesByEvent(int eventId)
    {
        var evenement = await _uow.Evenements.GetByIdAsync(eventId);
        if (evenement == null)
            return NotFound(new { message = $"Event with ID {eventId} not found" });

        var billetTypes = await _uow.BilletTypes.GetByEvenementAsync(eventId);

        var result = billetTypes.Select(bt => new
        {
            bt.Id,
            bt.Nom,
            bt.Prix,
            bt.Quantite,
            bt.Vendu,
            Disponibles = bt.Quantite - bt.Vendu
        });

        return Ok(result);
    }

    [Authorize]
    [HttpPost("event/{eventId}")]
    public async Task<ActionResult> CreateBilletType(int eventId, [FromBody] CreateBilletTypeDto createDto)
    {
        if (!ClaimsHelper.IsOrganisateur(User))
            return StatusCode(403, new { message = "Only organisateurs can create ticket types" });

        var evenement = await _uow.Evenements.GetByIdAsync(eventId);
        if (evenement == null)
            return NotFound(new { message = $"Event with ID {eventId} not found" });

        // ✅ Owner check from JWT
        var userId = ClaimsHelper.GetUserId(User);
        if (evenement.OrganisateurId != userId)
            return StatusCode(403, new { message = "You are not the owner of this event" });

        var exists = await _uow.BilletTypes.ExistsAsync(eventId, createDto.Nom);
        if (exists)
            return Conflict(new { message = $"Ticket type '{createDto.Nom}' already exists for this event" });

        var billetType = new BilletType
        {
            Nom = createDto.Nom,
            Prix = createDto.Prix,
            Quantite = createDto.Quantite,
            Vendu = 0,
            EvenementId = eventId,
            Billets = new List<Billet>()
        };

        for (int i = 0; i < createDto.Quantite; i++)
        {
            billetType.Billets.Add(new Billet
            {
                Code = _codeGenerator.Generate(eventId),
                Statut = "Disponible",
                BilletType = billetType
            });
        }

        await _uow.BilletTypes.AddAsync(billetType);
        await _uow.SaveChangesAsync();

        return Ok(new
        {
            id = billetType.Id,
            nom = billetType.Nom,
            prix = billetType.Prix,
            quantite = billetType.Quantite,
            billetsGeneres = billetType.Billets.Count,
            message = $"{billetType.Quantite} tickets generated automatically"
        });
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBilletType(int id, [FromBody] UpdateBilletTypeDto updateDto)
    {
        var billetType = await _uow.BilletTypes.GetByIdAsync(id);
        if (billetType == null)
            return NotFound(new { message = $"Ticket type with ID {id} not found" });

        // ✅ Owner check from JWT
        var userId = ClaimsHelper.GetUserId(User);
        if (billetType.Evenement.OrganisateurId != userId)
            return StatusCode(403, new { message = "You are not the owner of this event" });

        if (updateDto.Quantite < billetType.Vendu)
            return BadRequest(new
            {
                message = $"New quantity ({updateDto.Quantite}) cannot be less than tickets already sold ({billetType.Vendu})"
            });

        var diff = updateDto.Quantite - billetType.Quantite;

        if (diff > 0)
        {
            for (int i = 0; i < diff; i++)
            {
                billetType.Billets.Add(new Billet
                {
                    Code = _codeGenerator.Generate(billetType.EvenementId),
                    Statut = "Disponible",
                    BilletType = billetType
                });
            }
        }
        else if (diff < 0)
        {
            var toRemove = billetType.Billets
                .Where(b => b.Statut == "Disponible")
                .TakeLast(Math.Abs(diff))
                .ToList();

            if (toRemove.Count < Math.Abs(diff))
                return BadRequest(new { message = "Cannot reduce quantity: not enough available tickets to remove" });

            _uow.Billets.DeleteRange(toRemove);
        }

        billetType.Nom = updateDto.Nom;
        billetType.Prix = updateDto.Prix;
        billetType.Quantite = updateDto.Quantite;

        _uow.BilletTypes.Update(billetType);
        await _uow.SaveChangesAsync();

        return Ok(new
        {
            id = billetType.Id,
            nom = billetType.Nom,
            prix = billetType.Prix,
            quantite = billetType.Quantite,
            vendu = billetType.Vendu,
            message = "Ticket type updated successfully"
        });
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBilletType(int id)
    {
        var billetType = await _uow.BilletTypes.GetByIdAsync(id);
        if (billetType == null)
            return NotFound(new { message = $"Ticket type with ID {id} not found" });

        // ✅ Owner check from JWT
        var userId = ClaimsHelper.GetUserId(User);
        if (billetType.Evenement.OrganisateurId != userId)
            return StatusCode(403, new { message = "You are not the owner of this event" });

        if (billetType.Billets.Any(b => b.Statut != "Disponible"))
            return BadRequest(new { message = "Cannot delete ticket type with tickets already sold or reserved" });

        _uow.BilletTypes.Delete(billetType);
        await _uow.SaveChangesAsync();

        return NoContent();
    }
}