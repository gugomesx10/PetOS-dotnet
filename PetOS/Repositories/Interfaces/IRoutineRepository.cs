using PetOS.Models;

namespace PetOS.Repositories.Interfaces;

public interface IRoutineRepository
{
        Task<IEnumerable<RoutineRecord>> GetAllAsync();
        Task<RoutineRecord?> GetByIdAsync(long id);
        Task<IEnumerable<RoutineRecord>> GetByPetIdAsync(long petId);
        Task AddAsync(RoutineRecord routine);
        Task UpdateAsync(RoutineRecord routine);
        Task DeleteAsync(long id);
}