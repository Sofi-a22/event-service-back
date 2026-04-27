using EventService.Core.Interfaces;
using EventService.Core.Models;
using EventService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventService.Infrastructure.Repositories;

public class EvenementRepository : IEvenementRepository
{
    private readonly EventDbContext _context;

    public EvenementRepository(EventDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Evenement>> GetAllAsync(int page, int pageSize)
    {
        return await _context.Evenements
            .Where(e => !e.IsDeleted)
            .Include(e => e.Localisation)
            .Include(e => e.BilletTypes)
            .OrderBy(e => e.StartDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _context.Evenements
            .CountAsync(e => !e.IsDeleted);
    }

    public async Task<Evenement?> GetByIdAsync(int id)
    {
        return await _context.Evenements
            .Where(e => !e.IsDeleted)
            .Include(e => e.Localisation)
            .Include(e => e.BilletTypes)
                .ThenInclude(bt => bt.Billets)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Evenement>> GetByOrganisateurAsync(int organisateurId)
    {
        return await _context.Evenements
            .Where(e => e.OrganisateurId == organisateurId && !e.IsDeleted)
            .Include(e => e.Localisation)
            .Include(e => e.BilletTypes)
            .OrderBy(e => e.StartDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Evenement>> SearchAsync(
        string? q, string? categorie, string? typeEvent,
        DateTime? startDate, DateTime? endDate,
        int page, int pageSize)
    {
        var query = BuildSearchQuery(q, categorie, typeEvent, startDate, endDate);

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetSearchCountAsync(
        string? q, string? categorie, string? typeEvent,
        DateTime? startDate, DateTime? endDate)
    {
        return await BuildSearchQuery(q, categorie, typeEvent, startDate, endDate)
            .CountAsync();
    }

    public async Task<IEnumerable<Evenement>> GetUpcomingAsync(int count)
    {
        var today = DateTime.UtcNow.Date;

        return await _context.Evenements
            .Where(e => !e.IsDeleted
                     && e.StartDate.Date >= today
                     && e.Disponibilite == "Disponible")
            .Include(e => e.Localisation)
            .Include(e => e.BilletTypes)
            .OrderBy(e => e.StartDate)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Evenement>> GetFreeAsync()
    {
        return await _context.Evenements
            .Where(e => !e.IsDeleted
                     && e.Disponibilite == "Disponible"
                     && e.BilletTypes.Any(bt => bt.Prix == 0))
            .Include(e => e.BilletTypes)
            .OrderBy(e => e.StartDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetCategoriesAsync()
    {
        return await _context.Evenements
            .Where(e => !e.IsDeleted && !string.IsNullOrEmpty(e.Categorie))
            .Select(e => e.Categorie)
            .Distinct()
            .ToListAsync();
    }

    public async Task AddAsync(Evenement evenement)
    {
        await _context.Evenements.AddAsync(evenement);
    }

    public void Update(Evenement evenement)
    {
        _context.Evenements.Update(evenement);
    }

    public void SoftDelete(Evenement evenement)
    {
        evenement.IsDeleted = true;
        evenement.UpdatedAt = DateTime.UtcNow;
        _context.Evenements.Update(evenement);
    }

    // ── Private helpers ──────────────────────────────────────────────

    private IQueryable<Evenement> BuildSearchQuery(
        string? q, string? categorie, string? typeEvent,
        DateTime? startDate, DateTime? endDate)
    {
        var query = _context.Evenements
            .Where(e => !e.IsDeleted)
            .Include(e => e.Localisation)
            .Include(e => e.BilletTypes)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(e =>
                e.Titre.ToLower().Contains(q.ToLower()) ||
                (e.Description != null && e.Description.ToLower().Contains(q.ToLower())));

        if (!string.IsNullOrWhiteSpace(categorie))
            query = query.Where(e => e.Categorie == categorie);

        if (!string.IsNullOrWhiteSpace(typeEvent))
            query = query.Where(e => e.TypeEvent == typeEvent);

        if (startDate.HasValue)
            query = query.Where(e => e.StartDate.Date >= startDate.Value.Date);

        if (endDate.HasValue)
            query = query.Where(e => e.EndDate.Date <= endDate.Value.Date);

        return query.OrderBy(e => e.StartDate);
    }
}