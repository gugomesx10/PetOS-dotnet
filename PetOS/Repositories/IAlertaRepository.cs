using PetOS.Models;

namespace PetOS.Repositories;

public interface IAlertaRepository
{
    Task<IReadOnlyList<Alerta>> GetAllAsync(int? petId, AlertaStatus? status, DateTime? ate);
    Task<Alerta?> GetByIdAsync(int id);
    Task<Alerta> AddAsync(Alerta alerta);
    Task UpdateAsync(Alerta alerta);
    Task DeleteAsync(Alerta alerta);
}

