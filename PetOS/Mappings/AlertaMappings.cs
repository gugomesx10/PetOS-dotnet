using PetOS.DTOs.Alertas;
using PetOS.Models;

namespace PetOS.Mappings;

public static class AlertaMappings
{
    public static AlertaResponseDto ToDto(this Alerta alerta) =>
        new(
            alerta.Id,
            alerta.PetId,
            alerta.RotinaId,
            alerta.Titulo,
            alerta.Mensagem,
            alerta.DataAlerta,
            alerta.Status);

    public static Alerta ToEntity(this CreateAlertaDto dto) =>
        new()
        {
            PetId = dto.PetId,
            RotinaId = dto.RotinaId,
            Titulo = dto.Titulo,
            Mensagem = dto.Mensagem,
            DataAlerta = dto.DataAlerta,
            Status = dto.Status
        };
}

