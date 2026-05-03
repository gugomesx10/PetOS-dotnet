using Microsoft.EntityFrameworkCore;
using PetOS.Data;
using PetOS.Models;

namespace PetOS.Repositories;

public class AlertaRepository(AppDbContext context) : IAlertaRepository
{
    public async Task<IReadOnlyList<Alerta>> GetAllAsync(int? petId, AlertaStatus? status, DateTime? ate)
    {
        var query = context.Alertas.AsNoTracking().AsQueryable();

        if (petId.HasValue)
        {
            query = query.Where(a => a.PetId == petId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(a => a.Status == status.Value);
        }

        if (ate.HasValue)
        {
            query = query.Where(a => a.DataAlerta <= ate.Value);
        }

        return await query.OrderBy(a => a.DataAlerta).ToListAsync();
    }

    public Task<Alerta?> GetByIdAsync(int id) =>
        context.Alertas.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);

    public async Task<Alerta> AddAsync(Alerta alerta)
    {
        context.Alertas.Add(alerta);
        await context.SaveChangesAsync();
        return alerta;
    }

    public async Task UpdateAsync(Alerta alerta)
    {
        context.Alertas.Update(alerta);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Alerta alerta)
    {
        context.Alertas.Remove(alerta);
        await context.SaveChangesAsync();
    }
}

