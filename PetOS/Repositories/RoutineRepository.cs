using PetOS.Models;
using PetOS.Repositories.Interfaces;
using PetOS.Data;
using Microsoft.EntityFrameworkCore;
using PetOS.Observability;

namespace PetOS.Repositories;

public class RoutineRepository : IRoutineRepository
{
    private readonly AppDbContext _context;
    
    public RoutineRepository (AppDbContext context)
        {
        _context = context;
        }

    public async Task<IEnumerable<RoutineRecord>> GetAllAsync()
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("RoutineRepository.GetAllAsync");

        return await _context.RoutineRecords.ToListAsync();
    }

    public async Task<RoutineRecord?> GetByIdAsync(long id)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("RoutineRepository.GetByIdAsync");

        activity?.SetTag("routine.id", id);

        return await _context.RoutineRecords.FindAsync(id);
    }

    public async Task<IEnumerable<RoutineRecord>> GetByPetIdAsync(long petId)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("RoutineRepository.GetByPetIdAsync");

        activity?.SetTag("pet.id", petId);

        return await _context.RoutineRecords
            .Where(r => r.PetId == petId)
            .ToListAsync();
    }

    public async Task AddAsync(RoutineRecord routine)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("RoutineRepository.AddAsync");

        await _context.RoutineRecords.AddAsync(routine);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(RoutineRecord routine)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("RoutineRepository.UpdateAsync");

        activity?.SetTag("routine.id", routine.Id);

        _context.RoutineRecords.Update(routine);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("RoutineRepository.DeleteAsync");

        activity?.SetTag("routine.id", id);

        var routine = await _context.RoutineRecords.FindAsync(id);

        if (routine != null)
        {
            _context.RoutineRecords.Remove(routine);
            await _context.SaveChangesAsync();
        }
    }
}