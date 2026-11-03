using PetOS.Dto.Common;
using PetOS.Dto.PetActivity;
using PetOS.Models;

namespace PetOS.Services.Interfaces;

/// <summary>
/// Define as operações de negócio relacionadas às atividades.
/// </summary>
public interface IPetActivityService
{
    Task<PagedResult<PetActivity>> GetPagedAsync(
        PetActivityQuery query);

    Task<PetActivity?> GetByIdAsync(string id);

    Task<PetActivity> CreateAsync(
        PetActivityCreateDto dto);

    Task<bool> UpdateAsync(
        string id,
        PetActivityCreateDto dto);

    Task<bool> DeleteAsync(string id);
}