using PetOS.Models;
using PetOS.Repositories.Interfaces;

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
        return await _context.Vaccines.ToListAsync();
    }
    
    public async Task<Vaccine> GetByIdAsync(long id)
    {
        return await _context.Vaccines.FindAsync(id);
    }

    public async Task AddAsync(Vaccine vaccine)
    {
        await _context.Vaccines.AddAsync(vaccine);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Vaccine vaccine)
    {
        _context.Vaccines.Update(vaccine);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var vaccine = await _context.Vaccines.FindAsync(id);

        if (vaccine != null)
        {
            _context.Vaccines.Remove(vaccine);
            
            await _context.SaveChangesAsync();
        }
    }
}