using PetOS.Dto.Vaccine;
using PetOS.Models;
using PetOS.Repositories.Interfaces;
using PetOS.Services.Interfaces;
using PetOS.Observability;

namespace PetOS.Services;

public class VaccineService : IVaccineService
{
    private readonly IVaccineRepository _repository;
    
    public VaccineService(IVaccineRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<VaccineResponseDto>> GetAllAsync()
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("VaccineService.GetAllAsync");
        
        var vaccines = await _repository.GetAllAsync();

        return vaccines.Select(v => new VaccineResponseDto()
        {
            Id = v.Id,
            PetId = v.PetId,
            Name = v.Name,
            Manufacturer = v.Manufacturer,
            ApplicationDate = v.ApplicationDate,
            NextDueDate = v.NextDueDate,
            Dose = v.Dose,
            CreatedAt = v.CreateAt
        });
    }

    public async Task<VaccineResponseDto?> GetByIdAsync(long id)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("VaccineService.GetByIdAsync");
        activity?.SetTag("vaccine.id", id);
        var vaccine = await _repository.GetByIdAsync(id);

        if (vaccine == null)
            return null;
        
        return new VaccineResponseDto
        {
            Id = vaccine.Id,
            PetId = vaccine.PetId,
            Name = vaccine.Name,
            Manufacturer = vaccine.Manufacturer,
            ApplicationDate = vaccine.ApplicationDate,
            NextDueDate = vaccine.NextDueDate,
            Dose = vaccine.Dose,
            CreatedAt = vaccine.CreateAt
        };
    }

    public async Task<IEnumerable<VaccineResponseDto>> GetByPetIdAsync(long petId)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("VaccineService.GetByPetIdAsync");

        activity?.SetTag("pet.id", petId);
        var vaccines = await _repository.GetByPetIdAsync(petId);

        return vaccines.Select(v => new VaccineResponseDto
        {
            Id = v.Id,
            PetId = v.PetId,
            Name = v.Name,
            Manufacturer = v.Manufacturer,
            ApplicationDate = v.ApplicationDate,
            NextDueDate = v.NextDueDate,
            Dose = v.Dose,
            CreatedAt = v.CreateAt
        });
    }

    public async Task<VaccineResponseDto> CreateAsync(VaccineCreateDto dto)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("VaccineService.CreateAsync");

        activity?.SetTag("pet.id", dto.PetId);
        var vaccine = new Vaccine
        {
            PetId = dto.PetId,
            Name = dto.Name,
            Manufacturer = dto.Manufacturer,
            ApplicationDate = dto.ApplicationDate,
            NextDueDate = dto.NextDueDate,
            Dose = dto.Dose,
            CreateAt = DateTime.UtcNow
        };
        
        await _repository.AddAsync(vaccine);
        return new VaccineResponseDto
        {
            Id = vaccine.Id,
            PetId = vaccine.PetId,
            Name = vaccine.Name,
            Manufacturer = vaccine.Manufacturer,
            ApplicationDate = vaccine.ApplicationDate,
            NextDueDate = vaccine.NextDueDate,
            Dose = vaccine.Dose,
            CreatedAt = vaccine.CreateAt
        };
    }

    public async Task UpdateAsync(long id, VaccineCreateDto dto)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("VaccineService.UpdateAsync");

        activity?.SetTag("vaccine.id", id);
        var vaccine = await _repository.GetByIdAsync(id);

        if (vaccine == null)
            return;
        
        vaccine.PetId = dto.PetId;
        vaccine.Name = dto.Name;
        vaccine.Manufacturer = dto.Manufacturer;
        vaccine.ApplicationDate = dto.ApplicationDate;
        vaccine.NextDueDate = dto.NextDueDate;
        vaccine.Dose = dto.Dose;
        
        await _repository.UpdateAsync(vaccine);
    }

    public async Task DeleteAsync(long id)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("VaccineService.DeleteAsync");

        activity?.SetTag("vaccine.id", id);
        await _repository.DeleteAsync(id);
    }
}