using PetOS.Dto.Routine;
using PetOS.Models;
using PetOS.Repositories.Interfaces;
using PetOS.Services.Interfaces;

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
        var routines = await _repository.GetAllAsync();

        return routines.Select(r => new RoutineResponseDto()
        {
            Id = r.Id,
            PetId = r.PetId,
            Type = r.Type,
            Description = r.Decription,
            Date = r.Date,
            Notes = r.Notes,
            CreatedAt = r.CreatedAt
        });
    }

    public async Task<RoutineResponseDto?> GetByIdAsync(long id)
    {
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
            Description = routine.Decription,
            Date = routine.Date,
            Notes = routine.Notes,
            CreatedAt = routine.CreatedAt
        };
    }

    public async Task<RoutineResponseDto> CreateAsync(RoutineCreateDto dto)
    {
        var routine = new RoutineRecord
        {
            PetId = dto.PetId,
            Type = dto.Type,
            Decription = dto.Description,
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
            Description = routine.Decription,
            Date = routine.Date,
            Notes = routine.Notes,
            CreatedAt = routine.CreatedAt
        };
    }

    public async Task UpdateAsync(long id, RoutineCreateDto dto)
    {
        var routine = await _repository.GetByIdAsync(id);
        
        if (routine == null)
            return;
        
        routine.PetId = dto.PetId;
        routine.Type = dto.Type;
        routine.Decription =  dto.Description;
        routine.Date = dto.Date;
        routine.Notes = dto.Notes;
        
        await _repository.UpdateAsync(routine);
    }
    
    public async Task DeleteAsync(long id)
    {
        await _repository.DeleteAsync(id);
    }
}