using PetOS.DTOs.Vacinas;
using PetOS.Models;

namespace PetOS.Mappings;

public static class VacinaMappings
{
    public static VacinaResponseDto ToDto(this Vacina vacina) =>
        new(
            vacina.Id,
            vacina.PetId,
            vacina.Nome,
            vacina.DataAplicacao,
            vacina.ProximaDose,
            vacina.Observacoes);

    public static Vacina ToEntity(this CreateVacinaDto dto) =>
        new()
        {
            PetId = dto.PetId,
            Nome = dto.Nome,
            DataAplicacao = dto.DataAplicacao,
            ProximaDose = dto.ProximaDose,
            Observacoes = dto.Observacoes
        };
}

