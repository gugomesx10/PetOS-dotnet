using PetOS.Dto.Pet;

namespace PetOS.Services.Interfaces;

public interface IPetService
{
    Task<IEnumerable<PetResponseDto>> GetAllSync();
    Task<PetResponseDto?> GetByIdAsync(long id);
    Task<IEnumerable<PetResponseDto>> GetBySpecieAsync(string species);
    Task<IEnumerable<PetResponseDto>> GetByNameAsync( string name);
    Task<PetResponseDto> CreateAsync(PetCreateDto dto);
    Task UpdateAsync(long id, PetCreateDto dto);
    Task DeleteAsync(long id);

}