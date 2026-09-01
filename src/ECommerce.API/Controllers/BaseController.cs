using ECommerce.Application.DTO.Common;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected IActionResult Success<T>(
        T data,
        string message = "Request completed successfully.",
        int statusCode = StatusCodes.Status200OK)
      {
         var response = new ApiResponse<T>(
               success: true,
               message: message,
               data: data);

        return StatusCode(statusCode, response);
      }

    protected IActionResult Created<T>(
        T data,
        string message = "Resource created successfully.")
      {
        var response = new ApiResponse<T>(
            success: true,
            message: message,
            data: data);

        return StatusCode(
            StatusCodes.Status201Created,
            response);
      }

      protected IActionResult SuccessMessage(
         string message = "Request completed successfully.",
         int statusCode = StatusCodes.Status204NoContent)
      {
         var response = new ApiResponse(
               success: true,
               message: message);

         return StatusCode(statusCode, response);
      }

      protected IActionResult CreatedMessage(
         string message = "Resource created successfully.")
      {
         var response = new ApiResponse(
               success: true,
               message: message);

         return StatusCode(
             StatusCodes.Status201Created,
             response);
      }
  
}

