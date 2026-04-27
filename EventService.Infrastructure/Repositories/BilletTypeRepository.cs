using EventService.Core.Interfaces;
using EventService.Core.Models;
using EventService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventService.Infrastructure.Repositories;

public class BilletTypeRepository : IBilletTypeRepository
{
    private readonly EventDbContext _context;

    public BilletTypeRepository(EventDbContext context)
    {
        _context = context;
    }

    public async Task<BilletType?> GetByIdAsync(int id)
    {
        return await _context.BilletTypes
            .Include(bt => bt.Billets)
            .Include(bt => bt.Evenement)
            .FirstOrDefaultAsync(bt => bt.Id == id);
    }

    public async Task<IEnumerable<BilletType>> GetByEvenementAsync(int evenementId)
    {
        return await _context.BilletTypes
            .Where(bt => bt.EvenementId == evenementId)
            .Include(bt => bt.Billets)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(int evenementId, string nom)
    {
        var count = await _context.BilletTypes
        .CountAsync(bt => bt.EvenementId == evenementId && bt.Nom == nom);
        return count > 0;

    }

    public async Task AddAsync(BilletType billetType)
    {
        await _context.BilletTypes.AddAsync(billetType);
    }

    public void Update(BilletType billetType)
    {
        _context.BilletTypes.Update(billetType);
    }

    public void Delete(BilletType billetType)
    {
        _context.BilletTypes.Remove(billetType);
    }
}