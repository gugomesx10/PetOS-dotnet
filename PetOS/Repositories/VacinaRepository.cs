using Microsoft.EntityFrameworkCore;
using PetOS.Data;
using PetOS.Models;

namespace PetOS.Repositories;

public class VacinaRepository(AppDbContext context) : IVacinaRepository
{
    public async Task<IReadOnlyList<Vacina>> GetAllAsync(int? petId, DateOnly? aplicadaAte)
    {
        var query = context.Vacinas.AsNoTracking().AsQueryable();

        if (petId.HasValue)
        {
            query = query.Where(v => v.PetId == petId.Value);
        }

        if (aplicadaAte.HasValue)
        {
            query = query.Where(v => v.DataAplicacao <= aplicadaAte.Value);
        }

        return await query.OrderByDescending(v => v.DataAplicacao).ToListAsync();
    }

    public Task<Vacina?> GetByIdAsync(int id) =>
        context.Vacinas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);

    public async Task<Vacina> AddAsync(Vacina vacina)
    {
        context.Vacinas.Add(vacina);
        await context.SaveChangesAsync();
        return vacina;
    }

    public async Task UpdateAsync(Vacina vacina)
    {
        context.Vacinas.Update(vacina);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Vacina vacina)
    {
        context.Vacinas.Remove(vacina);
        await context.SaveChangesAsync();
    }
}

