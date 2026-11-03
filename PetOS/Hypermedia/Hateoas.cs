using Microsoft.AspNetCore.WebUtilities;
using PetOS.Dto.Common;

namespace PetOS.Hypermedia;

/// <summary>
/// Representa um link de navegação da API.
/// </summary>
public sealed record ApiLink(
    string Rel,
    string Href,
    string Method);

/// <summary>
/// Constrói respostas com links HATEOAS.
/// </summary>
public static class Hateoas
{
    public static object Item<T>(
        string resource,
        long id,
        T data,
        string message)
    {
        return new
        {
            message,
            data,
            links = ForItem(resource, id)
        };
    }

    public static object Page<T>(
        HttpRequest request,
        string resource,
        PagedResult<T> result,
        string message)
    {
        return new
        {
            message,
            data = result.Items,
            pagination = new
            {
                page = result.Page,
                pageSize = result.PageSize,
                totalItems = result.TotalCount,
                totalPages = result.TotalPages,
                hasNext = result.Page < result.TotalPages,
                hasPrevious = result.Page > 1
            },
            links = ForPage(
                request,
                resource,
                result.Page,
                result.TotalPages)
        };
    }

    public static IReadOnlyList<ApiLink> ForItem(
        string resource,
        long id)
    {
        var path = $"/api/{resource}/{id}";

        return new[]
        {
            new ApiLink("self", path, "GET"),
            new ApiLink(
                "collection",
                $"/api/{resource}",
                "GET"),
            new ApiLink("update", path, "PUT"),
            new ApiLink("delete", path, "DELETE")
        };
    }

    public static IReadOnlyList<ApiLink> ForCollection(
        string resource)
    {
        return new[]
        {
            new ApiLink(
                "collection",
                $"/api/{resource}",
                "GET"),
            new ApiLink(
                "create",
                $"/api/{resource}",
                "POST")
        };
    }

    private static IReadOnlyList<ApiLink> ForPage(
        HttpRequest request,
        string resource,
        int page,
        int totalPages)
    {
        var links = new List<ApiLink>
        {
            new(
                "self",
                $"{request.PathBase}{request.Path}{request.QueryString}",
                "GET"),

            new(
                "create",
                $"/api/{resource}",
                "POST")
        };

        if (page > 1)
        {
            links.Add(new ApiLink(
                "previous",
                PageUrl(request, page - 1),
                "GET"));
        }

        if (page < totalPages)
        {
            links.Add(new ApiLink(
                "next",
                PageUrl(request, page + 1),
                "GET"));
        }

        return links;
    }

    private static string PageUrl(HttpRequest request, int page)
    {
        var query = request.Query.ToDictionary(
            item => item.Key,
            item => (string?)item.Value.ToString(),
            StringComparer.OrdinalIgnoreCase
        );

        query["page"] = page.ToString();

        return QueryHelpers.AddQueryString(
            $"{request.PathBase}{request.Path}",
            query
        );
    }
    
    public static object Item<T>(
        string resource,
        string id,
        T data,
        string message) => new
    {
        message,
        data,
        links = ForItem(resource, id)
    };

    public static IReadOnlyList<ApiLink> ForItem(
        string resource,
        string id)
    {
        var path = $"/api/{resource}/{Uri.EscapeDataString(id)}";

        return new[]
        {
            new ApiLink("self", path, "GET"),
            new ApiLink("collection", $"/api/{resource}", "GET"),
            new ApiLink("update", path, "PUT"),
            new ApiLink("delete", path, "DELETE")
        };
    }
}