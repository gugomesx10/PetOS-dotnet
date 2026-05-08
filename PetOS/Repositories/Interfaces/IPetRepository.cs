using PetOS.Models;

namespace PetOS.Repositories.Interfaces;

public interface IPetRepository
{
    Task<IEnumerable<Pet>> GetAllAsync();
    Task<Pet?> GetByIdAsync(long id);
    Task<IEnumerable<Pet>> GetBySpeciesAsync(string species);
    Task<IEnumerable<Pet>> GetByNameAsync(string name);
    Task AddAsync(Pet pet);
    Task UpdateAsync(Pet pet);
    Task DeleteAsync(long id);
    
}