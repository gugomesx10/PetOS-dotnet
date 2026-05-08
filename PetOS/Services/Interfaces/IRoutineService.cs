using PetOS.Dto.Routine;

namespace PetOS.Services.Interfaces;

public interface IRoutineService
{
    Task<IEnumerable<RoutineResponseDto>> GetAllAsync();
    Task<RoutineResponseDto?> GetByIdAsync(long id);
    Task<IEnumerable<RoutineResponseDto>> GetByPetIdAsync(long petId);
    Task<RoutineResponseDto> CreateAsync(RoutineCreateDto dto);
    Task UpdateAsync(long id, RoutineCreateDto dto);
    Task DeleteAsync(long id);
}