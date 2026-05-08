using PetOS.Dto.Alert;
using PetOS.Models;
using PetOS.Repositories.Interfaces;
using PetOS.Services.Interfaces;

namespace PetOS.Services;

public class AlertService : IAlertService
{
    private readonly IAlertRepository _repository;
    
    public AlertService(IAlertRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<AlertResponseDto>> GetAllAsync()
    {
        var alerts = await _repository.GetAllAsync();

        return alerts.Select(a => new AlertResponseDto
        {
            Id = a.Id,
            PetId = a.PetId,
            VaccineId = a.VaccineId,
            Message = a.Message,
            AlertDate = a.AlertDate,
            IsRead = a.IsRead,
            CreatedAt = a.CreatedAt,
        });
    }

    public async Task<AlertResponseDto?> GetByIdAsync(long id)
    {
        var alert = await _repository.GetByIdAsync(id);

        if (alert == null)
            return null;

        return new AlertResponseDto
        {
            Id = alert.Id,
            PetId = alert.PetId,
            VaccineId = alert.VaccineId,
            Message = alert.Message,
            AlertDate = alert.AlertDate,
            IsRead = alert.IsRead,
            CreatedAt = alert.CreatedAt,
        };
    }

    public async Task<IEnumerable<AlertResponseDto>> GetUnreadAsync()
    {
        var alerts = await _repository.GetUnreadAsync();

        return alerts.Select(a => new AlertResponseDto
        {
            Id = a.Id,
            PetId = a.PetId,
            VaccineId = a.VaccineId,
            Message = a.Message,
            AlertDate = a.AlertDate,
            IsRead = a.IsRead,
            CreatedAt = a.CreatedAt,
        });
    }

    public async Task<AlertResponseDto> CreateAsync(AlertCreateDto dto)
    {
        var alert = new Alert
        {
            PetId = dto.PetId,
            VaccineId = dto.VaccineId,
            Message = dto.Message,
            AlertDate = dto.AlertDate,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };
        
        await _repository.AddAsync(alert);

        return new AlertResponseDto
        {
            Id = alert.Id,
            PetId = alert.PetId,
            VaccineId = alert.VaccineId,
            Message = alert.Message,
            AlertDate = alert.AlertDate,
            IsRead = alert.IsRead,
            CreatedAt = alert.CreatedAt,
        };
    }
    
    public async Task UpdateAsync(long id, AlertCreateDto dto)
    {
        var alert = await _repository.GetByIdAsync(id);

        if (alert == null)
            return;
        
        alert.PetId = dto.PetId;
        alert.VaccineId = dto.VaccineId;
        alert.Message = dto.Message;
        alert.AlertDate = dto.AlertDate;
        
        await _repository.UpdateAsync(alert);
    }
    
    public async Task DeleteAsync(long id)
    {
        await _repository.DeleteAsync(id);
    }
}