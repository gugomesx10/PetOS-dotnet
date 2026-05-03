using PetOS.DTOs.Rotinas;
using PetOS.Exceptions;
using PetOS.Mappings;
using PetOS.Repositories;

namespace PetOS.Services;

public class RotinaService(IRotinaRepository rotinaRepository, IPetRepository petRepository) : IRotinaService
{
    public async Task<IReadOnlyList<RotinaResponseDto>> GetAllAsync(int? petId, bool? ativa)
    {
        var rotinas = await rotinaRepository.GetAllAsync(petId, ativa);
        return rotinas.Select(r => r.ToDto()).ToList();
    }

    public async Task<RotinaResponseDto> GetByIdAsync(int id)
    {
        var rotina = await rotinaRepository.GetByIdAsync(id) ??
                     throw new NotFoundException($"Rotina {id} nao encontrada.");

        return rotina.ToDto();
    }

    public async Task<RotinaResponseDto> CreateAsync(CreateRotinaDto dto)
    {
        if (!await petRepository.ExistsAsync(dto.PetId))
        {
            throw new ArgumentException($"Pet {dto.PetId} nao existe.");
        }

        var rotina = await rotinaRepository.AddAsync(dto.ToEntity());
        return rotina.ToDto();
    }

    public async Task UpdateAsync(int id, UpdateRotinaDto dto)
    {
        var rotina = await rotinaRepository.GetByIdAsync(id) ??
                     throw new NotFoundException($"Rotina {id} nao encontrada.");

        rotina.Titulo = dto.Titulo;
        rotina.Descricao = dto.Descricao;
        rotina.Horario = dto.Horario;
        rotina.Tipo = dto.Tipo;
        rotina.Frequencia = dto.Frequencia;
        rotina.Ativa = dto.Ativa;

        await rotinaRepository.UpdateAsync(rotina);
    }

    public async Task DeleteAsync(int id)
    {
        var rotina = await rotinaRepository.GetByIdAsync(id) ??
                     throw new NotFoundException($"Rotina {id} nao encontrada.");

        await rotinaRepository.DeleteAsync(rotina);
    }
}

