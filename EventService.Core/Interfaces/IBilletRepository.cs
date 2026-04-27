using EventService.Core.Models;

namespace EventService.Core.Interfaces;

public interface IBilletRepository
{
    Task<Billet?> GetByIdAsync(int id);
    Task<Billet?> GetByCodeAsync(string code);
    Task<IEnumerable<Billet>> GetByVisiteurAsync(int visiteurId);
    Task<IEnumerable<Billet>> GetAvailableByTypeAsync(int billetTypeId, int count);
    void Update(Billet billet);
    void DeleteRange(IEnumerable<Billet> billets);
}