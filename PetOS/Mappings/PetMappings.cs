using PetOS.DTOs.Pets;
using PetOS.Models;

namespace PetOS.Mappings;

public static class PetMappings
{
    public static PetResponseDto ToDto(this Pet pet) =>
        new(
            pet.Id,
            pet.Nome,
            pet.Especie,
            pet.Raca,
            pet.DataNascimento,
            pet.ResponsavelNome,
            pet.ResponsavelTelefone,
            pet.CriadoEm);

    public static Pet ToEntity(this CreatePetDto dto) =>
        new()
        {
            Nome = dto.Nome,
            Especie = dto.Especie,
            Raca = dto.Raca,
            DataNascimento = dto.DataNascimento,
            ResponsavelNome = dto.ResponsavelNome,
            ResponsavelTelefone = dto.ResponsavelTelefone
        };
}

