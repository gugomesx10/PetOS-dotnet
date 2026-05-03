using PetOS.DTOs.Pets;
using PetOS.Exceptions;
using PetOS.Mappings;
using PetOS.Repositories;

namespace PetOS.Services;

public class PetService(IPetRepository petRepository) : IPetService
{
    public async Task<IReadOnlyList<PetResponseDto>> GetAllAsync(string? especie, string? responsavel)
    {
        var pets = await petRepository.GetAllAsync(especie, responsavel);
        return pets.Select(p => p.ToDto()).ToList();
    }

    public async Task<PetResponseDto> GetByIdAsync(int id)
    {
        var pet = await petRepository.GetByIdAsync(id) ??
                  throw new NotFoundException($"Pet {id} nao encontrado.");

        return pet.ToDto();
    }

    public async Task<PetResponseDto> CreateAsync(CreatePetDto dto)
    {
        var pet = await petRepository.AddAsync(dto.ToEntity());
        return pet.ToDto();
    }

    public async Task UpdateAsync(int id, UpdatePetDto dto)
    {
        var pet = await petRepository.GetByIdAsync(id) ??
                  throw new NotFoundException($"Pet {id} nao encontrado.");

        pet.Nome = dto.Nome;
        pet.Especie = dto.Especie;
        pet.Raca = dto.Raca;
        pet.DataNascimento = dto.DataNascimento;
        pet.ResponsavelNome = dto.ResponsavelNome;
        pet.ResponsavelTelefone = dto.ResponsavelTelefone;

        await petRepository.UpdateAsync(pet);
    }

    public async Task DeleteAsync(int id)
    {
        var pet = await petRepository.GetByIdAsync(id) ??
                  throw new NotFoundException($"Pet {id} nao encontrado.");

        await petRepository.DeleteAsync(pet);
    }
}

