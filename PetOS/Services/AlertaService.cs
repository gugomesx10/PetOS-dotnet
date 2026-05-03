using PetOS.DTOs.Alertas;
using PetOS.Exceptions;
using PetOS.Mappings;
using PetOS.Repositories;

namespace PetOS.Services;

public class AlertaService(IAlertaRepository alertaRepository, IPetRepository petRepository, IRotinaRepository rotinaRepository) : IAlertaService
{
    public async Task<IReadOnlyList<AlertaResponseDto>> GetAllAsync(int? petId, Models.AlertaStatus? status, DateTime? ate)
    {
        var alertas = await alertaRepository.GetAllAsync(petId, status, ate);
        return alertas.Select(a => a.ToDto()).ToList();
    }

    public async Task<AlertaResponseDto> GetByIdAsync(int id)
    {
        var alerta = await alertaRepository.GetByIdAsync(id) ??
                     throw new NotFoundException($"Alerta {id} nao encontrado.");

        return alerta.ToDto();
    }

    public async Task<AlertaResponseDto> CreateAsync(CreateAlertaDto dto)
    {
        if (!await petRepository.ExistsAsync(dto.PetId))
        {
            throw new ArgumentException($"Pet {dto.PetId} nao existe.");
        }

        if (dto.RotinaId.HasValue && await rotinaRepository.GetByIdAsync(dto.RotinaId.Value) is null)
        {
            throw new ArgumentException($"Rotina {dto.RotinaId.Value} nao existe.");
        }

        var alerta = await alertaRepository.AddAsync(dto.ToEntity());
        return alerta.ToDto();
    }

    public async Task UpdateAsync(int id, UpdateAlertaDto dto)
    {
        var alerta = await alertaRepository.GetByIdAsync(id) ??
                     throw new NotFoundException($"Alerta {id} nao encontrado.");

        if (dto.RotinaId.HasValue && await rotinaRepository.GetByIdAsync(dto.RotinaId.Value) is null)
        {
            throw new ArgumentException($"Rotina {dto.RotinaId.Value} nao existe.");
        }

        alerta.RotinaId = dto.RotinaId;
        alerta.Titulo = dto.Titulo;
        alerta.Mensagem = dto.Mensagem;
        alerta.DataAlerta = dto.DataAlerta;
        alerta.Status = dto.Status;

        await alertaRepository.UpdateAsync(alerta);
    }

    public async Task DeleteAsync(int id)
    {
        var alerta = await alertaRepository.GetByIdAsync(id) ??
                     throw new NotFoundException($"Alerta {id} nao encontrado.");

        await alertaRepository.DeleteAsync(alerta);
    }
}

