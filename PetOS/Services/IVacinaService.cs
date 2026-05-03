using PetOS.DTOs.Vacinas;

namespace PetOS.Services;

public interface IVacinaService
{
    Task<IReadOnlyList<VacinaResponseDto>> GetAllAsync(int? petId, DateOnly? aplicadaAte);
    Task<VacinaResponseDto> GetByIdAsync(int id);
    Task<VacinaResponseDto> CreateAsync(CreateVacinaDto dto);
    Task UpdateAsync(int id, UpdateVacinaDto dto);
    Task DeleteAsync(int id);
}

