namespace EventService.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IEvenementRepository Evenements { get; }
    IBilletTypeRepository BilletTypes { get; }
    IBilletRepository Billets { get; }

    Task<int> SaveChangesAsync();
}