using PetOS.DTOs.Rotinas;
using PetOS.Models;

namespace PetOS.Mappings;

public static class RotinaMappings
{
    public static RotinaResponseDto ToDto(this Rotina rotina) =>
        new(
            rotina.Id,
            rotina.PetId,
            rotina.Titulo,
            rotina.Descricao,
            rotina.Horario,
            rotina.Tipo,
            rotina.Frequencia,
            rotina.Ativa);

    public static Rotina ToEntity(this CreateRotinaDto dto) =>
        new()
        {
            PetId = dto.PetId,
            Titulo = dto.Titulo,
            Descricao = dto.Descricao,
            Horario = dto.Horario,
            Tipo = dto.Tipo,
            Frequencia = dto.Frequencia,
            Ativa = dto.Ativa
        };
}

