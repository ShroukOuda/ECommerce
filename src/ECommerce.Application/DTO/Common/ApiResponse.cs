namespace ECommerce.Application.DTO.Common;

public class ApiResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;


    public IReadOnlyList<ApiError>? Errors { get; init; }

    public ApiResponse(
        bool success,
        string message,
        IReadOnlyList<ApiError>? errors = null)
    {
        Success = success;
        Message = message;
        Errors = errors;
    }
}
