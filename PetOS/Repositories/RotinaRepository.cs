using Microsoft.EntityFrameworkCore;
using PetOS.Data;
using PetOS.Models;

namespace PetOS.Repositories;

public class RotinaRepository(AppDbContext context) : IRotinaRepository
{
    public async Task<IReadOnlyList<Rotina>> GetAllAsync(int? petId, bool? ativa)
    {
        var query = context.Rotinas.AsNoTracking().AsQueryable();

        if (petId.HasValue)
        {
            query = query.Where(r => r.PetId == petId.Value);
        }

        if (ativa.HasValue)
        {
            query = query.Where(r => r.Ativa == ativa.Value);
        }

        return await query.OrderBy(r => r.Horario).ToListAsync();
    }

    public Task<Rotina?> GetByIdAsync(int id) =>
        context.Rotinas.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);

    public async Task<Rotina> AddAsync(Rotina rotina)
    {
        context.Rotinas.Add(rotina);
        await context.SaveChangesAsync();
        return rotina;
    }

    public async Task UpdateAsync(Rotina rotina)
    {
        context.Rotinas.Update(rotina);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Rotina rotina)
    {
        context.Rotinas.Remove(rotina);
        await context.SaveChangesAsync();
    }
}

