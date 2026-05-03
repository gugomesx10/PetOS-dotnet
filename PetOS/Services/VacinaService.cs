using PetOS.DTOs.Vacinas;
using PetOS.Exceptions;
using PetOS.Mappings;
using PetOS.Repositories;

namespace PetOS.Services;

public class VacinaService(IVacinaRepository vacinaRepository, IPetRepository petRepository) : IVacinaService
{
    public async Task<IReadOnlyList<VacinaResponseDto>> GetAllAsync(int? petId, DateOnly? aplicadaAte)
    {
        var vacinas = await vacinaRepository.GetAllAsync(petId, aplicadaAte);
        return vacinas.Select(v => v.ToDto()).ToList();
    }

    public async Task<VacinaResponseDto> GetByIdAsync(int id)
    {
        var vacina = await vacinaRepository.GetByIdAsync(id) ??
                     throw new NotFoundException($"Vacina {id} nao encontrada.");

        return vacina.ToDto();
    }

    public async Task<VacinaResponseDto> CreateAsync(CreateVacinaDto dto)
    {
        if (!await petRepository.ExistsAsync(dto.PetId))
        {
            throw new ArgumentException($"Pet {dto.PetId} nao existe.");
        }

        var vacina = await vacinaRepository.AddAsync(dto.ToEntity());
        return vacina.ToDto();
    }

    public async Task UpdateAsync(int id, UpdateVacinaDto dto)
    {
        var vacina = await vacinaRepository.GetByIdAsync(id) ??
                     throw new NotFoundException($"Vacina {id} nao encontrada.");

        vacina.Nome = dto.Nome;
        vacina.DataAplicacao = dto.DataAplicacao;
        vacina.ProximaDose = dto.ProximaDose;
        vacina.Observacoes = dto.Observacoes;

        await vacinaRepository.UpdateAsync(vacina);
    }

    public async Task DeleteAsync(int id)
    {
        var vacina = await vacinaRepository.GetByIdAsync(id) ??
                     throw new NotFoundException($"Vacina {id} nao encontrada.");

        await vacinaRepository.DeleteAsync(vacina);
    }
}

