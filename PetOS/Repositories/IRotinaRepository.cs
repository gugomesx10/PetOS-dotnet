using PetOS.Models;

namespace PetOS.Repositories;

public interface IRotinaRepository
{
    Task<IReadOnlyList<Rotina>> GetAllAsync(int? petId, bool? ativa);
    Task<Rotina?> GetByIdAsync(int id);
    Task<Rotina> AddAsync(Rotina rotina);
    Task UpdateAsync(Rotina rotina);
    Task DeleteAsync(Rotina rotina);
}

