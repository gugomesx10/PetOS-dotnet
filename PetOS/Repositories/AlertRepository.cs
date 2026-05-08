using PetOS.Models;
using PetOS.Repositories.Interfaces;
using PetOS.Data;
using Microsoft.EntityFrameworkCore;

namespace PetOS.Repositories;

public class AlertRepository : IAlertRepository
{
    private readonly AppDbContext _context;
    
    public AlertRepository (AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Alert>> GetAllAsync()
    {
        return await _context.Alerts.ToListAsync();
    }

    public async Task<Alert?> GetByIdAsync(long id)
    {
        return await _context.Alerts.FindAsync(id);
    }

    public async Task<IEnumerable<Alert>> GetUnreadAsync()
    {
        return await _context.Alerts
            .Where(a => !a.IsRead)
            .ToListAsync();
    }

    public async Task AddAsync(Alert alert)
    {
        await _context.Alerts.AddAsync(alert);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Alert alert)
    {
        _context.Alerts.Update(alert);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var alert = await _context.Alerts.FindAsync(id);

        if (alert != null)
        {
            _context.Alerts.Remove(alert);
            await _context.SaveChangesAsync();
        }
    }
}