using PetOS.Models;

namespace PetOS.Repositories.Interfaces;

public interface IVaccineRepository
{
    Task <IEnumerable<Vaccine>> GetAllAsync();
    Task<Vaccine> GetByIdAsync(long id);
    Task AddAsync(Vaccine vaccine);
    Task UpdateAsync(Vaccine vaccine);
    Task DeleteAsync(long id);
}