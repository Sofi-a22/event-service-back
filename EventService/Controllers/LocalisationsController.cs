using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EventService.Core.Models;
using EventService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LocalisationsController : ControllerBase
{
    private readonly EventDbContext _context;

    public LocalisationsController(EventDbContext context)
    {
        _context = context;
    }

    // GET: api/localisations
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var localisations = await _context.Localisations.ToListAsync();
        return Ok(localisations);
    }

    // POST: api/localisations
    [Authorize]
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateLocalisationDto dto)
    {
        var localisation = new Localisation
        {
            Adresse = dto.Adresse,
            Ville = dto.Ville,
            CodePostal = dto.CodePostal ?? string.Empty,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude
        };

        _context.Localisations.Add(localisation);
        await _context.SaveChangesAsync();

        return Ok(localisation);
    }
}

// DTO
public class CreateLocalisationDto
{
    public string Adresse { get; set; } = string.Empty;
    public string Ville { get; set; } = string.Empty;
    public string? CodePostal { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}