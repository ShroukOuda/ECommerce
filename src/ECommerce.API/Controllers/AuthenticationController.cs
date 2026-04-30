using ECommerce.Application.DTO.Authentication;

namespace ECommerce.API.Controllers;

public class AuthenticationController : BaseController
{
    private readonly IAuthenticationService _authenticationService;
    
    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }
    
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
    {        
        var result = await _authenticationService.RegisterAsync(registerDTO);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
    {
        var result = await _authenticationService.LoginAsync(loginDTO);
        return Ok(result);
    }
}