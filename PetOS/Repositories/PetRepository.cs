using PetOS.Models;
using PetOS.Repositories.Interfaces;
using PetOS.Data;
using Microsoft.EntityFrameworkCore;

namespace PetOS.Repositories;

public class PetRepository : IPetRepository
{
    private readonly AppDbContext _context;
    
    public PetRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Pet>>  GetAllAsync()
    {
        return await _context.Pets.ToListAsync();
    }

    public async Task<Pet?> GetByIdAsync(long id)
    {
        return await _context.Pets.FindAsync(id);
    }

    public async Task<IEnumerable<Pet>> GetBySpeciesAsync(string species)
    {
        return await _context.Pets
            .Where(p => p.Species.ToLower() == species.ToLower())
            .ToListAsync();
    }

    public async Task<IEnumerable<Pet>> GetByNameAsync(string name)
    {
        return await _context.Pets
            .Where(p => p.Name.ToLower().Contains(name.ToLower()))
            .ToListAsync();
    }

    public async Task AddAsync(Pet pet)
    {
        await _context.Pets.AddAsync(pet);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Pet pet)
    {
        _context.Pets.Update(pet);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var pet = await _context.Pets.FindAsync(id);

        if (pet != null)
        {
            _context.Pets.Remove(pet);
            
            await _context.SaveChangesAsync();
        }
    }
}