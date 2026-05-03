using PetOS.Models;

namespace PetOS.Repositories;

public interface IPetRepository
{
    Task<IReadOnlyList<Pet>> GetAllAsync(string? especie, string? responsavel);
    Task<Pet?> GetByIdAsync(int id);
    Task<Pet> AddAsync(Pet pet);
    Task UpdateAsync(Pet pet);
    Task DeleteAsync(Pet pet);
    Task<bool> ExistsAsync(int id);
}

