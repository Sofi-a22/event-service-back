using EventService.Core.Models;

namespace EventService.Core.Interfaces;

public interface IBilletTypeRepository
{
    Task<BilletType?> GetByIdAsync(int id);
    Task<IEnumerable<BilletType>> GetByEvenementAsync(int evenementId);
    Task<bool> ExistsAsync(int evenementId, string nom);
    Task AddAsync(BilletType billetType);
    void Update(BilletType billetType);
    void Delete(BilletType billetType);
}