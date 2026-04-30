using ECommerce.Application.DTO.Authentication;
using ECommerce.Application.DTO.UserSession;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities.User;


namespace ECommerce.Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IMapper _mapper;
    private readonly IIdentityService _identityService;
    private readonly IUserSessionService _userSessionService;
    private readonly ITokenService _tokenService;
    private readonly IRequestContextService _requestContextService;
    private readonly IPhoneNumberService _phoneNumberService;

    public AuthenticationService(
        IMapper mapper, 
        IIdentityService identityService,
        IUserSessionService userSessionService,
        ITokenService tokenService,
        IRequestContextService requestContextService,
        IPhoneNumberService phoneNumberService)
    {
        _mapper = mapper;
        _identityService = identityService;
        _userSessionService = userSessionService;
        _tokenService = tokenService;
        _requestContextService = requestContextService;
        _phoneNumberService = phoneNumberService;
    }

    public async Task<UserResultDto> RegisterAsync(RegisterDTO registerDto)
    {
        if (string.IsNullOrWhiteSpace(registerDto.CountryCode) || 
            registerDto.CountryCode.Length != 2 || 
            !registerDto.CountryCode.All(char.IsUpper))
        {
            throw new ArgumentException("Country code must be a valid ISO 3166-1 alpha-2 code (2 uppercase letters).");
        }

        bool isPhoneValid = _phoneNumberService.IsValid(registerDto.PhoneNumber, registerDto.CountryCode);
        if (isPhoneValid == false)
        {
            throw new ArgumentException("Invalid phone number format for the specified country code.");
        }
        
        var existingEmail = await _identityService.FindByEmailAsync(registerDto.Email);

        if (existingEmail != null)
        {
            throw new ArgumentException("Email already exists.");
        }

        var existingPhone = await _identityService.FindByPhoneNumberAsync(registerDto.PhoneNumber);
        if (existingPhone != null)
        {
            throw new ArgumentException("Phone number already exists.");
        }

        var user = new User();
        _mapper.Map(registerDto, user);
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        
        var result = await _identityService.CreateUserAsync(user, registerDto.Password);
        if (!result.Success)
        {
            throw new ArgumentException(string.Join("; ", result.Errors));
        }

        await _identityService.AddToRoleAsync(user, "Customer");

        var sessionDto = new AddUserSessionDTO
        {
            UserId = user.Id,
            IpAddress = _requestContextService.GetIpAddress(),
            UserAgent = _requestContextService.GetUserAgent(),
            ExpiresAt = DateTime.UtcNow.AddDays(30) 
        };

        await _userSessionService.AddSessionAsync(sessionDto);
        
        return new UserResultDto
        {
            Success = true,
            Message = "User registered successfully.",
            UserId = user.Id,
            Name = $"{user.FirstName} {user.LastName}",
            Email = user.Email,
            Token = await _tokenService.GenerateTokenAsync(user),
            RefreshToken = await _tokenService.GenerateRefreshTokenAsync()
        };
    }

    public async Task<UserResultDto> LoginAsync(LoginDTO loginDto)
    {
        var user = await _identityService.FindByEmailAsync(loginDto.Email);
        if (user == null)
        {
            throw new ArgumentException("Invalid email or password.");
        }

        bool result = await _identityService.CheckPasswordAsync(user, loginDto.Password);

        if (!result)        
        {
            throw new ArgumentException("Invalid email or password.");
        }

        var sessionDto = new AddUserSessionDTO
        {
            UserId = user.Id,
            IpAddress = _requestContextService.GetIpAddress(),
            UserAgent = _requestContextService.GetUserAgent(),
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };

        await _userSessionService.AddSessionAsync(sessionDto);

        return new UserResultDto
        {
            Success = true,
            Message = "User logged in successfully.",
            UserId = user.Id,
            Name = $"{user.FirstName} {user.LastName}",
            Email = user.Email,
            Token = await _tokenService.GenerateTokenAsync(user),
            RefreshToken = await _tokenService.GenerateRefreshTokenAsync()
        };
    }

    public async Task LogoutAsync(string userId, Guid sessionId)
    {
        await _userSessionService.DeleteSessionAsync(sessionId);
    }



}




