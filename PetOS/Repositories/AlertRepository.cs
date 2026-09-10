using PetOS.Models;
using PetOS.Repositories.Interfaces;
using PetOS.Data;
using Microsoft.EntityFrameworkCore;
using PetOS.Observability;

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
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("AlertRepository.GetAllAsync");

        return await _context.Alerts.ToListAsync();
    }

    public async Task<Alert?> GetByIdAsync(long id)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("AlertRepository.GetByIdAsync");

        activity?.SetTag("alert.id", id);

        return await _context.Alerts.FindAsync(id);
    }

    public async Task<IEnumerable<Alert>> GetUnreadAsync()
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("AlertRepository.GetUnreadAsync");

        return await _context.Alerts
            .Where(a => a.IsRead == 0)
            .ToListAsync();
    }

    public async Task AddAsync(Alert alert)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("AlertRepository.AddAsync");

        await _context.Alerts.AddAsync(alert);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Alert alert)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("AlertRepository.UpdateAsync");

        activity?.SetTag("alert.id", alert.Id);

        _context.Alerts.Update(alert);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("AlertRepository.DeleteAsync");

        activity?.SetTag("alert.id", id);

        var alert = await _context.Alerts.FindAsync(id);

        if (alert != null)
        {
            _context.Alerts.Remove(alert);
            await _context.SaveChangesAsync();
        }
    }
}