using EventService.Core.Interfaces;
using EventService.Infrastructure.Data;

namespace EventService.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly EventDbContext _context;

    public IEvenementRepository Evenements { get; }
    public IBilletTypeRepository BilletTypes { get; }
    public IBilletRepository Billets { get; }

    public UnitOfWork(EventDbContext context)
    {
        _context = context;

        // All repositories share the SAME DbContext instance
        Evenements = new EvenementRepository(context);
        BilletTypes = new BilletTypeRepository(context);
        Billets = new BilletRepository(context);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}