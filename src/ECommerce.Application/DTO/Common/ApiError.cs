namespace ECommerce.Application.DTO.Common;

public class ApiError
{
    public string Code { get; init; } = string.Empty;

    public string? Field { get; init; }

    public string Message { get; init; } = string.Empty;
}