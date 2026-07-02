namespace ECommerce.Application.DTO.Pagination;

public record PaginatedResult<T>(IReadOnlyList<T> Items, int TotalCount, int PageNumber, int PageSize);