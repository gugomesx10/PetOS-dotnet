using PetOS.DTOs.Alertas;
using PetOS.Models;

namespace PetOS.Services;

public interface IAlertaService
{
    Task<IReadOnlyList<AlertaResponseDto>> GetAllAsync(int? petId, AlertaStatus? status, DateTime? ate);
    Task<AlertaResponseDto> GetByIdAsync(int id);
    Task<AlertaResponseDto> CreateAsync(CreateAlertaDto dto);
    Task UpdateAsync(int id, UpdateAlertaDto dto);
    Task DeleteAsync(int id);
}

