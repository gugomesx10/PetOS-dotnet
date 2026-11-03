using MongoDB.Driver;
using PetOS.Dto.Common;
using PetOS.Dto.PetActivity;
using PetOS.Models;
using PetOS.Repositories.Interfaces;

namespace PetOS.Repositories;

/// <summary>
/// Implementa as operações de persistência das atividades no MongoDB.
/// </summary>
public class PetActivityRepository : IPetActivityRepository
{
    private readonly IMongoCollection<PetActivity> _collection;

    public PetActivityRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<PetActivity>(
            "pet_activities"
        );
    }

    public async Task<PagedResult<PetActivity>> GetPagedAsync(
        PetActivityQuery query)
    {
        var filterBuilder = Builders<PetActivity>.Filter;

        var filter = filterBuilder.Empty;

        // Filtro por pet
        if (query.PetId.HasValue)
        {
            filter = filterBuilder.And(
                filter,
                filterBuilder.Eq(
                    activity => activity.PetId,
                    query.PetId.Value
                )
            );
        }

        // Filtro por tipo de atividade
        if (!string.IsNullOrWhiteSpace(query.Type))
        {
            filter = filterBuilder.And(
                filter,
                filterBuilder.Eq(
                    activity => activity.Type,
                    query.Type.Trim()
                )
            );
        }

        // Lista de campos permitidos para ordenação
        var sortField = (query.SortBy ?? string.Empty)
            .Trim()
            .ToLowerInvariant() switch
        {
            "id" => "_id",
            "petid" => "petId",
            "type" => "type",
            "createdatutc" => "createdAtUtc",

            _ => throw new ArgumentException(
                "Ordenação inválida. Utilize id, petId, type ou createdAtUtc."
            )
        };

        var sortBuilder = Builders<PetActivity>.Sort;

        SortDefinition<PetActivity> sort =
            string.Equals(
                query.SortDirection,
                "desc",
                StringComparison.OrdinalIgnoreCase
            )
                ? sortBuilder.Descending(sortField)
                : sortBuilder.Ascending(sortField);

        // Ordenação estável para a paginação
        if (sortField != "_id")
        {
            sort = sortBuilder.Combine(
                sort,
                sortBuilder.Ascending("_id")
            );
        }

        // Total de registros encontrados
        var totalCount = await _collection.CountDocumentsAsync(
            filter
        );

        var skip = (query.Page - 1) * query.PageSize;

        // Consulta paginada
        var activities = await _collection
            .Find(filter)
            .Sort(sort)
            .Skip(skip)
            .Limit(query.PageSize)
            .ToListAsync();

        return new PagedResult<PetActivity>(
            activities,
            query.Page,
            query.PageSize,
            checked((int)totalCount)
        );
    }

    public async Task<PetActivity?> GetByIdAsync(string id)
    {
        return await _collection
            .Find(activity => activity.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task InsertAsync(PetActivity activity)
    {
        await _collection.InsertOneAsync(activity);
    }

    public async Task<bool> ReplaceAsync(PetActivity activity)
    {
        var result = await _collection.ReplaceOneAsync(
            existing => existing.Id == activity.Id,
            activity
        );

        return result.MatchedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _collection.DeleteOneAsync(
            activity => activity.Id == id
        );

        return result.DeletedCount > 0;
    }
}