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
}