using Microsoft.EntityFrameworkCore;
using PetOS.Data;
using PetOS.Models;

namespace PetOS.Repositories;

public class PetRepository(AppDbContext context) : IPetRepository
{
    public async Task<IReadOnlyList<Pet>> GetAllAsync(string? especie, string? responsavel)
    {
        var query = context.Pets.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(especie))
        {
            query = query.Where(p => p.Especie.ToUpper() == especie.ToUpper());
        }

        if (!string.IsNullOrWhiteSpace(responsavel))
        {
            query = query.Where(p => p.ResponsavelNome.ToUpper().Contains(responsavel.ToUpper()));
        }

        return await query.OrderBy(p => p.Nome).ToListAsync();
    }

    public Task<Pet?> GetByIdAsync(int id) =>
        context.Pets.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Pet> AddAsync(Pet pet)
    {
        context.Pets.Add(pet);
        await context.SaveChangesAsync();
        return pet;
    }

    public async Task UpdateAsync(Pet pet)
    {
        context.Pets.Update(pet);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Pet pet)
    {
        context.Pets.Remove(pet);
        await context.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(int id) =>
        context.Pets.AsNoTracking().AnyAsync(p => p.Id == id);
}

