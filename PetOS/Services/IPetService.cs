using PetOS.DTOs.Pets;

namespace PetOS.Services;

public interface IPetService
{
    Task<IReadOnlyList<PetResponseDto>> GetAllAsync(string? especie, string? responsavel);
    Task<PetResponseDto> GetByIdAsync(int id);
    Task<PetResponseDto> CreateAsync(CreatePetDto dto);
    Task UpdateAsync(int id, UpdatePetDto dto);
    Task DeleteAsync(int id);
}

