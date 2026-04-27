using EventService.Core.Models;

namespace EventService.Core.Interfaces;

public interface IEvenementRepository
{
    Task<IEnumerable<Evenement>> GetAllAsync(int page, int pageSize);
    Task<int> GetTotalCountAsync();
    Task<Evenement?> GetByIdAsync(int id);
    Task<IEnumerable<Evenement>> GetByOrganisateurAsync(int organisateurId);
    Task<IEnumerable<Evenement>> SearchAsync(string? q, string? categorie, string? typeEvent, DateTime? startDate, DateTime? endDate, int page, int pageSize);
    Task<int> GetSearchCountAsync(string? q, string? categorie, string? typeEvent, DateTime? startDate, DateTime? endDate);
    Task<IEnumerable<Evenement>> GetUpcomingAsync(int count);
    Task<IEnumerable<Evenement>> GetFreeAsync();
    Task<IEnumerable<string>> GetCategoriesAsync();
    Task AddAsync(Evenement evenement);
    void Update(Evenement evenement);
    void SoftDelete(Evenement evenement);
}