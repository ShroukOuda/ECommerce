namespace ECommerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BaseController : ControllerBase
{
   protected IActionResult ApiResponse(int statusCode = 200, string? message = null)
   {
      var response = new ResponseAPI(statusCode, message);
      return StatusCode(statusCode, response);
   }

   protected IActionResult BadRequestResponse()
   {
      return ApiResponse(400, null);
   }

   protected IActionResult InternalServerErrorResponse()
   {
      return ApiResponse(500, null);
   }

   protected IActionResult SuccessResponse()
   {
      return ApiResponse(200, null);
   }

   protected IActionResult NotFoundResponse()
   {
      return ApiResponse(404, null);
   }
   
   protected IActionResult UnAuthorizedResponse()
   {
      return ApiResponse(401, null);
   }
}