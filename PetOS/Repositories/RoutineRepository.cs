using PetOS.Models;
using PetOS.Repositories.Interfaces;
using PetOS.Data;
using Microsoft.EntityFrameworkCore;

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
        return await _context.RoutineRecords.ToListAsync();
    }

    public async Task<RoutineRecord?> GetByIdAsync(long id)
    {
        return await _context.RoutineRecords.FindAsync(id);
    }

    public async Task<IEnumerable<RoutineRecord>> GetByPetIdAsync(long petId)
    {
        return await _context.RoutineRecords
            .Where(r => r.PetId == petId)
            .ToListAsync();
    }

    public async Task AddAsync(RoutineRecord routine)
    {
        await _context.RoutineRecords.AddAsync(routine);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(RoutineRecord routine)
    {
        _context.RoutineRecords.Update(routine);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var routine = await _context.RoutineRecords.FindAsync(id);

        if (routine != null)
        {
            _context.RoutineRecords.Remove(routine);
            await _context.SaveChangesAsync();
        }
    }
}