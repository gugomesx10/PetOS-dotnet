using MongoDB.Bson;
using PetOS.Dto.Common;
using PetOS.Dto.PetActivity;
using PetOS.Models;
using PetOS.Repositories.Interfaces;
using PetOS.Services.Interfaces;

namespace PetOS.Services;

/// <summary>
/// Gerencia as regras de negócio das atividades dos pets.
/// </summary>
public class PetActivityService : IPetActivityService
{
    private readonly IPetActivityRepository _repository;
    private readonly IPetService _petService;

    public PetActivityService(
        IPetActivityRepository repository,
        IPetService petService)
    {
        _repository = repository;
        _petService = petService;
    }

    public Task<PagedResult<PetActivity>> GetPagedAsync(
        PetActivityQuery query)
    {
        return _repository.GetPagedAsync(query);
    }

    public async Task<PetActivity?> GetByIdAsync(string id)
    {
        ValidateId(id);

        return await _repository.GetByIdAsync(id);
    }

    public async Task<PetActivity> CreateAsync(
        PetActivityCreateDto dto)
    {
        ValidateDto(dto);

        await EnsurePetExistsAsync(dto.PetId);

        var activity = new PetActivity
        {
            PetId = dto.PetId,
            Type = dto.Type.Trim(),
            Description = dto.Description.Trim(),
            Metadata = dto.Metadata == null
                ? new Dictionary<string, string>()
                : new Dictionary<string, string>(dto.Metadata),
            CreatedAtUtc = DateTime.UtcNow
        };

        await _repository.InsertAsync(activity);

        return activity;
    }

    public async Task<bool> UpdateAsync(
        string id,
        PetActivityCreateDto dto)
    {
        ValidateId(id);
        ValidateDto(dto);

        var activity = await _repository.GetByIdAsync(id);

        if (activity == null)
        {
            return false;
        }

        await EnsurePetExistsAsync(dto.PetId);

        activity.PetId = dto.PetId;
        activity.Type = dto.Type.Trim();
        activity.Description = dto.Description.Trim();

        activity.Metadata = dto.Metadata == null
            ? new Dictionary<string, string>()
            : new Dictionary<string, string>(dto.Metadata);

        return await _repository.ReplaceAsync(activity);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        ValidateId(id);

        return await _repository.DeleteAsync(id);
    }

    private async Task EnsurePetExistsAsync(long petId)
    {
        var pet = await _petService.GetByIdAsync(petId);

        if (pet == null)
        {
            throw new KeyNotFoundException(
                "Pet não encontrado."
            );
        }
    }

    private static void ValidateId(string id)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            throw new ArgumentException(
                "O identificador da atividade é inválido.",
                nameof(id)
            );
        }
    }

    private static void ValidateDto(PetActivityCreateDto dto)
    {
        if (dto.PetId <= 0)
        {
            throw new ArgumentException(
                "O identificador do pet deve ser maior que zero."
            );
        }

        if (string.IsNullOrWhiteSpace(dto.Type) ||
            string.IsNullOrWhiteSpace(dto.Description))
        {
            throw new ArgumentException(
                "O tipo e a descrição da atividade são obrigatórios."
            );
        }
    }
}