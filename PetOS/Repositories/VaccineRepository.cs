using PetOS.Models;
using PetOS.Repositories.Interfaces;
using PetOS.Data;
using Microsoft.EntityFrameworkCore;
using PetOS.Observability;

namespace PetOS.Repositories;

public class VaccineRepository :  IVaccineRepository
{
    private readonly AppDbContext _context;
    
    public VaccineRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Vaccine>> GetAllAsync()
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("VaccineRepository.GetAllAsync");

        return await _context.Vaccines.ToListAsync();
    }

    public async Task<Vaccine?> GetByIdAsync(long id)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("VaccineRepository.GetByIdAsync");

        activity?.SetTag("vaccine.id", id);

        return await _context.Vaccines.FindAsync(id);
    }

    public async Task<IEnumerable<Vaccine>> GetByPetIdAsync(long petId)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("VaccineRepository.GetByPetIdAsync");

        activity?.SetTag("pet.id", petId);

        return await _context.Vaccines
            .Where(v => v.PetId == petId)
            .ToListAsync();
    }

    public async Task AddAsync(Vaccine vaccine)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("VaccineRepository.AddAsync");

        await _context.Vaccines.AddAsync(vaccine);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Vaccine vaccine)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("VaccineRepository.UpdateAsync");

        activity?.SetTag("vaccine.id", vaccine.Id);

        _context.Vaccines.Update(vaccine);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("VaccineRepository.DeleteAsync");

        activity?.SetTag("vaccine.id", id);

        var vaccine = await _context.Vaccines.FindAsync(id);

        if (vaccine != null)
        {
            _context.Vaccines.Remove(vaccine);
            await _context.SaveChangesAsync();
        }
    }
}