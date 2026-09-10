using PetOS.Models;
using PetOS.Repositories.Interfaces;
using PetOS.Data;
using Microsoft.EntityFrameworkCore;
using PetOS.Observability;

namespace PetOS.Repositories;

public class PetRepository : IPetRepository
{
    private readonly AppDbContext _context;
    
    public PetRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Pet>> GetAllAsync()
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("PetRepository.GetAllAsync");

        return await _context.Pets.ToListAsync();
    }

    public async Task<Pet?> GetByIdAsync(long id)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("PetRepository.GetByIdAsync");

        activity?.SetTag("pet.id", id);

        return await _context.Pets.FindAsync(id);
    }

    public async Task<IEnumerable<Pet>> GetBySpeciesAsync(string species)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("PetRepository.GetBySpeciesAsync");

        activity?.SetTag("pet.species", species);

        return await _context.Pets
            .Where(p => p.Species.ToLower() == species.ToLower())
            .ToListAsync();
    }

    public async Task<IEnumerable<Pet>> GetByNameAsync(string name)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("PetRepository.GetByNameAsync");

        activity?.SetTag("pet.name", name);

        return await _context.Pets
            .Where(p => p.Name.ToLower().Contains(name.ToLower()))
            .ToListAsync();
    }

    public async Task AddAsync(Pet pet)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("PetRepository.AddAsync");

        activity?.SetTag("pet.name", pet.Name);
        activity?.SetTag("pet.species", pet.Species);

        await _context.Pets.AddAsync(pet);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Pet pet)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("PetRepository.UpdateAsync");

        activity?.SetTag("pet.id", pet.Id);

        _context.Pets.Update(pet);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("PetRepository.DeleteAsync");

        activity?.SetTag("pet.id", id);

        var pet = await _context.Pets.FindAsync(id);

        if (pet != null)
        {
            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();
        }
    }
}