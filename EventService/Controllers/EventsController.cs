using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventService.Core.Models;
using EventService.Infrastructure.Data;
using EventService.Core.DTOs;

namespace EventService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly EventDbContext _context;

        public EventsController(EventDbContext context)
        {
            _context = context;
        }

        // ========== CRUD EXISTANT ==========

        // GET: api/events
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Evenement>>> GetEvents()
        {
            var events = await _context.Evenements
                .Include(e => e.Localisation)
                .Include(e => e.BilletTypes)
                .ToListAsync();

            return Ok(events);
        }

        // GET: api/events/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Evenement>> GetEvent(int id)
        {
            var evenement = await _context.Evenements
                .Include(e => e.Localisation)
                .Include(e => e.BilletTypes)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (evenement == null)
            {
                return NotFound(new { message = $"Event with ID {id} not found" });
            }

            return Ok(evenement);
        }

        // GET: api/events/organizer/{organisateurId}
        [HttpGet("organizer/{organisateurId}")]
        public async Task<ActionResult<IEnumerable<Evenement>>> GetEventsByOrganizer(int organisateurId)
        {
            var events = await _context.Evenements
                .Include(e => e.Localisation)
                .Include(e => e.BilletTypes)
                .Where(e => e.OrganisateurId == organisateurId)
                .ToListAsync();

            return Ok(events);
        }

        // POST: api/events
        [HttpPost]
        public async Task<ActionResult<Evenement>> CreateEvent(CreateEventDto createDto)
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
                CreatedAt = DateTime.UtcNow
            };

            _context.Evenements.Add(evenement);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvent), new { id = evenement.Id }, evenement);
        }

        // PUT: api/events/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, UpdateEventDto updateDto)
        {
            if (id != updateDto.Id)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            var existingEvent = await _context.Evenements.FindAsync(id);
            if (existingEvent == null)
            {
                return NotFound(new { message = $"Event with ID {id} not found" });
            }

            existingEvent.Titre = updateDto.Titre;
            existingEvent.Description = updateDto.Description;
            existingEvent.StartDate = updateDto.StartDate;
            existingEvent.EndDate = updateDto.EndDate;
            existingEvent.TypeEvent = updateDto.TypeEvent;
            existingEvent.Categorie = updateDto.Categorie;
            existingEvent.Disponibilite = updateDto.Disponibilite;
            existingEvent.Capacite = updateDto.Capacite;
            existingEvent.PlacesRestantes = updateDto.Capacite - (existingEvent.Capacite - existingEvent.PlacesRestantes);
            existingEvent.OrganisateurId = updateDto.OrganisateurId;
            existingEvent.LocalisationId = updateDto.LocalisationId;
            existingEvent.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/events/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var evenement = await _context.Evenements
                .Include(e => e.BilletTypes)
                .ThenInclude(bt => bt.Billets)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (evenement == null)
            {
                return NotFound(new { message = $"Event with ID {id} not found" });
            }

            _context.Evenements.Remove(evenement);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/events/{id}/purchase
        [HttpPost("{id}/purchase")]
        public async Task<IActionResult> PurchaseTickets(int id, [FromBody] PurchaseRequestDto request)
        {
            var evenement = await _context.Evenements
                .Include(e => e.BilletTypes)
                .ThenInclude(bt => bt.Billets)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (evenement == null)
                return NotFound(new { message = "Event not found" });

            // Limite de 5 billets maximum
            if (request.Quantite > 5)
                return BadRequest(new { message = "Maximum 5 tickets per purchase" });

            // Récupérer les billets disponibles
            var billetsDisponibles = evenement.BilletTypes
                .SelectMany(bt => bt.Billets)
                .Where(b => b.Statut == "Disponible")
                .Take(request.Quantite)
                .ToList();

            if (billetsDisponibles.Count < request.Quantite)
                return BadRequest(new { message = "Not enough tickets available" });

            // Réserver les billets
            foreach (var billet in billetsDisponibles)
            {
                billet.Statut = "Reserve";
                billet.DateReservation = DateTime.UtcNow;
                billet.VisiteurId = request.VisiteurId;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"{request.Quantite} ticket(s) reserved",
                tickets = billetsDisponibles.Select(b => new { b.Id, b.Code, b.Statut })
            });
        }

        // ========== NOUVEAUX ENDPOINTS DE RECHERCHE ==========

        // GET: api/events/search?q=music&categorie=Music&typeEvent=ONSITE
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Evenement>>> SearchEvents(
            [FromQuery] string? q,
            [FromQuery] string? categorie,
            [FromQuery] string? typeEvent,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var query = _context.Evenements
                .Include(e => e.Localisation)
                .Include(e => e.BilletTypes)
                .AsQueryable();

            // 1. Recherche par mot-clé (titre ou description)
            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(e =>
                    e.Titre.ToLower().Contains(q.ToLower()) ||
                    (e.Description != null && e.Description.ToLower().Contains(q.ToLower())));
            }

            // 2. Filtre par catégorie
            if (!string.IsNullOrWhiteSpace(categorie))
            {
                query = query.Where(e => e.Categorie == categorie);
            }

            // 3. Filtre par type d'événement (ONSITE, ONLINE, HYBRID)
            if (!string.IsNullOrWhiteSpace(typeEvent))
            {
                query = query.Where(e => e.TypeEvent == typeEvent);
            }

            // 4. Filtre par date de début
            if (startDate.HasValue)
            {
                query = query.Where(e => e.StartDate.Date >= startDate.Value.Date);
            }

            // 5. Filtre par date de fin
            if (endDate.HasValue)
            {
                query = query.Where(e => e.EndDate.Date <= endDate.Value.Date);
            }

            // Trier par date (les plus proches d'abord)
            query = query.OrderBy(e => e.StartDate);

            var events = await query.ToListAsync();
            return Ok(events);
        }

        // GET: api/events/categories
        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<string>>> GetCategories()
        {
            var categories = await _context.Evenements
                .Where(e => !string.IsNullOrEmpty(e.Categorie))
                .Select(e => e.Categorie)
                .Distinct()
                .ToListAsync();

            return Ok(categories);
        }

        // GET: api/events/upcoming
        [HttpGet("upcoming")]
        public async Task<ActionResult<IEnumerable<Evenement>>> GetUpcomingEvents()
        {
            var today = DateTime.UtcNow.Date;

            var events = await _context.Evenements
                .Include(e => e.Localisation)
                .Include(e => e.BilletTypes)
                .Where(e => e.StartDate.Date >= today && e.Disponibilite == "Disponible")
                .OrderBy(e => e.StartDate)
                .Take(10)
                .ToListAsync();

            return Ok(events);
        }

        // GET: api/events/free
        [HttpGet("free")]
        public async Task<ActionResult<IEnumerable<Evenement>>> GetFreeEvents()
        {
            var events = await _context.Evenements
                .Include(e => e.BilletTypes)
                .Where(e => e.BilletTypes.Any(bt => bt.Prix == 0) && e.Disponibilite == "Disponible")
                .OrderBy(e => e.StartDate)
                .ToListAsync();

            return Ok(events);
        }

        private string GenerateLienPartage()
        {
            return Guid.NewGuid().ToString().Substring(0, 8);
        }
    }
}