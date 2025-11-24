using AutoMapper;
using E_Commerece.Api.Helper;
using E_Commerece.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerece.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BaseController : ControllerBase
{
   protected IActionResult ApiResponse(int statusCode = 200, string? message = null, string? data = null)
   {
      var response = new ResponseAPI(statusCode, message, data);
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

   protected IActionResult SuccessResponse(string data)
   {
      return ApiResponse(200, null, data);
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