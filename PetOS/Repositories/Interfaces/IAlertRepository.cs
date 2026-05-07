using PetOS.Models;

namespace PetOS.Repositories.Interfaces;

public interface IAlertRepository
{
    Task<IEnumerable<Alert>> GetAllAsync();
    Task<Alert?> GetByIdAsync(long id);
    Task AddAsync(Alert alert);
    Task UpdateAsync(Alert alert);
    Task DeleteAsync(long id);
}