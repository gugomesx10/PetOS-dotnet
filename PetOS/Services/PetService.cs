using PetOS.Dto.Pet;
using PetOS.Models;
using PetOS.Repositories.Interfaces;
using PetOS.Services.Interfaces;

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

    public async Task<PetResponseDto> CreateAsync(PetCreateDto dto)
    {
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
        await _repository.DeleteAsync(id);
    }
}