using ECommerce.Application.DTO.Auth; 
using ECommerce.Application.Interfaces.Services;
using ECommerce.Application.Interfaces.Email;
using ECommerce.Domain.Entities.User;
using ECommerce.Domain.Enums.User;
using ECommerce.Domain.Interfaces.Repositories;


namespace ECommerce.Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IMapper _mapper;
    private readonly IIdentityService _identityService;
    private readonly ITokenService _tokenService;
    private readonly ITokenEncoder _tokenEncoder;
    private readonly IUrlBuilder _urlBuilder;
    private readonly IRequestContextService _requestContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationEmailService _notificationEmailService;

    public AuthenticationService(
        IMapper mapper, 
        IIdentityService identityService,
        ITokenService tokenService,
        ITokenEncoder tokenEncoder,
        IUrlBuilder urlBuilder,
        IRequestContextService requestContext,
        IUnitOfWork unitOfWork,
        INotificationEmailService notificationEmailService)
    {
        _mapper = mapper;
        _identityService = identityService;
        _tokenService = tokenService;
        _tokenEncoder = tokenEncoder;
        _urlBuilder = urlBuilder;
        _requestContext = requestContext;
        _unitOfWork = unitOfWork;
        _notificationEmailService = notificationEmailService;
    }

    public async Task<RegisterResultDTO> RegisterAsync(RegisterDTO registerDto)
    {
        
        var existingEmail = await _identityService.FindByEmailAsync(registerDto.Email);

        if (existingEmail != null)
        {
            throw new BadRequestException("Email already exists.");
        }

        var existingUsername = await _identityService.FindByUsernameAsync(registerDto.UserName);

        if (existingUsername != null)
        {
            throw new BadRequestException("Username already exists.");
        }

        if (registerDto.ConfirmPassword != registerDto.Password)
        {
            throw new BadRequestException("Passwords do not match.");
        }


        var user = new User();
        _mapper.Map(registerDto, user);
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        user.EmailConfirmed = false;
        
        var result = await _identityService.CreateUserAsync(user, registerDto.Password);

        if (!result.Success)
        {
            throw new BadRequestException(string.Join("; ", result.Errors));
        }


        await _identityService.AddToRoleAsync(user, AppRoles.Customer.ToString());

        await SendConfirmationEmailAsync(user);

        return new RegisterResultDTO
        {
            Message = "Registration successful. Please check your email to confirm your account.",
        };
    }

    public async Task ConfirmEmailAsync(string email, string token)
    {
        var user = await _identityService.FindByEmailAsync(email)
                   ?? throw new NotFoundException("User not found.");

        var decodedToken = _tokenEncoder.DecodeToken(token);
 
        var result = await _identityService.ConfirmEmailAsync(user, decodedToken);

        if (!result)
            throw new BadRequestException("Email confirmation failed. The link may have expired.");
    }

    public async Task ResendConfirmationEmailAsync(string email)
    {
        var user = await _identityService.FindByEmailAsync(email)
                   ?? throw new NotFoundException("User not found.");

        if (user.EmailConfirmed)
            throw new BadRequestException("Email is already confirmed.");

        await SendConfirmationEmailAsync(user);
    }

    
    public async Task<AuthResultDTO> LoginAsync(LoginDTO loginDto)
    {
        var user = await _identityService.FindByEmailAsync(loginDto.Email);

        if (user == null)
            throw new ArgumentException("Invalid email or password.");

        if (!user.EmailConfirmed && !string.IsNullOrWhiteSpace(user.Email))
        {
            throw new BadRequestException(
                "Please confirm your email address before logging in. " +
                "Check your inbox for the confirmation link.");
        }

        var valid = await _identityService.CheckPasswordAsync(user, loginDto.Password);

        if (!valid)
            throw new ArgumentException("Invalid email or password.");

        return await CreateSessionAndBuildResultAsync(user);
    }

    public async Task ForgotPasswordAsync(string email)
    {
        var user = await _identityService.FindByEmailAsync(email);

        if (user == null)
            throw new NotFoundException("User not found.");

        var rawToken = await _identityService.GeneratePasswordResetTokenAsync(user);
        var encoded = _tokenEncoder.EncodeToken(rawToken);
        var link = _urlBuilder.PasswordReset(user.Email!, encoded);

        await _notificationEmailService.SendPasswordResetAsync(
            user.Email!, $"{user.FirstName} {user.LastName}", link);
    }

    public async Task ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO)
    {
        var user = await _identityService.FindByEmailAsync(resetPasswordDTO.Email);

        if (user == null)
            throw new NotFoundException("User not found.");

        var decodedToken = _tokenEncoder.DecodeToken(resetPasswordDTO.Token);

        if (resetPasswordDTO.NewPassword != resetPasswordDTO.ConfirmPassword)
            throw new BadRequestException("Passwords do not match.");

        var result = await _identityService.ResetPasswordAsync(user, decodedToken, resetPasswordDTO.NewPassword);

        if (!result)
            throw new BadRequestException("Password reset failed. The link may have expired or the password does not meet the requirements.");
    }
  
    public async Task<AuthResultDTO> RefreshTokenAsync(string refreshToken)
    {
        var session = await _unitOfWork.UserSessionRepository.GetByRefreshTokenAsync(refreshToken)
                      ?? throw new BadRequestException("Invalid refresh token.");
 
        if (!session.IsValid)
        {
           
            await RevokeAllAsync(session.UserId);
            throw new BadRequestException(
                "Refresh token has expired or been revoked. Please log in again.");
        }
 
        var user = await _identityService.FindByIdAsync(session.UserId)
                   ?? throw new NotFoundException("User not found.");
 
        var newRefreshToken = await _tokenService.GenerateRefreshTokenAsync();
        session.IsActive        = false;
        session.RevokedAt       = DateTime.UtcNow;
        session.ReplacedByToken = newRefreshToken;
        await _unitOfWork.UserSessionRepository.UpdateAsync(session);   
 

        await _unitOfWork.SaveChangesAsync();
 
        return await CreateSessionAndBuildResultAsync(user, newRefreshToken);
    }
    public async Task RevokeAsync(string refreshToken)
    {
        var session = await _unitOfWork.UserSessionRepository.GetByRefreshTokenAsync(refreshToken);

        if (session is null || !session.IsActive)
            return; 
 
        session.IsActive  = false;
        session.RevokedAt = DateTime.UtcNow;
        await _unitOfWork.UserSessionRepository.UpdateAsync(session);
        await _unitOfWork.SaveChangesAsync();
    }
    public async Task RevokeAllAsync(string userId)
    {
        var activeSessions = await _unitOfWork.UserSessionRepository.GetActiveSessionsAsync(userId);
        var now = DateTime.UtcNow;
 
        foreach (var session in activeSessions)
        {
            session.IsActive  = false;
            session.RevokedAt = now;
            await _unitOfWork.UserSessionRepository.UpdateAsync(session);
        }
 
        await _unitOfWork.SaveChangesAsync();
    }


    private async Task<AuthResultDTO> CreateSessionAndBuildResultAsync(User user, string? refreshTokenOverride = null)
    {
        var accessToken  = await _tokenService.GenerateTokenAsync(user);
        var refreshToken = refreshTokenOverride ?? await _tokenService.GenerateRefreshTokenAsync();
        var roles = await _identityService.GetRolesAsync(user);
 
        var session = new UserSession
        {
            UserId = user.Id,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7),
            IpAddress = _requestContext.GetIpAddress()  ?? "unknown",
            UserAgent = _requestContext.GetUserAgent()  ?? "unknown",
            DeviceInfo = ParseDeviceInfo(_requestContext.GetUserAgent()),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };
 
        await _unitOfWork.UserSessionRepository.AddAsync(session);
        await _unitOfWork.SaveChangesAsync();  

        return new AuthResultDTO
        {
            UserId = user.Id,
            FullName = $"{user.FirstName} {user.LastName}",
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            Email = user.Email ?? string.Empty,
            AccessToken = accessToken,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(60),
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = session.RefreshTokenExpiresAt,
            Roles = roles.ToList()
        }; 

    }

    private async Task SendConfirmationEmailAsync(User user)
    {
        var rawToken = await _identityService.GenerateEmailConfirmationTokenAsync(user);
        var encoded = _tokenEncoder.EncodeToken(rawToken);
        var link = _urlBuilder.EmailConfirmation(user.Email!, encoded);
        await _notificationEmailService.SendEmailConfirmationAsync(user.Email!, $"{user.FirstName} {user.LastName}", link);
    }



    private static string ParseDeviceInfo(string? userAgent)
    {
        if (string.IsNullOrWhiteSpace(userAgent)) return "Unknown device";
        if (userAgent.Contains("iPhone") || userAgent.Contains("iPad")) return "iOS device";
        if (userAgent.Contains("Android"))  return "Android device";
        if (userAgent.Contains("Windows"))  return "Windows PC";
        if (userAgent.Contains("Macintosh")) return "Mac";
        if (userAgent.Contains("Linux"))    return "Linux";
        return "Unknown device";
    }

     


}




