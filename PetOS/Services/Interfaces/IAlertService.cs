using PetOS.Dto.Alert;
using PetOS.Dto.Routine;

namespace PetOS.Services.Interfaces;

public interface IAlertService
{
    Task<IEnumerable<AlertResponseDto>> GetAllAsync();
    Task<AlertResponseDto?> GetByIdAsync(long id);
    Task<AlertResponseDto> CreateAsync(AlertCreateDto dto);
    Task UpdateAsync(long id, AlertCreateDto dto);
    Task DeleteAsync(long id);
}