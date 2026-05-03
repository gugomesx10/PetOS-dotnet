using PetOS.DTOs.Rotinas;

namespace PetOS.Services;

public interface IRotinaService
{
    Task<IReadOnlyList<RotinaResponseDto>> GetAllAsync(int? petId, bool? ativa);
    Task<RotinaResponseDto> GetByIdAsync(int id);
    Task<RotinaResponseDto> CreateAsync(CreateRotinaDto dto);
    Task UpdateAsync(int id, UpdateRotinaDto dto);
    Task DeleteAsync(int id);
}

