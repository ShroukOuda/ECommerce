namespace ECommerce.Application.DTO.Common;

public class ApiResponse<T> : ApiResponse
{
    
    public T? Data { get; init; }

    public ApiResponse(
        bool success,
        string message,
        T? data = default,
        IReadOnlyList<ApiError>? errors = null)
        : base(success, message, errors)
    {
        Data = data;
    }
}