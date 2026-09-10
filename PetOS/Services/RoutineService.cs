using PetOS.Dto.Routine;
using PetOS.Models;
using PetOS.Repositories.Interfaces;
using PetOS.Services.Interfaces;
using PetOS.Observability;

namespace PetOS.Services;

public class RoutineService : IRoutineService
{
    private readonly IRoutineRepository _repository;
    
    public RoutineService(IRoutineRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<RoutineResponseDto>> GetAllAsync()
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("RoutineService.GetAllAsync");
        var routines = await _repository.GetAllAsync();

        return routines.Select(r => new RoutineResponseDto()
        {
            Id = r.Id,
            PetId = r.PetId,
            Type = r.Type,
            Description = r.Description,
            Date = r.Date,
            Notes = r.Notes,
            CreatedAt = r.CreatedAt
        });
    }

    public async Task<RoutineResponseDto?> GetByIdAsync(long id)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("RoutineService.GetByIdAsync");

        activity?.SetTag("routine.id", id);
        var routine = await _repository.GetByIdAsync(id);
        
        if (routine == null)
        {
            return null;
        }

        return new RoutineResponseDto
        {
            Id = routine.Id,
            PetId = routine.PetId,
            Type = routine.Type,
            Description = routine.Description,
            Date = routine.Date,
            Notes = routine.Notes,
            CreatedAt = routine.CreatedAt
        };
    }

    public async Task<IEnumerable<RoutineResponseDto>> GetByPetIdAsync(long petId)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("RoutineService.GetByPetIdAsync");

        activity?.SetTag("pet.id", petId);
        var routines = await _repository.GetByPetIdAsync(petId);

        return routines.Select(r => new RoutineResponseDto
        {
            Id = r.Id,
            PetId = r.PetId,
            Type = r.Type,
            Description = r.Description,
            Date = r.Date,
            Notes = r.Notes,
            CreatedAt = r.CreatedAt
        });
    }

    public async Task<RoutineResponseDto> CreateAsync(RoutineCreateDto dto)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("RoutineService.CreateAsync");

        activity?.SetTag("pet.id", dto.PetId);
        var routine = new RoutineRecord
        {
            PetId = dto.PetId,
            Type = dto.Type,
            Description = dto.Description,
            Date = dto.Date,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow
        };
        
        await _repository.AddAsync(routine);

        return new RoutineResponseDto
        {
            Id = routine.Id,
            PetId = routine.PetId,
            Type = routine.Type,
            Description = routine.Description,
            Date = routine.Date,
            Notes = routine.Notes,
            CreatedAt = routine.CreatedAt
        };
    }

    public async Task UpdateAsync(long id, RoutineCreateDto dto)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("RoutineService.UpdateAsync");

        activity?.SetTag("routine.id", id);
        var routine = await _repository.GetByIdAsync(id);
        
        if (routine == null)
            return;
        
        routine.PetId = dto.PetId;
        routine.Type = dto.Type;
        routine.Description =  dto.Description;
        routine.Date = dto.Date;
        routine.Notes = dto.Notes;
        
        await _repository.UpdateAsync(routine);
    }
    
    public async Task DeleteAsync(long id)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("RoutineService.DeleteAsync");

        activity?.SetTag("routine.id", id);
        await _repository.DeleteAsync(id);
    }
}