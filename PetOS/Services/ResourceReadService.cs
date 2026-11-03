using PetOS.Dto.Common;
using PetOS.Dto.Pet;
using PetOS.Dto.Vaccine;
using PetOS.Dto.Routine;
using PetOS.Dto.Alert;
using PetOS.Repositories.Interfaces;
using PetOS.Services.Interfaces;

namespace PetOS.Services;

/// <summary>
/// Coordena as consultas paginadas dos recursos do sistema.
/// </summary>
public sealed class ResourceReadService : IResourceReadService
{
    private readonly IResourceReadRepository _repository;

    public ResourceReadService(IResourceReadRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResult<PetResponseDto>> GetPetsAsync(
        PetQuery query)
    {
        return _repository.GetPetsAsync(query);
    }

    public Task<PagedResult<VaccineResponseDto>> GetVaccinesAsync(
        VaccineQuery query)
    {
        return _repository.GetVaccinesAsync(query);
    }

    public Task<PagedResult<RoutineResponseDto>> GetRoutinesAsync(
        RoutineQuery query)
    {
        return _repository.GetRoutinesAsync(query);
    }

    public Task<PagedResult<AlertResponseDto>> GetAlertsAsync(
        AlertQuery query)
    {
        return _repository.GetAlertsAsync(query);
    }
}