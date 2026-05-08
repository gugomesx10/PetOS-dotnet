using PetOS.Dto.Vaccine;

namespace PetOS.Services.Interfaces;

public interface IVaccineService
{
    Task<IEnumerable<VaccineResponseDto>> GetAllAsync();
    Task<VaccineResponseDto?> GetByIdAsync(long id);
    Task<IEnumerable<VaccineResponseDto>> GetByPetIdAsync(long petId);
    Task<VaccineResponseDto> CreateAsync(VaccineCreateDto dto);
    Task UpdateAsync(long id, VaccineCreateDto dto);
    Task DeleteAsync(long id);
}