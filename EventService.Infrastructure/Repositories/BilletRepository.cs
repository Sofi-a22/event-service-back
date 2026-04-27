using EventService.Core.Interfaces;
using EventService.Core.Models;
using EventService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventService.Infrastructure.Repositories;

public class BilletRepository : IBilletRepository
{
    private readonly EventDbContext _context;

    public BilletRepository(EventDbContext context)
    {
        _context = context;
    }

    public async Task<Billet?> GetByIdAsync(int id)
    {
        return await _context.Billets
            .Include(b => b.BilletType)
                .ThenInclude(bt => bt.Evenement)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Billet?> GetByCodeAsync(string code)
    {
        return await _context.Billets
            .Include(b => b.BilletType)
                .ThenInclude(bt => bt.Evenement)
            .FirstOrDefaultAsync(b => b.Code == code);
    }

    public async Task<IEnumerable<Billet>> GetByVisiteurAsync(int visiteurId)
    {
        return await _context.Billets
            .Where(b => b.VisiteurId == visiteurId)
            .Include(b => b.BilletType)
                .ThenInclude(bt => bt.Evenement)
            .OrderByDescending(b => b.DateReservation)
            .ToListAsync();
    }

    public async Task<IEnumerable<Billet>> GetAvailableByTypeAsync(int billetTypeId, int count)
    {
        return await _context.Billets
            .Where(b => b.BilletTypeId == billetTypeId && b.Statut == "Disponible")
            .Take(count)
            .ToListAsync();
    }

    public void Update(Billet billet)
    {
        _context.Billets.Update(billet);
    }

    public void DeleteRange(IEnumerable<Billet> billets)
    {
        _context.Billets.RemoveRange(billets);
    }
}