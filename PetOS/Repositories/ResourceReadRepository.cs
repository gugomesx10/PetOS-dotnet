using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PetOS.Data;
using PetOS.Dto.Common;
using PetOS.Dto.Pet;
using PetOS.Dto.Vaccine;
using PetOS.Dto.Routine;
using PetOS.Dto.Alert;
using PetOS.Models;
using PetOS.Repositories.Interfaces;

namespace PetOS.Repositories;

/// <summary>
/// Executa consultas paginadas e filtradas dos recursos do PetOS.
/// </summary>
public sealed class ResourceReadRepository : IResourceReadRepository
{
    private readonly AppDbContext _context;

    public ResourceReadRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<PetResponseDto>> GetPetsAsync(
        PetQuery query)
    {
        IQueryable<Pet> source = _context.Pets.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            var name = query.Name.Trim().ToLower();

            source = source.Where(
                p => p.Name.ToLower().Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(query.Species))
        {
            var species = query.Species.Trim().ToLower();

            source = source.Where(
                p => p.Species.ToLower() == species);
        }

        source = (Key(query.SortBy), Descending(query)) switch
        {
            ("id", false) =>
                source.OrderBy(p => p.Id),

            ("id", true) =>
                source.OrderByDescending(p => p.Id),

            ("name", false) =>
                source.OrderBy(p => p.Name).ThenBy(p => p.Id),

            ("name", true) =>
                source.OrderByDescending(p => p.Name).ThenBy(p => p.Id),

            ("species", false) =>
                source.OrderBy(p => p.Species).ThenBy(p => p.Id),

            ("species", true) =>
                source.OrderByDescending(p => p.Species).ThenBy(p => p.Id),

            ("birthdate", false) =>
                source.OrderBy(p => p.BirthDate).ThenBy(p => p.Id),

            ("birthdate", true) =>
                source.OrderByDescending(p => p.BirthDate).ThenBy(p => p.Id),

            ("createdat", false) =>
                source.OrderBy(p => p.CreatedAt).ThenBy(p => p.Id),

            ("createdat", true) =>
                source.OrderByDescending(p => p.CreatedAt).ThenBy(p => p.Id),

            _ => throw new ArgumentException(
                "sortBy inválido para pets. " +
                "Use id, name, species, birthDate ou createdAt.")
        };

        return await PaginateAsync(
            source,
            query,
            p => new PetResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Species = p.Species,
                Breed = p.Breed,
                BirthDate = p.BirthDate,
                Gender = p.Gender,
                weight = p.Weight,
                CreatedAt = p.CreatedAt
            });
    }

    public async Task<PagedResult<VaccineResponseDto>> GetVaccinesAsync(
        VaccineQuery query)
    {
        IQueryable<Vaccine> source =
            _context.Vaccines.AsNoTracking();

        if (query.PetId.HasValue)
        {
            source = source.Where(
                v => v.PetId == query.PetId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            var name = query.Name.Trim().ToLower();

            source = source.Where(
                v => v.Name.ToLower().Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(query.Manufacturer))
        {
            var manufacturer =
                query.Manufacturer.Trim().ToLower();

            source = source.Where(
                v => v.Manufacturer.ToLower().Contains(manufacturer));
        }

        source = (Key(query.SortBy), Descending(query)) switch
        {
            ("id", false) =>
                source.OrderBy(v => v.Id),

            ("id", true) =>
                source.OrderByDescending(v => v.Id),

            ("name", false) =>
                source.OrderBy(v => v.Name).ThenBy(v => v.Id),

            ("name", true) =>
                source.OrderByDescending(v => v.Name).ThenBy(v => v.Id),

            ("applicationdate", false) =>
                source.OrderBy(v => v.ApplicationDate).ThenBy(v => v.Id),

            ("applicationdate", true) =>
                source.OrderByDescending(v => v.ApplicationDate)
                    .ThenBy(v => v.Id),

            ("nextduedate", false) =>
                source.OrderBy(v => v.NextDueDate).ThenBy(v => v.Id),

            ("nextduedate", true) =>
                source.OrderByDescending(v => v.NextDueDate)
                    .ThenBy(v => v.Id),

            _ => throw new ArgumentException(
                "sortBy inválido para vacinas. " +
                "Use id, name, applicationDate ou nextDueDate.")
        };

        return await PaginateAsync(
            source,
            query,
            v => new VaccineResponseDto
            {
                Id = v.Id,
                PetId = v.PetId,
                Name = v.Name,
                Manufacturer = v.Manufacturer,
                ApplicationDate = v.ApplicationDate,
                NextDueDate = v.NextDueDate,
                Dose = v.Dose,
                CreatedAt = v.CreateAt
            });
    }

    public async Task<PagedResult<RoutineResponseDto>> GetRoutinesAsync(
        RoutineQuery query)
    {
        IQueryable<RoutineRecord> source =
            _context.RoutineRecords.AsNoTracking();

        if (query.PetId.HasValue)
        {
            source = source.Where(
                r => r.PetId == query.PetId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Type))
        {
            var type = query.Type.Trim().ToLower();

            source = source.Where(
                r => r.Type.ToLower().Contains(type));
        }

        source = (Key(query.SortBy), Descending(query)) switch
        {
            ("id", false) =>
                source.OrderBy(r => r.Id),

            ("id", true) =>
                source.OrderByDescending(r => r.Id),

            ("type", false) =>
                source.OrderBy(r => r.Type).ThenBy(r => r.Id),

            ("type", true) =>
                source.OrderByDescending(r => r.Type).ThenBy(r => r.Id),

            ("date", false) =>
                source.OrderBy(r => r.Date).ThenBy(r => r.Id),

            ("date", true) =>
                source.OrderByDescending(r => r.Date).ThenBy(r => r.Id),

            _ => throw new ArgumentException(
                "sortBy inválido para rotinas. Use id, type ou date.")
        };

        return await PaginateAsync(
            source,
            query,
            r => new RoutineResponseDto
            {
                Id = r.Id,
                PetId = r.PetId,
                Type = r.Type,
                Description = r.Description,
                Date = r.Date,
                Notes = r.Notes,
                CreatedAt = r.CreatedAt
            });
    }

    public async Task<PagedResult<AlertResponseDto>> GetAlertsAsync(
        AlertQuery query)
    {
        IQueryable<Alert> source = _context.Alerts.AsNoTracking();

        if (query.PetId.HasValue)
        {
            source = source.Where(
                a => a.PetId == query.PetId.Value);
        }

        if (query.IsRead.HasValue)
        {
            var isRead = query.IsRead.Value ? 1 : 0;

            source = source.Where(
                a => a.IsRead == isRead);
        }

        source = (Key(query.SortBy), Descending(query)) switch
        {
            ("id", false) =>
                source.OrderBy(a => a.Id),

            ("id", true) =>
                source.OrderByDescending(a => a.Id),

            ("alertdate", false) =>
                source.OrderBy(a => a.AlertDate).ThenBy(a => a.Id),

            ("alertdate", true) =>
                source.OrderByDescending(a => a.AlertDate)
                    .ThenBy(a => a.Id),

            ("createdat", false) =>
                source.OrderBy(a => a.CreatedAt).ThenBy(a => a.Id),

            ("createdat", true) =>
                source.OrderByDescending(a => a.CreatedAt)
                    .ThenBy(a => a.Id),

            _ => throw new ArgumentException(
                "sortBy inválido para alertas. " +
                "Use id, alertDate ou createdAt.")
        };

        return await PaginateAsync(
            source,
            query,
            a => new AlertResponseDto
            {
                Id = a.Id,
                PetId = a.PetId,
                VaccineId = a.VaccineId,
                Message = a.Message,
                AlertDate = a.AlertDate,
                IsRead = a.IsRead,
                CreatedAt = a.CreatedAt
            });
    }

    private static string Key(string? key)
    {
        return string.IsNullOrWhiteSpace(key)
            ? "id"
            : key.Trim().ToLowerInvariant();
    }

    private static bool Descending(PageQuery query)
    {
        return string.Equals(
            query.SortDirection,
            "desc",
            StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<PagedResult<TResult>> PaginateAsync<T, TResult>(
        IQueryable<T> source,
        PageQuery query,
        Expression<Func<T, TResult>> projection)
    {
        var total = await source.CountAsync();

        var items = await source
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(projection)
            .ToListAsync();

        return new PagedResult<TResult>(
            items,
            query.Page,
            query.PageSize,
            total);
    }
}