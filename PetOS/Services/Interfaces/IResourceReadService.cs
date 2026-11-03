using PetOS.Dto.Common;
using PetOS.Dto.Pet;
using PetOS.Dto.Vaccine;
using PetOS.Dto.Routine;
using PetOS.Dto.Alert;

namespace PetOS.Services.Interfaces;

public interface IResourceReadService
{
    Task<PagedResult<PetResponseDto>> GetPetsAsync(
        PetQuery query);

    Task<PagedResult<VaccineResponseDto>> GetVaccinesAsync(
        VaccineQuery query);

    Task<PagedResult<RoutineResponseDto>> GetRoutinesAsync(
        RoutineQuery query);

    Task<PagedResult<AlertResponseDto>> GetAlertsAsync(
        AlertQuery query);
}