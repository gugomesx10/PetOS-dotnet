using PetOS.Dto.Common;
using PetOS.Dto.PetActivity;
using PetOS.Models;

namespace PetOS.Repositories.Interfaces;

/// <summary>
/// Define as operações de persistência das atividades no MongoDB.
/// </summary>
public interface IPetActivityRepository
{
    Task<PagedResult<PetActivity>> GetPagedAsync(
        PetActivityQuery query);

    Task<PetActivity?> GetByIdAsync(string id);

    Task InsertAsync(PetActivity activity);

    Task<bool> ReplaceAsync(PetActivity activity);

    Task<bool> DeleteAsync(string id);
}