using PetOS.Models;

namespace PetOS.Repositories;

public interface IVacinaRepository
{
    Task<IReadOnlyList<Vacina>> GetAllAsync(int? petId, DateOnly? aplicadaAte);
    Task<Vacina?> GetByIdAsync(int id);
    Task<Vacina> AddAsync(Vacina vacina);
    Task UpdateAsync(Vacina vacina);
    Task DeleteAsync(Vacina vacina);
}

