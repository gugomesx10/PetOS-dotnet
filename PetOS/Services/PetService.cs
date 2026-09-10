using PetOS.Dto.Pet;
using PetOS.Models;
using PetOS.Repositories.Interfaces;
using PetOS.Services.Interfaces;
using PetOS.Observability;

namespace PetOS.Services;

public class PetService : IPetService
{
    private readonly IPetRepository _repository;
    public PetService(IPetRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PetResponseDto>> GetAllSync()
    {
        
        using var activity = PetOsTelemetry.ActivitySource.StartActivity("PetService.GetAllSync");
        
        var pets = await _repository.GetAllAsync();

        return pets.Select(p => new PetResponseDto()
        {
            Id = p.Id,
            Name = p.Name,
            Species = p.Species,
            Breed = p.Breed,
            BirthDate = p.BirthDate,
            Gender = p.Gender,
            weight = p.Weight,
            CreatedAt = p.CreatedAt,
        });
    }

    public async Task<PetResponseDto?> GetByIdAsync(long id)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("PetService.GetByIdAsync");

        activity?.SetTag("pet.id", id);
        var pet = await _repository.GetByIdAsync(id);

        if (pet == null)
            return null;

        return new PetResponseDto
        {
            Id = pet.Id,
            Name = pet.Name,
            Species = pet.Species,
            Breed = pet.Breed,
            BirthDate = pet.BirthDate,
            Gender = pet.Gender,
            weight = pet.Weight,
            CreatedAt = pet.CreatedAt,
        };
    }

    public async Task<IEnumerable<PetResponseDto>> GetBySpecieAsync(string species)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("PetService.GetBySpecieAsync");

        activity?.SetTag("pet.species", species);
        var pets = await _repository.GetBySpeciesAsync(species);

        return pets.Select(p => new PetResponseDto
        {
            Id = p.Id,
            Name = p.Name,
            Species = p.Species,
            Breed = p.Breed,
            BirthDate = p.BirthDate,
            Gender = p.Gender,
            weight = p.Weight,
            CreatedAt = p.CreatedAt,
        });
    }

    public async Task<IEnumerable<PetResponseDto>> GetByNameAsync(string name)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("PetService.GetByNameAsync");

        activity?.SetTag("pet.name", name);
        var pets = await _repository.GetByNameAsync(name);

        return pets.Select(p => new PetResponseDto
        {
            Id = p.Id,
            Name = p.Name,
            Species = p.Species,
            Breed = p.Breed,
            BirthDate = p.BirthDate,
            Gender = p.Gender,
            weight = p.Weight,
            CreatedAt = p.CreatedAt,
        });
    }

    public async Task<PetResponseDto> CreateAsync(PetCreateDto dto)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("PetService.CreateAsync");

        activity?.SetTag("pet.species", dto.Species);
        var pet = new Pet
        {
            Name = dto.Name,
            Species = dto.Species,
            Breed = dto.Breed,
            BirthDate = dto.BirthDate,
            Gender = dto.Gender,
            Weight = dto.weight,
            CreatedAt = DateTime.UtcNow,
        };

        await _repository.AddAsync(pet);
        return new PetResponseDto()
        {
            Id = pet.Id,
            Name = pet.Name,
            Species = pet.Species,
            Breed = pet.Breed,
            BirthDate = pet.BirthDate,
            Gender = pet.Gender,
            weight = pet.Weight,
            CreatedAt = pet.CreatedAt,
        };
    }

    public async Task UpdateAsync(long id, PetCreateDto dto)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("PetService.UpdateAsync");

        activity?.SetTag("pet.id", id);
        var pet = await _repository.GetByIdAsync(id);

        if (pet == null)
            return; 
        
        pet.Name = dto.Name;
        pet.Species = dto.Species;
        pet.Breed = dto.Breed;
        pet.BirthDate = dto.BirthDate;
        pet.Gender = dto.Gender;
        pet.Weight = dto.weight;

        await _repository.UpdateAsync(pet);
    }
    
    public async Task DeleteAsync(long id)
    {
        using var activity =
            PetOsTelemetry.ActivitySource.StartActivity("PetService.DeleteAsync");

        activity?.SetTag("pet.id", id);
        await _repository.DeleteAsync(id);
    }
}