namespace E_Commerece.Api.Helper;

public class ResponseAPI
{
    public ResponseAPI(int statusCode, string message = null)
    {
        StatusCode = statusCode;
        Message = message ?? GetMessageFromStatusCode(StatusCode);
    }

    private string GetMessageFromStatusCode(int statusCode)
    {
        return statusCode switch
        {
            200 => "Success",
            400 => "Bad Request",
            401 => "Unauthorized",
            404 => "Not Found",
            500 =>  "InternalServerError",
            _ => "UnKnown Error"
            
        };
        // switch (statusCode)
        // {
        //     case 200:
        //         return "Success";
        //     case 400: 
        //         return "Bad Request";
        //     case 401:
        //         return "Unauthorized";
        //     case 500:
        //         return "Internal Server Error";
        // }
        //
        // return "Unknown Error";
    }
    public int StatusCode { get; set; }
    public string? Message { get; set; }
}